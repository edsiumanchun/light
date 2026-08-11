class_name CastlePickup
extends Area2D

@export var pickup_name := ""
var persistent_id := ""

func setup(name: String, id: String) -> void:
	pickup_name = name
	persistent_id = id
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	if pickup_name.begins_with("Key"):
		GameState.change_key(pickup_name.trim_prefix("Key"), 1)
	elif pickup_name == "PotionRed":
		GameState.change_health(GameState.hp_max)
	GameState.mark_destroyed(persistent_id)
	queue_free()
