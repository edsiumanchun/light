class_name DataLoader
extends RefCounted

const JSON_ROOT := "res://Resources/Json"

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
	return load_json("%s/Game/%d/Config.json" % [JSON_ROOT, stage])

static func floor_data(stage: int, floor: int) -> Dictionary:
	return load_json("%s/Game/%d/%d.json" % [JSON_ROOT, stage, floor])

static func enemy_data(stage: int) -> Dictionary:
	return load_json("%s/Game/%d/EnemyData.json" % [JSON_ROOT, stage])

static func sprite_path(folder: String, asset_name: String) -> String:
	for extension in ["png", "jpg", "jpeg"]:
		var path := "res://Resources/Sprites/%s/%s.%s" % [folder, asset_name, extension]
		if ResourceLoader.exists(path):
			return path
	return ""

static func texture(folder: String, asset_name: String) -> Texture2D:
	var path := sprite_path(folder, asset_name)
	return load(path) as Texture2D if not path.is_empty() else null
