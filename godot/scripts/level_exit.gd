class_name CastleLevelExit
extends StaticBody2D

@export var direction := 1
@export var target_floor := -1

func _ready() -> void:
	add_to_group("interactable")

func interact(_player: CastlePlayer) -> void:
	var destination := target_floor if target_floor >= 0 else GameState.current_floor + direction
	GameState.change_floor(destination)
