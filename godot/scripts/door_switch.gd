extends Area2D

signal changed(is_on: bool)

var persistent_id := ""
var reusable := false
var is_on := false
var artwork: Sprite2D
var handle: Node2D
var handle_motion: Tween
var story_event := 0

func set_story_event(number: int) -> void:
	story_event = number
	if is_on:
		if story_event > 0:
			add_to_group("interactable")
		else:
			remove_from_group("interactable")

func _ready() -> void:
	if not is_on:
		add_to_group("interactable")
	update_artwork(false)

func interact(_player: CastlePlayer) -> void:
	if is_queued_for_deletion() or StoryManager.active:
		return
	if not is_on:
		is_on = true
		GameState.mark_destroyed(persistent_id)
		update_artwork(true)
		changed.emit(true)
	if story_event == 0 or StoryManager.start_event(story_event):
		remove_from_group("interactable")
	if not reusable:
		queue_free()

func update_artwork(animate: bool) -> void:
	if not reusable:
		return
	if artwork != null:
		artwork.self_modulate = Color.WHITE if is_on else Color(0.78, 0.78, 0.78)
	if handle == null:
		return
	var angle := deg_to_rad(50.0 if is_on else -50.0)
	if handle_motion != null and handle_motion.is_valid():
		handle_motion.kill()
	if animate:
		handle_motion = create_tween().set_trans(Tween.TRANS_CUBIC).set_ease(Tween.EASE_IN_OUT)
		handle_motion.tween_property(handle, "rotation", angle, 0.28)
	else:
		handle.rotation = angle
