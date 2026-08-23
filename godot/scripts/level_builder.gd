class_name LevelBuilder
extends Node2D

const PLAYER_SCENE := preload("res://godot/scenes/player.tscn")
const CHARACTER_VISUAL_SCENE := preload("res://godot/scenes/character_visual.tscn")
const ENEMY_SCRIPT := preload("res://godot/scripts/enemy.gd")
const GATE_SCRIPT := preload("res://godot/scripts/gate.gd")
const PICKUP_SCRIPT := preload("res://godot/scripts/pickup.gd")
const EXIT_SCRIPT := preload("res://godot/scripts/level_exit.gd")
const STORY_SCRIPT := preload("res://godot/scripts/story_interaction.gd")
const DOOR_SCRIPT := preload("res://godot/scripts/door.gd")

const FALLBACK_GRID_ROWS := 5
const FALLBACK_VIEW_SIZE := Vector2(1200, 675)
const GRID_ASSET_SCALE := 0.45

var config := {}
var paths := {}
var enemy_definitions := {}
var player: CastlePlayer
var view_size := FALLBACK_VIEW_SIZE
var cell_size := Vector2(135, 135)
var world_size := FALLBACK_VIEW_SIZE
var group_width := FALLBACK_VIEW_SIZE.x
var content_height := FALLBACK_VIEW_SIZE.y
var content_origin_x := 0.0
var group_count := 1
var grid_columns := 1
var grid_rows := FALLBACK_GRID_ROWS
var platform_half_thickness := 0.0
var camera_visible_rows := 5.0
var camera_follow_player_x := false
var camera_follow_player_y := false
var camera_horizontal_dead_zone := 55.0
var camera_vertical_dead_zone := 55.0

func build(stage: int, floor: int) -> CastlePlayer:
	clear_level()
	config = DataLoader.stage_config(stage)
	paths = config.get("paths", {})
	index_enemy_data(DataLoader.enemy_data(stage))
	var floor_definition := DataLoader.floor_data(stage, floor)
	configure_grid(
		floor_definition,
		floor_definition.get("layout", {}),
		floor_definition.get("camera", {})
	)
	for group_index in range(3):
		var assets: Array = floor_definition.get("assets%d" % group_index, [])
		build_group_background(assets, group_index)
		for asset in assets:
			build_asset(asset, group_index)
	build_background_margin_masks()
	return player

func configure_grid(floor_definition: Dictionary, layout: Dictionary, camera_config: Dictionary) -> void:
	var viewport_size := get_viewport_rect().size if is_inside_tree() else view_size
	view_size = viewport_size if viewport_size.x > 0.0 and viewport_size.y > 0.0 else FALLBACK_VIEW_SIZE
	var detected_size := detect_grid_size(floor_definition)
	# The grids are authoritative. Layout values remain useful as fallbacks for
	# old or empty floors, but must not add blank background outside the plates.
	grid_columns = detected_size.x if detected_size.x > 0 else maxi(int(layout.get("columns", 1)), 1)
	grid_rows = detected_size.y if detected_size.y > 0 else maxi(int(layout.get("rows", FALLBACK_GRID_ROWS)), 1)
	var row_height := float(layout.get("cellHeight", view_size.y / grid_rows))
	var column_width := row_height
	platform_half_thickness = 0.0
	var horizontal_names: Array = paths.get("plateH", [])
	if not horizontal_names.is_empty():
		var reference_texture := DataLoader.texture("PlateH", str(horizontal_names[0]))
		if reference_texture != null:
			column_width = reference_texture.get_width() * GRID_ASSET_SCALE
			platform_half_thickness = reference_texture.get_height() * GRID_ASSET_SCALE * 0.5
	column_width = float(layout.get("cellWidth", column_width))
	cell_size = Vector2(column_width, row_height)
	group_count = 1
	for group_index in range(3):
		if not floor_definition.get("assets%d" % group_index, []).is_empty():
			group_count = group_index + 1
	group_width = cell_size.x * grid_columns
	content_height = cell_size.y * grid_rows
	var total_content_width := group_width * group_count
	world_size = Vector2(maxf(view_size.x, total_content_width), maxf(view_size.y, content_height))
	content_origin_x = maxf((world_size.x - total_content_width) * 0.5, 0.0)
	camera_visible_rows = maxf(float(camera_config.get("visibleRows", grid_rows)), 1.0)
	camera_follow_player_x = bool(camera_config.get("followPlayerX", false))
	camera_follow_player_y = bool(camera_config.get("followPlayerY", false))
	camera_horizontal_dead_zone = maxf(float(camera_config.get("horizontalDeadZone", 55.0)), 0.0)
	camera_vertical_dead_zone = maxf(float(camera_config.get("verticalDeadZone", 55.0)), 0.0)

