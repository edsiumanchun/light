class_name CastleGate
extends StaticBody2D

@export var key_name := ""
var persistent_id := ""
var switch_controlled := false
var opening := false

func _ready() -> void:
	if not switch_controlled:
		add_to_group("interactable")

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
	collision_layer = 0
	var tween := create_tween().set_parallel(true)
	tween.tween_property(self, "position:y", position.y - 120.0, 0.5)
	tween.tween_property(self, "modulate:a", 0.0, 0.5)
	await tween.finished
	queue_free()
