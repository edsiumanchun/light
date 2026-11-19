extends Area2D

signal changed(is_on: bool)

var persistent_id := ""
var reusable := false
var is_on := false
var artwork: Sprite2D
var handle: Node2D
var handle_motion: Tween

func _ready() -> void:
	add_to_group("interactable")
	update_artwork(false)

func interact(_player: CastlePlayer) -> void:
	if is_queued_for_deletion():
		return
	is_on = not is_on
	GameState.set_destroyed(persistent_id, is_on)
	update_artwork(true)
	changed.emit(is_on)
	if not reusable:
		queue_free()

func update_artwork(animate: bool) -> void:
	if not reusable:
		return
	if artwork != null:
		artwork.self_modulate = Color.WHITE if is_on else Color(0.78, 0.78, 0.78)
	if handle == null:
		return
	var angle := deg_to_rad(38.0 if is_on else -38.0)
	if handle_motion != null and handle_motion.is_valid():
		handle_motion.kill()
	if animate:
		handle_motion = create_tween().set_trans(Tween.TRANS_CUBIC).set_ease(Tween.EASE_IN_OUT)
		handle_motion.tween_property(handle, "rotation", angle, 0.28)
	else:
		handle.rotation = angle
