extends Node

signal opening
signal opened
signal line_changed(speaker: String, text: String, left_portrait: String, right_portrait: String)
signal story_cue(cue: String)
signal time_skip_requested
signal dialogue_paused
signal leaving_dialogue
signal closed
signal completed(stage: int, floor: int, event_number: int)
signal event_unlocked(stage: int, floor: int, event_number: int)

var lines: Array[Dictionary] = []
var current_index := -1
var active := false
var waiting_for_cue := false
var dialogue_visible := false
var current_event_number := 0
var current_story_id := ""
var story_stage := -1
var source_floor := -1
var pending_destination := -1
var pending_story_event := 0
var starting_initial_story := false
signal story_selected

func _unhandled_input(event: InputEvent) -> void:
	if not active:
		return
	var advance := event.is_action_pressed("interact")
	advance = advance or (event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT and event.pressed)
	advance = advance or (event is InputEventScreenTouch and event.pressed)
	if advance:
		next_line()
		get_viewport().set_input_as_handled()

func start_story(suffix := "", story_index := 0) -> void:
	start_story_at_floor(GameState.current_floor, suffix, story_index)

func start_story_at_floor(story_floor: int, suffix := "", story_index := 0) -> void:
	_start_story(str(story_floor) + suffix, 0, story_index)

func start_event(event_number: int) -> bool:
	if event_number > 0 and GameState.has_completed_location_story(GameState.current_stage, event_number):
		return false
	var story_id := str(event_number)
	if event_number < 0 and not FileAccess.file_exists(DataLoader.story_path(GameState.current_stage, story_id)):
		story_id = str(absi(event_number))
	return _start_story(story_id, event_number, 0)

func start_initial_story(event_number: int) -> bool:
	if event_number <= 0 or GameState.has_completed_location_story(GameState.current_stage, event_number):
		return false
	if GameState.has_completed_story_event(GameState.current_stage, GameState.current_floor, event_number):
		GameState.complete_location_story(GameState.current_stage, event_number)
		return false
	starting_initial_story = true
	var started := start_event(event_number)
	if not started:
		starting_initial_story = false
	return started

func _start_story(story_id: String, event_number: int, story_index: int) -> bool:
	if active:
		return false
	var path := DataLoader.story_path(GameState.current_stage, story_id)
	var wrapper := DataLoader.load_json(path)
	var stories: Array = wrapper.get("storyArray", [])
	if stories.is_empty():
		return false
	lines.clear()
	var use_room_cues := not story_cue.get_connections().is_empty()
	var left_portrait := ""
	var right_portrait := ""
	var location_unlock_counts: Dictionary = {}
	for index in range(stories.size()):
		var story: Dictionary = stories[index]
		var speaker := str(story.get("name", ""))
		if story.has("leftPrefab"):
			left_portrait = str(story.leftPrefab)
		if story.has("rightPrefab"):
			right_portrait = str(story.rightPrefab)
		for dialogue in story.get("dialogue", []):
			if speaker == "event":
				var command := str(dialogue).strip_edges().to_lower()
				var unlock_number := get_unlock_number(command, event_number)
				var unlock_location := command.trim_prefix("unlock")
				var destination := command.trim_prefix("goto")
				if command.begins_with("unlock") and DataLoader.stage_for_location(unlock_location) >= 0:
					var occurrence := int(location_unlock_counts.get(unlock_location, 0)) + 1
					location_unlock_counts[unlock_location] = occurrence
					if index >= story_index:
						var cue_id := "%s/%s/%s/%d" % [
							DataLoader.location_for_stage(GameState.current_stage), story_id, unlock_location, occurrence
						]
						lines.append({"unlock_location": unlock_location, "cue_id": cue_id})
					continue
				if index < story_index:
					continue
				if unlock_number > 0:
					lines.append({"unlock": unlock_number})
				elif command.begins_with("goto") and DataLoader.stage_for_location(destination) >= 0:
					var stage := DataLoader.stage_for_location(destination)
					if FileAccess.file_exists(DataLoader.floor_path(stage, DataLoader.ENTRY_FLOOR)):
						lines.append({"destination": stage})
				elif command.begins_with("start"):
					var next_story := parse_story_destination(command.trim_prefix("start"))
					if not next_story.is_empty():
						lines.append(next_story)
				elif dialogue == "timefly" and not time_skip_requested.get_connections().is_empty():
					lines.append({"cue": "timefly"})
				elif command == "playerattack":
					lines.append({"cue": command})
				elif use_room_cues and dialogue in ["teacherleave", "teacherback", "playersleep", "fatherarrive", "playerwake", "moverock", "grandpacome"]:
					lines.append({"cue": str(dialogue)})
			elif index >= story_index:
				lines.append({
					"speaker": speaker,
					"text": str(dialogue),
					"left_portrait": left_portrait,
					"right_portrait": right_portrait,
				})
	if lines.is_empty():
		return false
	current_event_number = event_number
	current_story_id = story_id
	story_stage = GameState.current_stage
	source_floor = GameState.current_floor
	pending_destination = -1
	pending_story_event = 0
	active = true
	dialogue_visible = false
	current_index = -1
	story_selected.emit()
	_advance_story()
	return true

