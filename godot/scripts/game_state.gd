extends Node

signal stats_changed
signal floor_changed(stage: int, floor: int)

const DEFAULT_SAVE_PATH := "user://castle-save.json"
const ENV_PATH := "res://.env"
const SAVE_PATH_ENV := "CASTLE_SAVE_PATH"
const START_STAGE_ENV := "CASTLE_START_STAGE"
const START_FLOOR_ENV := "CASTLE_START_FLOOR"

var current_stage := 0
var current_floor := 0
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
	var floor_value := env_value(START_FLOOR_ENV)
	if stage_value.is_empty() and floor_value.is_empty():
		return
	var stage := current_stage if stage_value.is_empty() else int(stage_value)
	var floor := current_floor if floor_value.is_empty() else int(floor_value)
	var floor_path := "res://Resources/Json/Game/%d/%d.json" % [stage, floor]
	if not FileAccess.file_exists(floor_path):
		push_warning("Ignoring test start location because %s does not exist." % floor_path)
		return
	current_stage = stage
	current_floor = floor

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

func change_floor(floor: int, entrance_h := 4, entrance_v := -1) -> void:
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

func change_inventory(item_name: String, amount: int) -> bool:
	var value: int = inventory.get(item_name, 0) + amount
	if value < 0:
		return false
	inventory[item_name] = value
	stats_changed.emit()
	save_game()
	return true

func object_id(stage: int, floor: int, id: String) -> String:
	return "%d/%d/%s" % [stage, floor, id]

func mark_destroyed(id: String) -> void:
	destroyed[object_id(current_stage, current_floor, id)] = true
	save_game()

func is_destroyed(id: String) -> bool:
	return destroyed.has(object_id(current_stage, current_floor, id))

func save_game() -> void:
	var file := FileAccess.open(save_path, FileAccess.WRITE)
	if file:
		file.store_string(JSON.stringify({
			"stage": current_stage, "floor": current_floor,
			"exit_h": exit_h, "exit_v": exit_v,
			"hp": hp, "hp_max": hp_max, "mp": mp, "mp_max": mp_max,
			"inventory": inventory, "destroyed": destroyed,
			"music_muted": music_muted
		}))

func load_game() -> void:
	if not FileAccess.file_exists(save_path):
		return
	var data = JSON.parse_string(FileAccess.get_file_as_string(save_path))
	if not data is Dictionary:
		return
	current_stage = int(data.get("stage", current_stage))
	current_floor = int(data.get("floor", current_floor))
	exit_h = int(data.get("exit_h", exit_h))
	exit_v = int(data.get("exit_v", exit_v))
	hp = int(data.get("hp", hp))
	hp_max = int(data.get("hp_max", hp_max))
	mp = int(data.get("mp", mp))
	mp_max = int(data.get("mp_max", mp_max))
	inventory = data.get("inventory", inventory)
	destroyed = data.get("destroyed", destroyed)
	music_muted = bool(data.get("music_muted", music_muted))
