extends Node

signal opening
signal opened
signal line_changed(speaker: String, text: String, left_portrait: String, right_portrait: String)
signal story_cue(cue: String)
signal time_skip_requested
signal dialogue_paused
signal leaving_dialogue
signal closed
signal completed(stage: int, floor: int, suffix: String)
signal event_unlocked(stage: int, floor: int, event_number: int)

var lines: Array[Dictionary] = []
var current_index := -1
var active := false
var waiting_for_cue := false
var dialogue_visible := false
var current_suffix := ""
var story_stage := -1
var source_floor := -1

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
	if active:
		return
	var path := "%s/Story/%d/%d%s.json" % [DataLoader.JSON_ROOT, GameState.current_stage, story_floor, suffix]
	var wrapper := DataLoader.load_json(path)
	var stories: Array = wrapper.get("storyArray", [])
	if stories.is_empty():
		return
	lines.clear()
	var use_room_cues := GameState.current_stage == 0 and story_floor == 0 and not story_cue.get_connections().is_empty()
	var left_portrait := ""
	var right_portrait := ""
	for index in range(story_index, stories.size()):
		var story: Dictionary = stories[index]
		var speaker := str(story.get("name", ""))
		if story.has("leftPrefab"):
			left_portrait = str(story.leftPrefab)
		if story.has("rightPrefab"):
			right_portrait = str(story.rightPrefab)
		for dialogue in story.get("dialogue", []):
			if speaker == "event":
				var unlock_number := get_unlock_number(str(dialogue), suffix)
				if unlock_number > 0:
					lines.append({"unlock": unlock_number})
				elif dialogue == "timefly" and not time_skip_requested.get_connections().is_empty():
					lines.append({"cue": "timefly"})
				elif use_room_cues and dialogue in ["teacherleave", "teacherback", "playersleep", "fatherarrive", "playerwake"]:
					lines.append({"cue": str(dialogue)})
			else:
				lines.append({
					"speaker": speaker,
					"text": str(dialogue),
					"left_portrait": left_portrait,
					"right_portrait": right_portrait,
				})
	if lines.is_empty():
		return
	current_suffix = suffix
	story_stage = GameState.current_stage
	source_floor = story_floor
	active = true
	dialogue_visible = false
	current_index = -1
	_advance_story()

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
	while current_index < lines.size() and lines[current_index].has("unlock"):
		event_unlocked.emit(story_stage, source_floor, int(lines[current_index].unlock))
		current_index += 1
	if current_index >= lines.size():
		active = false
		dialogue_visible = false
		closed.emit()
		completed.emit(story_stage, source_floor, current_suffix)
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

func run_current_cue() -> void:
	if active and waiting_for_cue:
		var cue := str(lines[current_index].cue)
		if cue == "timefly":
			time_skip_requested.emit()
		else:
			story_cue.emit(cue)

func _emit_dialogue_line(line: Dictionary) -> void:
	line_changed.emit(
		str(line.speaker),
		str(line.text),
		str(line.left_portrait),
		str(line.right_portrait),
	)

func get_unlock_number(command: String, suffix: String) -> int:
	if command == "unlocknext" and suffix.begins_with("-"):
		var event_number := suffix.substr(1)
		if event_number.is_valid_int() and int(event_number) > 0:
			return int(event_number) + 1
	if command.begins_with("unlock:"):
		var target := command.trim_prefix("unlock:").strip_edges()
		if target.is_valid_int():
			return int(target)
	return 0

func finish_story_cue() -> void:
	if waiting_for_cue:
		waiting_for_cue = false
		_advance_story()
