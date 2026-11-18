extends Node

signal stats_changed
signal floor_changed(stage: int, floor: int)
signal location_progress_changed(location: String)

const DEFAULT_SAVE_PATH := "user://castle-save.json"
const ENV_PATH := "res://.env"
const SAVE_PATH_ENV := "CASTLE_SAVE_PATH"
const START_STAGE_ENV := "CASTLE_START_STAGE"
const START_LOCATION_ENV := "CASTLE_START_LOCATION"
const START_FLOOR_ENV := "CASTLE_START_FLOOR"

var current_stage := 0
var current_floor := DataLoader.ENTRY_FLOOR
var previous_floor := -1
var exit_h := 4
var exit_v := -1
var hp := 50
var hp_max := 50
var mp := 50
var mp_max := 50
var inventory := {
	"Greed": 0, "Hatred": 0, "Delusion": 0,
	"Copper": 0, "Silver": 0, "Gold": 0,
}
var destroyed := {}
var story_completions: Dictionary = {}
var story_unlocks: Dictionary = {}
var location_progress: Dictionary = {"home": 1}
var applied_location_unlocks: Dictionary = {}
var location_story_progress: Dictionary = {}
var unlock_all_for_testing := false
var music_muted := true
var save_path := DEFAULT_SAVE_PATH

func _ready() -> void:
	save_path = resolve_save_path()
	load_game()
	apply_start_location_override()

func resolve_save_path() -> String:
	var override := env_value(SAVE_PATH_ENV)
	return override if not override.is_empty() else DEFAULT_SAVE_PATH

func env_value(key: String) -> String:
	var system_override := OS.get_environment(key).strip_edges()
	if not system_override.is_empty():
		return system_override
	if FileAccess.file_exists(ENV_PATH):
		var file_override := parse_env_value(FileAccess.get_file_as_string(ENV_PATH), key)
		if not file_override.is_empty():
			return file_override
	return ""

func apply_start_location_override() -> void:
	var stage_value := env_value(START_STAGE_ENV)
	var location_value := env_value(START_LOCATION_ENV)
	var floor_value := env_value(START_FLOOR_ENV)
	if location_value.is_empty() and stage_value.is_empty() and floor_value.is_empty():
		return
	var stage := current_stage
	if not location_value.is_empty():
		stage = DataLoader.stage_for_location(location_value)
	elif not stage_value.is_empty():
		stage = int(stage_value) if stage_value.is_valid_int() else DataLoader.stage_for_location(stage_value)
	var floor := DataLoader.ENTRY_FLOOR if floor_value.is_empty() else DataLoader.floor_number(stage, floor_value)
	var floor_path := DataLoader.floor_path(stage, floor)
	if DataLoader.location_for_stage(stage).is_empty() or not FileAccess.file_exists(floor_path):
		push_warning("Ignoring test start location because %s does not exist." % floor_path)
		return
	current_stage = stage
	current_floor = floor
	# A manually selected test floor was not entered through a door.
	previous_floor = -1

func parse_env_value(contents: String, key: String) -> String:
	for source_line in contents.split("\n"):
		var line := str(source_line).strip_edges()
		if line.is_empty() or line.begins_with("#"):
			continue
		if line.begins_with("export "):
			line = line.trim_prefix("export ").strip_edges()
		var parts := line.split("=", true, 1)
		if parts.size() != 2 or str(parts[0]).strip_edges() != key:
			continue
		var value := str(parts[1]).strip_edges()
		if value.length() >= 2 and (
			(value.begins_with("\"") and value.ends_with("\""))
			or (value.begins_with("'") and value.ends_with("'"))
		):
			value = value.substr(1, value.length() - 2)
		return value
	return ""

func reset_progress_for_testing() -> void:
	current_stage = 0
	current_floor = DataLoader.ENTRY_FLOOR
	previous_floor = -1
	exit_h = 4
	exit_v = -1
	hp = 50
	hp_max = 50
	mp = 50
	mp_max = 50
	for item in inventory:
		inventory[item] = 0
	destroyed.clear()
	story_completions.clear()
	story_unlocks.clear()
	location_progress = {"home": 1}
	applied_location_unlocks.clear()
	location_story_progress.clear()
	unlock_all_for_testing = false
	apply_start_location_override()
	save_game()
	stats_changed.emit()
	floor_changed.emit(current_stage, current_floor)

