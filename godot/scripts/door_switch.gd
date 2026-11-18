extends Area2D

signal activated

var persistent_id := ""

func _ready() -> void:
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	if is_queued_for_deletion():
		return
	GameState.mark_destroyed(persistent_id)
	activated.emit()
	queue_free()
