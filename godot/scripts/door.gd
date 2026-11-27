class_name CastleDoor
extends Area2D

@export var target_floor := -1
var target_stage := -1
var required_event := 0
var unlock_event := 0
var required_location_progress := 0
var source_stage := -1
var source_floor := -1
var key_name := ""
var persistent_id := ""
var required_gates: Array = []
var opens_world_map := false
var discovers_destination := false

func setup(destination_floor: int) -> void:
	target_floor = destination_floor
	source_stage = GameState.current_stage
	source_floor = GameState.current_floor
	add_to_group("interactable")

func is_locked() -> bool:
	if GameState.unlock_all_for_testing:
		return false
	if GameState.progress_for_location(DataLoader.location_for_stage(source_stage)) < required_location_progress:
		return true
	for gate_id in required_gates:
		if not GameState.is_destroyed(str(gate_id)):
			return true
	if not key_name.is_empty() and not GameState.is_destroyed(persistent_id) and int(GameState.inventory.get(key_name, 0)) < 1:
		return true
	return not GameState.meets_story_requirements(source_stage, source_floor, required_event, unlock_event)

func interact(_player: CastlePlayer) -> void:
	if is_locked():
		return
	if opens_world_map:
		GameState.world_map_requested.emit()
		return
	var stage := GameState.current_stage if target_stage < 0 else target_stage
	if target_floor < 0 or (stage == GameState.current_stage and target_floor == GameState.current_floor):
		return
	if not FileAccess.file_exists(DataLoader.floor_path(stage, target_floor)):
		return
	if not GameState.unlock_all_for_testing and not key_name.is_empty() and not GameState.open_key_lock(persistent_id, key_name):
		return
	if discovers_destination:
		var location := DataLoader.location_for_stage(stage)
		if int(GameState.location_progress.get(location, 0)) == 0:
			GameState.advance_location(location, "discover_route_" + location)
	GameState.travel_to(stage, target_floor)