func jump_to_floor_for_testing(label: String) -> bool:
	var floor := DataLoader.floor_number(current_stage, label)
	if label != DataLoader.floor_label(current_stage, floor) or not FileAccess.file_exists(DataLoader.floor_path(current_stage, floor)):
		return false
	current_floor = floor
	previous_floor = -1
	floor_changed.emit(current_stage, current_floor)
	return true

func change_floor(floor: int, entrance_h := 4, entrance_v := -1) -> void:
	travel_to(current_stage, floor, entrance_h, entrance_v)

func travel_to(stage: int, floor: int, entrance_h := 4, entrance_v := -1) -> void:
	if DataLoader.location_for_stage(stage).is_empty() or not FileAccess.file_exists(DataLoader.floor_path(stage, floor)):
		return
	previous_floor = current_floor if stage == current_stage else -1
	current_stage = stage
	current_floor = floor
	exit_h = entrance_h
	exit_v = entrance_v
	save_game()
	floor_changed.emit(current_stage, current_floor)

func change_health(amount: int) -> bool:
	if hp + amount < 0:
		return false
	hp = clampi(hp + amount, 0, hp_max)
	stats_changed.emit()
	save_game()
	return true

func change_key(key_name: String, amount: int) -> bool:
	return change_inventory(key_name, amount)

func open_key_lock(id: String, key_name: String) -> bool:
	var save_id := object_id(current_stage, current_floor, id)
	if destroyed.has(save_id):
		return true
	if not key_name.is_empty():
		var count := int(inventory.get(key_name, 0))
		if count < 1:
			return false
		inventory[key_name] = count - 1
	destroyed[save_id] = true
	save_game()
	stats_changed.emit()
	return true

func change_inventory(item_name: String, amount: int) -> bool:
	var value: int = inventory.get(item_name, 0) + amount
	if value < 0:
		return false
	inventory[item_name] = value
	stats_changed.emit()
	save_game()
	return true

func object_id(stage: int, floor: int, id: String) -> String:
	return "%s/%d/%s" % [DataLoader.location_for_stage(stage), floor, id]

func mark_destroyed(id: String) -> void:
	destroyed[object_id(current_stage, current_floor, id)] = true
	save_game()

func is_destroyed(id: String) -> bool:
	if destroyed.has(object_id(current_stage, current_floor, id)):
		return true
	var old_stage := 1 if current_stage == 2 else 0
	var old_floor := current_floor + 1 if current_stage == 1 else current_floor
	return destroyed.has("%d/%d/%s" % [old_stage, old_floor, id])

func has_completed_story_event(stage: int, floor: int, event_number: int) -> bool:
	return story_completions.has(object_id(stage, floor, str(event_number)))

func record_story_completion(stage: int, floor: int, event_number: int) -> void:
	if event_number == 0 or has_completed_story_event(stage, floor, event_number):
		return
	story_completions[object_id(stage, floor, str(event_number))] = true
	save_game()

func has_unlocked_story_event(stage: int, floor: int, event_number: int) -> bool:
	return story_unlocks.has(object_id(stage, floor, str(event_number)))

func meets_story_requirements(stage: int, floor: int, required_event: int, unlock_event: int) -> bool:
	if unlock_all_for_testing:
		return true
	return (
		(required_event == 0 or has_completed_story_event(stage, floor, required_event))
		and (unlock_event == 0 or has_unlocked_story_event(stage, floor, unlock_event))
	)

func record_story_unlock(stage: int, floor: int, event_number: int) -> void:
	if has_unlocked_story_event(stage, floor, event_number):
		return
	story_unlocks[object_id(stage, floor, str(event_number))] = true
	save_game()

func progress_for_location(location: String) -> int:
	if unlock_all_for_testing:
		return 2147483647
	return int(location_progress.get(location, 0))

func completed_story_for_location(location: String) -> int:
	return int(location_story_progress.get(location, 0))

func has_completed_location_story(stage: int, story_number: int) -> bool:
	var location := DataLoader.location_for_stage(stage)
	return not location.is_empty() and story_number <= completed_story_for_location(location)

