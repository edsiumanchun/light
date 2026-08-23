class_name CastleDoor
extends Area2D

@export var target_floor := -1

func setup(destination_floor: int) -> void:
	target_floor = destination_floor
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	if target_floor < 0 or target_floor == GameState.current_floor:
		return
	GameState.change_floor(target_floor)