func reveal_dialogue() -> void:
	if not active or waiting_for_cue or dialogue_visible or not lines[current_index].has("speaker"):
		return
	dialogue_visible = true
	opened.emit()
	_emit_dialogue_line(lines[current_index])

func next_line() -> void:
	if not active or not dialogue_visible:
		return
	_advance_story()

func _advance_story() -> void:
	current_index += 1
	while current_index < lines.size():
		var command := lines[current_index]
		if command.has("unlock"):
			var unlock_number := int(command.unlock)
			GameState.record_story_unlock(story_stage, source_floor, unlock_number)
			event_unlocked.emit(story_stage, source_floor, unlock_number)
		elif command.has("unlock_location"):
			GameState.advance_location(str(command.unlock_location), str(command.cue_id))
		elif command.has("destination"):
			pending_destination = int(command.destination)
			pending_story_event = int(command.get("story_event", 0))
		else:
			break
		current_index += 1
	if current_index >= lines.size():
		active = false
		dialogue_visible = false
		GameState.record_story_completion(story_stage, source_floor, current_event_number)
		if starting_initial_story:
			GameState.complete_location_story(story_stage, current_event_number)
		starting_initial_story = false
		closed.emit()
		completed.emit(story_stage, source_floor, current_event_number)
		if pending_destination >= 0:
			# Travel after the end of the source scene, including any remaining dialogue.
			open_story_destination.call_deferred(pending_destination, pending_story_event)
		return
	var line: Dictionary = lines[current_index]
	if line.has("cue"):
		waiting_for_cue = true
		if dialogue_visible:
			dialogue_visible = false
			dialogue_paused.emit()
			if line.cue != "timefly" and not leaving_dialogue.get_connections().is_empty():
				leaving_dialogue.emit()
				return
		run_current_cue()
	elif dialogue_visible:
		_emit_dialogue_line(line)
	elif opening.get_connections().is_empty():
		reveal_dialogue()
	else:
		opening.emit()

func parse_story_destination(target: String) -> Dictionary:
	for location in DataLoader.LOCATIONS:
		if not target.begins_with(location):
			continue
		var event := target.trim_prefix(location)
		if not event.is_valid_int() or int(event) <= 0:
			continue
		var stage := DataLoader.stage_for_location(location)
		if FileAccess.file_exists(DataLoader.floor_path(stage, DataLoader.ENTRY_FLOOR)) and FileAccess.file_exists(DataLoader.story_path(stage, str(int(event)))):
			return {"destination": stage, "story_event": int(event)}
	return {}

func open_story_destination(stage: int, event_number: int) -> void:
	GameState.travel_to(stage, DataLoader.ENTRY_FLOOR)
	if event_number > 0:
		start_event(event_number)

func run_current_cue() -> void:
	if active and waiting_for_cue:
		var cue := str(lines[current_index].cue)
		if cue == "timefly":
			time_skip_requested.emit()
		elif cue == "playerattack":
			var player := get_tree().get_first_node_in_group("player") as CastlePlayer
			if is_instance_valid(player):
				var cast := player.cast_story_spell()
				await cast.finished
			finish_story_cue()
		else:
			story_cue.emit(cue)

func _emit_dialogue_line(line: Dictionary) -> void:
	line_changed.emit(
		str(line.speaker),
		str(line.text),
		str(line.left_portrait),
		str(line.right_portrait),
	)

func get_unlock_number(command: String, event_number: int) -> int:
	if command == "unlocknext" and event_number > 0:
		return event_number + 1
	if command.begins_with("unlock:"):
		var target := command.trim_prefix("unlock:").strip_edges()
		if target.is_valid_int():
			return int(target)
	return 0

func finish_story_cue() -> void:
	if waiting_for_cue:
		waiting_for_cue = false
		_advance_story()