func detect_grid_size(floor_definition: Dictionary) -> Vector2i:
	var detected_columns := 0
	var detected_rows := 0
	for group_index in range(3):
		var assets: Array = floor_definition.get("assets%d" % group_index, [])
		for asset in assets:
			var type := str(asset.get("name", ""))
			if type not in ["PlateH", "PlateV", "Object", "Door"]:
				continue
			var grid := asset_grid(asset)
			var row_count: int = grid.size() - 1 if type == "PlateH" else grid.size()
			detected_rows = maxi(detected_rows, row_count)
			for cells in grid:
				if not cells is Array:
					continue
				var column_count: int = cells.size() - 1 if type == "PlateV" else cells.size()
				detected_columns = maxi(detected_columns, column_count)
	return Vector2i(detected_columns, detected_rows)

func gameplay_zoom() -> Vector2:
	# Visible rows measure the vertical PlateV spans. Extend the camera by only
	# half a PlateH height so the lower boundary ends exactly at the viewport.
	var visible_height := camera_visible_rows * cell_size.y + platform_half_thickness
	var zoom_value := view_size.y / visible_height if visible_height > 0.0 else 1.0
	return Vector2.ONE * zoom_value

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
		return
	if type == "Audio":
		play_floor_audio(asset)
		return
	if type in ["Exit", "Exit2", "Teleport", "TeleportExit", "WallSkip", "Effect", "ShowStory", "GoUp"]:
		return
	var grid := asset_grid(asset)
	for row in range(grid.size()):
		var cells: Array = grid[row]
		for column in range(cells.size()):
			var signed_value := int(cells[column])
			if signed_value == 0:
				continue
			match type:
				"PlateH":
					# Horizontal entries occupy row boundary lines.
					var position := Vector2(
						content_origin_x + (column + 0.5) * cell_size.x + group_index * group_width,
						row * cell_size.y
					)
					build_plate(position, signed_value, false, group_index)
				"PlateV":
					# Vertical entries occupy column boundary lines.
					var position := Vector2(
						content_origin_x + column * cell_size.x + group_index * group_width,
						(row + 0.5) * cell_size.y
					)
					build_plate(position, signed_value, true, group_index)
				"Object":
					# Object roots use the bottom-center of the cell as their floor anchor.
					var boundary_y := (row + 1.0) * cell_size.y
					var position := Vector2(
						content_origin_x + (column + 0.5) * cell_size.x + group_index * group_width,
						horizontal_plate_surface_y(boundary_y)
					)
					build_object(position, signed_value, "%d_%d_%d" % [group_index, row, column])
				"Door":
					var boundary_y := (row + 1.0) * cell_size.y
					var position := Vector2(
						content_origin_x + (column + 0.5) * cell_size.x + group_index * group_width,
						horizontal_plate_surface_y(boundary_y)
					)
					build_door(position, absi(signed_value))

func horizontal_plate_surface_y(boundary_y: float) -> float:
	# Outer plates are clamped inside the world. Ground objects against the
	# resulting top edge, not against the original grid boundary.
	var plate_center_y := clampf(
		boundary_y,
		platform_half_thickness,
		content_height - platform_half_thickness
	)
	return plate_center_y - platform_half_thickness

func asset_grid(asset: Dictionary) -> Array:
	var nested_grid = asset.get("grid", [])
	if nested_grid is Array and not nested_grid.is_empty():
		return nested_grid
	var legacy_grid: Array = []
	var row := 0
	while asset.has("gridArray%d" % row):
		legacy_grid.append(asset["gridArray%d" % row])
		row += 1
	return legacy_grid

func play_floor_audio(asset: Dictionary) -> void:
	var indexes: Array = asset.get("gridArray0", [])
	var audio_names: Array = paths.get("audios", [])
	if indexes.is_empty():
		return
	var index := int(indexes[0])
	if index < 0 or index >= audio_names.size():
		return
	MusicManager.play_track(str(audio_names[index]))

