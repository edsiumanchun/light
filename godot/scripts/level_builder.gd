class_name LevelBuilder
extends Node2D

const PLAYER_SCENE := preload("res://godot/scenes/player.tscn")
const ENEMY_SCRIPT := preload("res://godot/scripts/enemy.gd")
const GATE_SCRIPT := preload("res://godot/scripts/gate.gd")
const PICKUP_SCRIPT := preload("res://godot/scripts/pickup.gd")
const EXIT_SCRIPT := preload("res://godot/scripts/level_exit.gd")
const STORY_SCRIPT := preload("res://godot/scripts/story_interaction.gd")

const VIEW_SIZE := Vector2(1200, 675)
const CELL_SIZE := Vector2(190, 120)
const GRID_ORIGIN := Vector2(125, 75)

var config := {}
var paths := {}
var enemy_definitions := {}
var player: CastlePlayer

func build(stage: int, floor: int) -> CastlePlayer:
	clear_level()
	config = DataLoader.stage_config(stage)
	paths = config.get("paths", {})
	index_enemy_data(DataLoader.enemy_data(stage))
	var floor_definition := DataLoader.floor_data(stage, floor)
	for group_index in range(3):
		var assets: Array = floor_definition.get("assets%d" % group_index, [])
		for asset in assets:
			build_asset(asset, group_index)
	return player

func clear_level() -> void:
	for child in get_children():
		child.queue_free()
	player = null

func index_enemy_data(data: Dictionary) -> void:
	enemy_definitions.clear()
	for definition in data.get("targets", []):
		enemy_definitions[str(definition.get("name", ""))] = definition

func build_asset(asset: Dictionary, group_index: int) -> void:
	var type := str(asset.get("name", ""))
	if type == "Background":
		build_background(asset, group_index)
		return
	if type in ["Audio", "Exit", "Exit2", "Teleport", "TeleportExit", "WallSkip", "Effect", "ShowStory", "GoUp"]:
		return
	for row in range(7):
		var cells: Array = asset.get("gridArray%d" % row, [])
		for column in range(cells.size()):
			var signed_value := int(cells[column])
			if signed_value == 0:
				continue
			var position := GRID_ORIGIN + Vector2(column * CELL_SIZE.x, row * CELL_SIZE.y)
			position.x += group_index * VIEW_SIZE.x
			match type:
				"PlateH": build_plate(position, signed_value, false)
				"PlateV": build_plate(position, signed_value, true)
				"Object": build_object(position + Vector2(0, CELL_SIZE.y), signed_value, "%d_%d_%d" % [group_index, row, column])

func build_background(asset: Dictionary, group_index: int) -> void:
	var indexes: Array = asset.get("gridArray0", [0])
	var names: Array = paths.get("backgrounds", [])
	if indexes.is_empty() or int(indexes[0]) >= names.size():
		return
	var sprite := Sprite2D.new()
	sprite.texture = DataLoader.texture("Background", str(names[int(indexes[0])]))
	if sprite.texture == null:
		return
	sprite.centered = true
	sprite.position = Vector2(group_index * VIEW_SIZE.x + VIEW_SIZE.x * 0.5, VIEW_SIZE.y * 0.5)
	sprite.scale = VIEW_SIZE / sprite.texture.get_size()
	sprite.z_index = -100
	add_child(sprite)

func build_plate(position: Vector2, signed_value: int, vertical: bool) -> void:
	var names: Array = paths.get("plateV" if vertical else "plateH", [])
	var index := absi(signed_value) - 1
	if index < 0 or index >= names.size():
		return
	var asset_name := str(names[index])
	var body := StaticBody2D.new()
	body.position = position
	body.collision_layer = 1
	body.collision_mask = 2
	add_sprite(body, "PlateV" if vertical else "PlateH", asset_name, Vector2(0.45, 0.45))
	var shape := CollisionShape2D.new()
	var rectangle := RectangleShape2D.new()
	rectangle.size = Vector2(35, CELL_SIZE.y) if vertical else Vector2(CELL_SIZE.x, 30)
	shape.shape = rectangle
	body.add_child(shape)
	if asset_name.begins_with("Gate"):
		body.set_script(GATE_SCRIPT)
		body.collision_layer = 5
		body.set("key_name", asset_name.trim_prefix("Gate"))
	elif asset_name.begins_with("GoDown"):
		body.set_script(EXIT_SCRIPT)
		body.collision_layer = 5
		body.set("direction", -1 if asset_name.contains("Revert") else 1)
	elif asset_name == "GoUp":
		body.set_script(EXIT_SCRIPT)
		body.collision_layer = 5
		body.set("direction", -1)
	add_child(body)

