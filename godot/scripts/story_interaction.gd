class_name StoryInteraction
extends StaticBody2D

@export var suffix := ""
@export var story_index := 0

func _ready() -> void:
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	StoryManager.start_story(suffix, story_index)

func _input_event(_viewport: Node, event: InputEvent, _shape_index: int) -> void:
	var clicked: bool = false
	if event is InputEventMouseButton:
		clicked = event.button_index == MOUSE_BUTTON_LEFT and event.pressed
	elif event is InputEventScreenTouch:
		clicked = event.pressed
	if clicked:
		StoryManager.start_story(suffix, story_index)
		get_viewport().set_input_as_handled()