func build_group_background(assets: Array, group_index: int) -> void:
	var wall_names: Array = paths.get("walls", [])
	if not wall_names.is_empty():
		build_tiled_wall_background(wall_names, group_index)
		return
	var background_index := 0
	for asset in assets:
		if str(asset.get("name", "")) != "Background":
			continue
		var indexes: Array = asset.get("gridArray0", [0])
		if not indexes.is_empty():
			background_index = int(indexes[0])
		break
	var names: Array = paths.get("backgrounds", [])
	if background_index < 0 or background_index >= names.size():
		return
	var sprite := Sprite2D.new()
	sprite.texture = DataLoader.texture("Background", str(names[background_index]))
	if sprite.texture == null:
		return
	sprite.centered = true
	sprite.position = Vector2(content_origin_x + group_index * group_width + group_width * 0.5, content_height * 0.5)
	sprite.scale = Vector2(group_width, content_height) / sprite.texture.get_size()
	sprite.z_index = -100
	add_child(sprite)

func build_tiled_wall_background(wall_names: Array, group_index: int) -> void:
	var wall_textures: Array[Texture2D] = []
	for wall_name in wall_names:
		var texture := DataLoader.texture("Walls", str(wall_name))
		if texture != null and texture.get_width() > 0:
			wall_textures.append(texture)
	if wall_textures.is_empty():
		return
	var layer := Node2D.new()
	layer.name = "WallBackground%d" % group_index
	layer.z_index = -100
	add_child(layer)
	var current_y := 0.0
	var tile_index := 0
	while current_y < content_height:
		var texture := wall_textures[tile_index % wall_textures.size()]
		var tile_scale := group_width / texture.get_width()
		var tile_height := texture.get_height() * tile_scale
		var sprite := Sprite2D.new()
		sprite.texture = texture
		sprite.scale = Vector2.ONE * tile_scale
		sprite.position = Vector2(
			content_origin_x + group_index * group_width + group_width * 0.5,
			current_y + tile_height * 0.5
		)
		layer.add_child(sprite)
		current_y += tile_height
		tile_index += 1

func build_background_margin_masks() -> void:
	var content_right := content_origin_x + group_width * group_count
	add_background_margin_mask("LeftBackgroundMask", 0.0, content_origin_x)
	add_background_margin_mask("RightBackgroundMask", content_right, world_size.x)

func add_background_margin_mask(node_name: String, left: float, right: float) -> void:
	if right <= left:
		return
	var mask := Polygon2D.new()
	mask.name = node_name
	mask.polygon = PackedVector2Array([
		Vector2(left, 0.0),
		Vector2(right, 0.0),
		Vector2(right, world_size.y),
		Vector2(left, world_size.y),
	])
	mask.color = Color.BLACK
	# Cover background overflow while remaining behind plates and characters.
	mask.z_index = -90
	add_child(mask)

func build_plate(position: Vector2, signed_value: int, vertical: bool, group_index: int) -> void:
	var names: Array = paths.get("plateV" if vertical else "plateH", [])
	var index := absi(signed_value) - 1
	if index < 0 or index >= names.size():
		return
	var asset_name := str(names[index])
	var body := StaticBody2D.new()
	body.position = position
	body.collision_layer = 1
	body.collision_mask = 2
	var sprite := add_sprite(body, "PlateV" if vertical else "PlateH", asset_name)
	# All imported 2D grid artwork uses the same Unity-style scale.
	var texture_size := sprite.texture.get_size()
	var plate_size := texture_size * GRID_ASSET_SCALE
	# Keep the complete outer boundary visible. Interior walls remain centered
	# on their JSON grid line.
	if vertical:
		var group_left := content_origin_x + group_index * group_width
		position.x = clampf(position.x, group_left + plate_size.x * 0.5, group_left + group_width - plate_size.x * 0.5)
	else:
		position.y = clampf(position.y, plate_size.y * 0.5, content_height - plate_size.y * 0.5)
	body.position = position
	sprite.position = Vector2.ZERO
	var shape := CollisionShape2D.new()
	var rectangle := RectangleShape2D.new()
	rectangle.size = plate_size
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
	add_character_visual(enemy)
	add_collision(enemy, Vector2(80, 120))
	enemy.call("setup", enemy_definitions.get(asset_name, {"name": asset_name, "hp": 10, "damage": 1}), id)