func build_object(position: Vector2, signed_value: int, id: String) -> void:
	var index := absi(signed_value) - 1
	if index == 0:
		if player == null:
			player = PLAYER_SCENE.instantiate()
			player.position = position
			add_child(player)
		return
	if GameState.is_destroyed(id):
		return
	if index >= 1000:
		build_enemy(position, "Greed", id)
	elif index >= 90:
		build_from_list(position, index % 10, "bosses", "Enemies", id, "enemy")
	elif index >= 50:
		build_from_list(position, index % 10, "triggers", "Triggers", id, "pickup")
	elif index >= 40:
		build_from_list(position, index % 10, "decorations", "Decorations", id, "visual")
	elif index >= 30:
		build_from_list(position, index % 10, "obstacles", "Obstacles", id, "solid")
	elif index >= 20:
		build_from_list(position, index % 10, "items", "Items", id, "pickup")
	elif index >= 10:
		build_from_list(position, index % 10, "enemies", "Enemies", id, "enemy")
	elif index > 0:
		build_story_character(position, index, id)

func build_from_list(position: Vector2, index: int, list_name: String, folder: String, id: String, kind: String) -> void:
	var names: Array = paths.get(list_name, [])
	if index < 0 or index >= names.size():
		return
	var asset_name := str(names[index])
	match kind:
		"enemy": build_enemy(position, asset_name, id)
		"pickup": build_pickup(position, asset_name, folder, id)
		"solid": build_visual(position, asset_name, folder, true)
		_: build_visual(position, asset_name, folder, false)

func build_enemy(position: Vector2, asset_name: String, id: String) -> void:
	var enemy := StaticBody2D.new()
	enemy.set_script(ENEMY_SCRIPT)
	enemy.position = position
	enemy.collision_layer = 5
	enemy.collision_mask = 2
	add_child(enemy)
	add_sprite(enemy, "Enemies", asset_name, Vector2(0.5, 0.5), Color(0.9, 0.35, 0.35))
	add_collision(enemy, Vector2(80, 120))
	enemy.call("setup", enemy_definitions.get(asset_name, {"name": asset_name, "hp": 10, "damage": 1}), id)

func build_pickup(position: Vector2, asset_name: String, folder: String, id: String) -> void:
	var pickup := Area2D.new()
	pickup.set_script(PICKUP_SCRIPT)
	pickup.position = position
	pickup.collision_layer = 4
	pickup.collision_mask = 2
	add_child(pickup)
	add_sprite(pickup, folder, asset_name, Vector2(0.4, 0.4), Color(0.3, 0.9, 0.5))
	add_collision(pickup, Vector2(70, 70))
	pickup.call("setup", asset_name, id)

func build_story_character(position: Vector2, index: int, _id: String) -> void:
	var names: Array = paths.get("characters", [])
	if index < 0 or index >= names.size():
		return
	var character := StaticBody2D.new()
	character.set_script(STORY_SCRIPT)
	character.position = position
	character.collision_layer = 5
	character.collision_mask = 2
	var sprite := add_sprite(character, "Story", str(names[index]).trim_prefix("Story").to_lower(), Vector2(0.45, 0.45), Color(0.8, 0.65, 0.9))
	var visual_size := Vector2(80, 120)
	if sprite.texture != null:
		visual_size = sprite.texture.get_size() * sprite.scale.abs()
	sprite.position.y = -visual_size.y * 0.5
	add_collision_at(character, visual_size, sprite.position)
	add_child(character)

func build_visual(position: Vector2, asset_name: String, folder: String, solid: bool) -> void:
	var node: Node2D = StaticBody2D.new() if solid else Node2D.new()
	node.position = position
	add_child(node)
	add_sprite(node, folder, asset_name, Vector2(0.45, 0.45))
	if solid:
		add_collision(node, Vector2(100, 100))

func add_sprite(parent: Node, folder: String, asset_name: String, target_scale: Vector2, fallback := Color.WHITE) -> Sprite2D:
	var sprite := Sprite2D.new()
	sprite.texture = DataLoader.texture(folder, asset_name)
	if sprite.texture != null:
		sprite.scale = target_scale
	else:
		var image := Image.create(64, 96, false, Image.FORMAT_RGBA8)
		image.fill(fallback)
		sprite.texture = ImageTexture.create_from_image(image)
	sprite.position.y = -45
	parent.add_child(sprite)
	return sprite

func add_collision(parent: Node, size: Vector2) -> void:
	add_collision_at(parent, size, Vector2(0, -size.y * 0.5))

func add_collision_at(parent: Node, size: Vector2, center: Vector2) -> void:
	var shape := CollisionShape2D.new()
	var rectangle := RectangleShape2D.new()
	rectangle.size = size
	shape.shape = rectangle
	shape.position = center
	parent.add_child(shape)
