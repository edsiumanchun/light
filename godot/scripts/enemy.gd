class_name CastleEnemy
extends StaticBody2D

@export var enemy_name := "Enemy"
@export var max_hp := 10
@export var damage := 1
var current_hp := 10
var persistent_id := ""
var reward := 1

func setup(data: Dictionary, id: String) -> void:
	persistent_id = id
	enemy_name = str(data.get("name", enemy_name))
	max_hp = int(data.get("hp", max_hp))
	damage = int(data.get("damage", damage))
	reward = int(data.get("reward", reward))
	current_hp = max_hp
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	if not GameState.change_health(-damage):
		return
	current_hp -= 10
	if current_hp <= 0:
		for currency in ["Greed", "Hatred", "Delusion"]:
			if enemy_name.begins_with(currency):
				GameState.change_inventory(currency, reward)
				break
		GameState.mark_destroyed(persistent_id)
		queue_free()
