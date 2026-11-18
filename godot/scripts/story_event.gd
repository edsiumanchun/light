class_name CastleStoryEvent
extends Area2D

@export var event_number := 0
var triggered := false

func setup(number: int) -> void:
	event_number = number
	body_entered.connect(_on_body_entered)

func _on_body_entered(body: Node) -> void:
	if triggered or event_number == 0 or not body.is_in_group("player"):
		return
	triggered = StoryManager.start_event(event_number)