func build_pickup(position: Vector2, asset_name: String, folder: String, id: String) -> void:
	var pickup := Area2D.new()
	pickup.set_script(PICKUP_SCRIPT)
	pickup.position = position
	pickup.collision_layer = 4
	pickup.collision_mask = 2
	add_child(pickup)
	var sprite := add_sprite(pickup, folder, asset_name, Color(0.3, 0.9, 0.5))
	var visible_rect := sprite_visible_rect(sprite)
	sprite.position = Vector2(-visible_rect.get_center().x, -visible_rect.end.y)
	add_collision_at(pickup, visible_rect.size, Vector2(0, -visible_rect.size.y * 0.5))
	pickup.call("setup", asset_name, id)

func build_door(position: Vector2, target_floor: int) -> void:
	var door := Area2D.new()
	door.name = "Door"
	door.set_script(DOOR_SCRIPT)
	door.position = position
	door.collision_layer = 4
	door.collision_mask = 0
	add_child(door)
	var sprite := add_sprite(door, "Decorations", "Door")
	var visible_rect := sprite_visible_rect(sprite)
	sprite.position = Vector2(-visible_rect.get_center().x, -visible_rect.end.y)
	sprite.z_index = -1
	# Detection only: Area2D never blocks the player's movement.
	add_collision_at(door, visible_rect.size, Vector2(0, -visible_rect.size.y * 0.5))
	door.call("setup", target_floor)

func build_story_character(position: Vector2, index: int, _id: String) -> void:
	var names: Array = paths.get("characters", [])
	if index < 0 or index >= names.size():
		return
	var character := StaticBody2D.new()
	character.set_script(STORY_SCRIPT)
	character.position = position
	character.collision_layer = 5
	character.collision_mask = 2
	var visual_size := Vector2(80, 120)
	add_character_visual(character)
	add_collision_at(character, visual_size, Vector2(0, -visual_size.y * 0.5))
	add_child(character)

func add_character_visual(parent: Node) -> void:
	parent.add_child(CHARACTER_VISUAL_SCENE.instantiate())

func build_visual(position: Vector2, asset_name: String, folder: String, solid: bool) -> void:
	var node: Node2D = StaticBody2D.new() if solid else Node2D.new()
	add_child(node)
	var sprite := add_sprite(node, folder, asset_name)
	var visual_size := sprite.texture.get_size() * GRID_ASSET_SCALE
	node.position = Vector2(position.x, position.y - visual_size.y * 0.5)
	if solid:
		add_collision_at(node, visual_size, Vector2.ZERO)

func add_sprite(parent: Node, folder: String, asset_name: String, fallback := Color.WHITE) -> Sprite2D:
	var sprite := Sprite2D.new()
	sprite.texture = DataLoader.texture(folder, asset_name)
	if sprite.texture == null:
		var image := Image.create(64, 96, false, Image.FORMAT_RGBA8)
		image.fill(fallback)
		sprite.texture = ImageTexture.create_from_image(image)
	sprite.scale = Vector2.ONE * GRID_ASSET_SCALE
	parent.add_child(sprite)
	return sprite

func sprite_visible_rect(sprite: Sprite2D) -> Rect2:
	var texture_size := sprite.texture.get_size()
	var used_rect := Rect2(Vector2.ZERO, texture_size)
	var image := sprite.texture.get_image()
	if image != null:
		var image_used_rect := image.get_used_rect()
		if image_used_rect.has_area():
			used_rect = Rect2(Vector2(image_used_rect.position), Vector2(image_used_rect.size))
	return Rect2(
		(used_rect.position - texture_size * 0.5) * sprite.scale,
		used_rect.size * sprite.scale
	)

func add_collision(parent: Node, size: Vector2) -> void:
	add_collision_at(parent, size, Vector2(0, -size.y * 0.5))

func add_collision_at(parent: Node, size: Vector2, center: Vector2) -> void:
	var shape := CollisionShape2D.new()
	var rectangle := RectangleShape2D.new()
	rectangle.size = size
	shape.shape = rectangle
	shape.position = center
	parent.add_child(shape)
