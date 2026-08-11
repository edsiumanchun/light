extends Node

signal stats_changed
signal floor_changed(stage: int, floor: int)

const SAVE_PATH := "user://castle-save.json"

var current_stage := 0
var current_floor := 0
var exit_h := 4
var exit_v := -1
var hp := 50
var hp_max := 50
var mp := 50
var mp_max := 50
var inventory := {"Copper": 0, "Silver": 0, "Gold": 0}
var destroyed := {}

func _ready() -> void:
	load_game()

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
	var value: int = inventory.get(key_name, 0) + amount
	if value < 0:
		return false
	inventory[key_name] = value
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
	var file := FileAccess.open(SAVE_PATH, FileAccess.WRITE)
	if file:
		file.store_string(JSON.stringify({
			"stage": current_stage, "floor": current_floor,
			"exit_h": exit_h, "exit_v": exit_v,
			"hp": hp, "hp_max": hp_max, "mp": mp, "mp_max": mp_max,
			"inventory": inventory, "destroyed": destroyed
		}))

func load_game() -> void:
	if not FileAccess.file_exists(SAVE_PATH):
		return
	var data = JSON.parse_string(FileAccess.get_file_as_string(SAVE_PATH))
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
