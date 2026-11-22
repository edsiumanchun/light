class_name DataLoader
extends RefCounted

const JSON_ROOT := "res://Resources/Json"
const LOCATIONS := ["home", "dojo", "frontier", "inn"]
const ENTRY_FLOOR := 1

static func location_for_stage(stage: int) -> String:
	return LOCATIONS[stage] if stage >= 0 and stage < LOCATIONS.size() else ""

static func stage_for_location(location: String) -> int:
	return LOCATIONS.find(location)

static func floor_path(stage: int, floor: int) -> String:
	return "%s/Game/%s/%s.json" % [JSON_ROOT, location_for_stage(stage), floor_label(stage, floor)]

static func floor_label(stage: int, floor: int) -> String:
	if location_for_stage(stage) == "dojo":
		match floor:
			100: return "0a"
			101: return "0b"
	return str(floor)

static func floor_number(stage: int, value: Variant) -> int:
	if location_for_stage(stage) == "dojo":
		match str(value):
			"0a": return 100
			"0b": return 101
	return int(value)

static func story_path(stage: int, story_id: String) -> String:
	return "%s/Story/%s/%s.json" % [JSON_ROOT, location_for_stage(stage), story_id]

static func load_json(path: String) -> Dictionary:
	if not FileAccess.file_exists(path):
		push_error("Missing JSON file: " + path)
		return {}
	var parsed = JSON.parse_string(FileAccess.get_file_as_string(path))
	if not parsed is Dictionary:
		push_error("Invalid JSON file: " + path)
		return {}
	return parsed

static func stage_config(stage: int) -> Dictionary:
	return load_json("%s/Game/%s/Config.json" % [JSON_ROOT, location_for_stage(stage)])

static func floor_data(stage: int, floor: int) -> Dictionary:
	return load_json(floor_path(stage, floor))

static func enemy_data(stage: int) -> Dictionary:
	return load_json("%s/Game/%s/EnemyData.json" % [JSON_ROOT, location_for_stage(stage)])

static func sprite_path(folder: String, asset_name: String) -> String:
	for extension in ["png", "jpg", "jpeg", "svg"]:
		var path := "res://Resources/Sprites/%s/%s.%s" % [folder, asset_name, extension]
		if ResourceLoader.exists(path):
			return path
	return ""

static func texture(folder: String, asset_name: String) -> Texture2D:
	var path := sprite_path(folder, asset_name)
	return load(path) as Texture2D if not path.is_empty() else null
