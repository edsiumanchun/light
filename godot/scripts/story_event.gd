class_name CastleStoryEvent
extends Area2D

@export var story_floor := -1
var triggered := false

func setup(target_story_floor: int) -> void:
	story_floor = target_story_floor
	body_entered.connect(_on_body_entered)

func _on_body_entered(body: Node) -> void:
	if triggered or story_floor < 0 or not body.is_in_group("player"):
		return
	triggered = true
	StoryManager.start_story_at_floor(story_floor)
