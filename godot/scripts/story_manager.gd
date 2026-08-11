extends Node

signal opened
signal line_changed(speaker: String, text: String)
signal closed

var lines: Array[Dictionary] = []
var current_index := -1
var active := false

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
	var path := "%s/Story/%d/%d%s.json" % [DataLoader.JSON_ROOT, GameState.current_stage, GameState.current_floor, suffix]
	var wrapper := DataLoader.load_json(path)
	var stories: Array = wrapper.get("storyArray", [])
	if stories.is_empty():
		return
	lines.clear()
	for index in range(story_index, stories.size()):
		var story: Dictionary = stories[index]
		var speaker := str(story.get("name", ""))
		for dialogue in story.get("dialogue", []):
			if speaker != "event":
				lines.append({"speaker": speaker, "text": str(dialogue)})
	if lines.is_empty():
		return
	active = true
	current_index = -1
	opened.emit()
	next_line()

func next_line() -> void:
	if not active:
		return
	current_index += 1
	if current_index >= lines.size():
		active = false
		closed.emit()
		return
	var line: Dictionary = lines[current_index]
	line_changed.emit(str(line.speaker), str(line.text))
