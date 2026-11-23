class_name CastleLevelExit
extends StaticBody2D

@export var direction := 1
@export var target_floor := -1
var required_event := 0

func _ready() -> void:
	add_to_group("interactable")

func is_locked() -> bool:
	return not GameState.meets_story_requirements(GameState.current_stage, GameState.current_floor, required_event, 0)

func interact(_player: CastlePlayer) -> void:
	if is_locked():
		return
	var destination := target_floor if target_floor >= 0 else GameState.current_floor + direction
	GameState.change_floor(destination)
