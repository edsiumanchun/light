class_name CastleGate
extends StaticBody2D

@export var key_name := ""

func _ready() -> void:
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	if not key_name.is_empty() and not GameState.change_key(key_name, -1):
		return
	var tween := create_tween().set_parallel(true)
	tween.tween_property(self, "position:y", position.y - 120.0, 0.5)
	tween.tween_property(self, "modulate:a", 0.0, 0.5)
	await tween.finished
	queue_free()