func complete_location_story(stage: int, story_number: int) -> void:
	var location := DataLoader.location_for_stage(stage)
	if location.is_empty() or story_number <= completed_story_for_location(location):
		return
	location_story_progress[location] = story_number
	save_game()

func unlock_all_stages_for_testing() -> void:
	if unlock_all_for_testing:
		return
	unlock_all_for_testing = true
	location_progress_changed.emit(DataLoader.location_for_stage(current_stage))

func advance_location(location: String, cue_id: String) -> void:
	if DataLoader.stage_for_location(location) < 0 or applied_location_unlocks.has(cue_id):
		return
	applied_location_unlocks[cue_id] = true
	location_progress[location] = int(location_progress.get(location, 0)) + 1
	save_game()
	location_progress_changed.emit(location)

func save_game() -> void:
	var file := FileAccess.open(save_path, FileAccess.WRITE)
	if file:
		file.store_string(JSON.stringify({
			"location": DataLoader.location_for_stage(current_stage), "floor": current_floor,
			"previous_floor": previous_floor,
			"exit_h": exit_h, "exit_v": exit_v,
			"hp": hp, "hp_max": hp_max, "mp": mp, "mp_max": mp_max,
			"inventory": inventory, "destroyed": destroyed,
			"story_completions": story_completions,
			"story_unlocks": story_unlocks,
			"location_progress": location_progress,
			"applied_location_unlocks": applied_location_unlocks,
			"location_story_progress": location_story_progress,
			"music_muted": music_muted
		}))

func load_game() -> void:
	if not FileAccess.file_exists(save_path):
		return
	var data = JSON.parse_string(FileAccess.get_file_as_string(save_path))
	if not data is Dictionary:
		return
	var saved_location := str(data.get("location", ""))
	var saved_floor := int(data.get("floor", DataLoader.ENTRY_FLOOR))
	var saved_stage := DataLoader.stage_for_location(saved_location)
	if saved_location.is_empty():
		match int(data.get("stage", 0)):
			0:
				saved_stage = 0 if saved_floor == 0 else 1
				saved_floor = maxi(saved_floor - 1, 0)
			1:
				saved_stage = 2
	if saved_floor == 0 and saved_stage >= 0 and not FileAccess.file_exists(DataLoader.floor_path(saved_stage, saved_floor)):
		saved_floor = DataLoader.ENTRY_FLOOR
	if saved_stage >= 0 and FileAccess.file_exists(DataLoader.floor_path(saved_stage, saved_floor)):
		current_stage = saved_stage
		current_floor = saved_floor
		if not saved_location.is_empty():
			previous_floor = int(data.get("previous_floor", -1))
	exit_h = int(data.get("exit_h", exit_h))
	exit_v = int(data.get("exit_v", exit_v))
	hp = int(data.get("hp", hp))
	hp_max = int(data.get("hp_max", hp_max))
	mp = int(data.get("mp", mp))
	mp_max = int(data.get("mp_max", mp_max))
	inventory = data.get("inventory", inventory)
	destroyed = data.get("destroyed", destroyed)
	story_completions = data.get("story_completions", {})
	story_unlocks = data.get("story_unlocks", {})
	for records: Dictionary in [destroyed, story_completions, story_unlocks]:
		for key: String in records.keys():
			if key.begins_with("home/0/"):
				records["home/1/" + key.trim_prefix("home/0/")] = records[key]
				records.erase(key)
	location_progress = data.get("location_progress", {"home": 1})
	applied_location_unlocks = data.get("applied_location_unlocks", {})
	location_story_progress = data.get("location_story_progress", {})
	if not data.has("location_progress"):
		if has_unlocked_story_event(0, DataLoader.ENTRY_FLOOR, 2):
			location_progress["home"] = 2
			applied_location_unlocks["home/1/home/1"] = true
		if has_unlocked_story_event(1, 1, 2):
			location_progress["dojo"] = 2
			applied_location_unlocks["home/2/dojo/1"] = true
			applied_location_unlocks["dojo/1/dojo/1"] = true
	music_muted = bool(data.get("music_muted", music_muted))
