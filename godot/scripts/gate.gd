class_name CastleGate
extends StaticBody2D

@export var key_name := ""
var persistent_id := ""
var switch_controlled := false
var gate_sprite: Sprite2D
var opening_window: Rect2
var retracts_down := false
var opening := false
var motion: Tween
var closed_sprite_y := 0.0
var clipped := false

func _ready() -> void:
	GameState.location_progress_changed.connect(_on_location_progress_changed)
	if not switch_controlled:
		add_to_group("interactable")

func _on_location_progress_changed(_location: String) -> void:
	if GameState.unlock_all_for_testing:
		collision_layer = 0
		open()

func interact(_player: CastlePlayer) -> void:
	if opening or switch_controlled:
		return
	if not (GameState.open_key_lock(persistent_id, key_name) if not persistent_id.is_empty() else key_name.is_empty() or GameState.change_key(key_name, -1)):
		return
	open()

func open() -> void:
	if opening:
		return
	opening = true
	remove_from_group("interactable")
	set_retracted(true)

func set_retracted(retracted: bool, animate := true) -> void:
	if GameState.unlock_all_for_testing and not retracted:
		return
	if motion != null and motion.is_valid():
		motion.kill()
	if is_instance_valid(gate_sprite) and gate_sprite.visible:
		prepare_clip()
		var travel := opening_window.size.y if retracts_down else -opening_window.size.y
		var target_y := closed_sprite_y + (travel if retracted else 0.0)
		if animate:
			motion = create_tween().set_trans(Tween.TRANS_CUBIC).set_ease(Tween.EASE_IN_OUT)
			motion.tween_property(gate_sprite, "position:y", target_y, 0.65)
			motion.tween_callback(_finish_motion.bind(retracted))
			return
		gate_sprite.position.y = target_y
	_finish_motion(retracted)

func prepare_clip() -> void:
	if not clipped:
		var clip := Polygon2D.new()
		clip.polygon = PackedVector2Array([
			opening_window.position,
			Vector2(opening_window.end.x, opening_window.position.y),
			opening_window.end,
			Vector2(opening_window.position.x, opening_window.end.y),
		])
		clip.clip_children = CanvasItem.CLIP_CHILDREN_ONLY
		add_child(clip)
		gate_sprite.reparent(clip)
		closed_sprite_y = gate_sprite.position.y
		clipped = true

func _finish_motion(retracted: bool) -> void:
	collision_layer = 0 if retracted else 5
	if retracted and not switch_controlled:
		queue_free()
