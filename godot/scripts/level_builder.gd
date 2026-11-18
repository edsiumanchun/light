class_name LevelBuilder
extends Node2D

const PLAYER_SCENE := preload("res://godot/scenes/player.tscn")
const CHARACTER_VISUAL_SCENE := preload("res://godot/scenes/character_visual.tscn")
const ENEMY_SCRIPT := preload("res://godot/scripts/enemy.gd")
const GATE_SCRIPT := preload("res://godot/scripts/gate.gd")
const PICKUP_SCRIPT := preload("res://godot/scripts/pickup.gd")
const EXIT_SCRIPT := preload("res://godot/scripts/level_exit.gd")
const STORY_SCRIPT := preload("res://godot/scripts/story_interaction.gd")
const STORY_EVENT_SCRIPT := preload("res://godot/scripts/story_event.gd")
const DOOR_SCRIPT := preload("res://godot/scripts/door.gd")
const DOOR_SWITCH_SCRIPT := preload("res://godot/scripts/door_switch.gd")

const FALLBACK_GRID_ROWS := 5
const FALLBACK_VIEW_SIZE := Vector2(1200, 675)
const GRID_ASSET_SCALE := 0.45
const TIMBER_DEPTH := Vector2(11, -16)
const METAL_GATE_COLORS := {
	"GateCopper": Color("#d87b58"),
	"GateSilver": Color("#d9e6f1"),
	"GateGold": Color("#ffdb72"),
}

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
var content_origin_y := 0.0
var group_count := 1
var grid_columns := 1
var grid_rows := FALLBACK_GRID_ROWS
var platform_half_thickness := 0.0
var camera_visible_rows := 5.0
var camera_follow_player_x := false
var camera_follow_player_y := false
var camera_horizontal_dead_zone := 55.0
var camera_vertical_dead_zone := 55.0
var background_mode := "auto"
var quiet_plaster := false
var timber_finish := false
var hide_plates := false
var invisible_plates: Array = []
var asset_scales := {}
var door_decoration := "Door"
var gate_decoration := ""
var horizontal_gate_decoration := ""
var switch_gate_decoration := ""
var switch_sprite := "TriggerLight"
var vertical_gate_min_width := 0.0
var door_destinations: Dictionary = {}
var gate_switches: Dictionary = {}
var camera_zoom_override := 0.0
var background_focus_y := 0.5
var background_lighting: Dictionary = {}
var background_prop_inset := 0.0
var fallback_player_position := Vector2.ZERO
var has_fallback_player_position := false
var door_player_position := Vector2.ZERO
var has_door_player_position := false
var teacher_exit_floor := -1
var teacher: StoryInteraction
var teacher_exit_door: CastleDoor
var father: StoryInteraction
var story_bed_position := Vector2.ZERO
var sleep_event := 0
var init_story := 0
var disable_npc_interaction := false
var event_requirements: Dictionary = {}
var event_unlock_requirements: Dictionary = {}
var event_lock_requirements: Dictionary = {}
var event_progress_requirements: Dictionary = {}
var once_events: Array = []
var rescue_rock: Node2D
var rescue_rock_offset := Vector2.ZERO
const RESCUE_ROCK_ID := "rescue_rock"
var object_id_prefix := ""
var event_cells: Array[Dictionary] = []
var event_interactions: Array[StoryInteraction] = []
var completed_events: Dictionary = {}

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
		if assets.is_empty():
			continue
		build_group_background(assets, group_index)
		for asset in assets:
			build_asset(asset, group_index)
	if teacher != null and teacher_exit_door != null:
		teacher.setup_exit(teacher_exit_door.position)
		StoryManager.story_cue.connect(play_room_story_cue)
	spawn_player_at_entrance()
	var rescue: Dictionary = floor_definition.get("layout", {}).get("rockRescue", {})
	if not rescue.is_empty():
		build_rock_rescue(rescue)
		if not StoryManager.story_cue.is_connected(play_room_story_cue):
			StoryManager.story_cue.connect(play_room_story_cue)
	if not event_cells.is_empty():
		StoryManager.completed.connect(on_event_story_completed)
		StoryManager.event_unlocked.connect(on_event_unlocked)
		GameState.location_progress_changed.connect(on_location_progress_changed)
		for actor in get_children():
			if actor is StoryInteraction and event_cells.any(func(cell): return actor.position.is_equal_approx(cell.position)):
				actor.interaction_enabled = false
				actor.input_pickable = false
				actor.remove_from_group("interactable")
		refresh_event_interactions()
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
	content_origin_y = maxf(world_size.y - content_height, 0.0) if str(layout.get("verticalAlign", "top")).to_lower() == "bottom" else 0.0
	camera_visible_rows = maxf(float(camera_config.get("visibleRows", grid_rows)), 1.0)
	camera_follow_player_x = bool(camera_config.get("followPlayerX", false))
	camera_follow_player_y = bool(camera_config.get("followPlayerY", false))
	camera_horizontal_dead_zone = maxf(float(camera_config.get("horizontalDeadZone", 55.0)), 0.0)
	camera_vertical_dead_zone = maxf(float(camera_config.get("verticalDeadZone", 55.0)), 0.0)
	background_mode = str(layout.get("backgroundMode", "auto")).to_lower()
	quiet_plaster = background_mode == "quiet_plaster"
	timber_finish = str(layout.get("plateFinish", "")) == "timber"
	teacher_exit_floor = int(layout.get("teacherExitDoorFloor", -1))
	sleep_event = int(layout.get("sleepEvent", 0))
	var init_story_value: Variant = layout.get("initStory", false)
	init_story = GameState.current_floor if init_story_value is bool and init_story_value else int(init_story_value)
	disable_npc_interaction = bool(layout.get("disableNpcInteraction", false))
	event_requirements = layout.get("eventRequires", {})
	event_lock_requirements = layout.get("eventRequiresOpened", {})
	event_progress_requirements = layout.get("eventProgress", {})
	once_events = layout.get("onceEvents", [])
	object_id_prefix = str(layout.get("objectIdPrefix", ""))
	event_unlock_requirements = layout.get("eventUnlocks", {}).duplicate()
	for event_number in layout.get("lockedEvents", []):
		var number := int(event_number)
		if not event_unlock_requirements.has(str(number)):
			event_unlock_requirements[str(number)] = number
	hide_plates = bool(layout.get("hidePlates", false))
	invisible_plates = layout.get("invisiblePlates", [])
	asset_scales = layout.get("assetScales", {})
	door_decoration = str(layout.get("doorDecoration", "Door"))
	gate_decoration = str(layout.get("gateDecoration", ""))
	horizontal_gate_decoration = str(layout.get("horizontalGateDecoration", ""))
	switch_gate_decoration = str(layout.get("switchGateDecoration", ""))
	switch_sprite = str(layout.get("switchSprite", "TriggerLight"))
	vertical_gate_min_width = float(layout.get("verticalGateMinWidth", 0.0))
	door_destinations = layout.get("doorDestinations", {})
	gate_switches = layout.get("gateSwitches", {})
	camera_zoom_override = float(layout.get("cameraZoom", 0.0))
	background_focus_y = clampf(float(layout.get("backgroundFocusY", 0.5)), 0.0, 1.0)
	background_lighting = layout.get("backgroundLighting", {})
	background_prop_inset = maxf(0.0, float(layout.get("backgroundPropInset", 0.0)))

func detect_grid_size(floor_definition: Dictionary) -> Vector2i:
	var detected_columns := 0
	var detected_rows := 0
	for group_index in range(3):
		var assets: Array = floor_definition.get("assets%d" % group_index, [])
		for asset in assets:
			var type := str(asset.get("name", ""))
			if type not in ["PlateH", "PlateV", "Object", "Backdrop", "Decoration", "Door", "Event", "Switch"]:
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
	if camera_zoom_override > 0.0:
		return Vector2.ONE * camera_zoom_override
	# Visible rows measure the vertical PlateV spans. Extend the camera by only
	# half a PlateH height so the lower boundary ends exactly at the viewport.
	var visible_height := camera_visible_rows * cell_size.y + platform_half_thickness
	var zoom_value := view_size.y / visible_height if visible_height > 0.0 else 1.0
	return Vector2.ONE * zoom_value

func clear_level() -> void:
	if StoryManager.completed.is_connected(on_event_story_completed):
		StoryManager.completed.disconnect(on_event_story_completed)
	if StoryManager.event_unlocked.is_connected(on_event_unlocked):
		StoryManager.event_unlocked.disconnect(on_event_unlocked)
	if GameState.location_progress_changed.is_connected(on_location_progress_changed):
		GameState.location_progress_changed.disconnect(on_location_progress_changed)
	if StoryManager.story_cue.is_connected(play_room_story_cue):
		StoryManager.story_cue.disconnect(play_room_story_cue)
	teacher = null
	teacher_exit_door = null
	rescue_rock = null
	father = null
	story_bed_position = Vector2.ZERO
	event_cells.clear()
	event_interactions.clear()
	completed_events.clear()
	for child in get_children():
		child.queue_free()
	player = null
	has_fallback_player_position = false
	has_door_player_position = false

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
						content_origin_y + row * cell_size.y
					)
					build_plate(position, signed_value, false, group_index, "gateH_%d_%d_%d" % [group_index, row, column])
				"PlateV":
					# Vertical entries occupy column boundary lines.
					var position := Vector2(
						content_origin_x + column * cell_size.x + group_index * group_width,
						content_origin_y + (row + 0.5) * cell_size.y
					)
					build_plate(position, signed_value, true, group_index, "gateV_%d_%d_%d" % [group_index, row, column])
				"Object":
					# Object roots use the bottom-center of the cell as their floor anchor.
					var boundary_y := content_origin_y + (row + 1.0) * cell_size.y
					var position := Vector2(
						content_origin_x + (column + 0.5) * cell_size.x + group_index * group_width,
						horizontal_plate_surface_y(boundary_y)
					)
					build_object(position, signed_value, object_id_prefix + "%d_%d_%d" % [group_index, row, column])
				"Backdrop":
					var position := Vector2(
						content_origin_x + (column + 0.5) * cell_size.x + group_index * group_width,
						content_origin_y + (row + 0.5) * cell_size.y
					)
					build_backdrop(position, signed_value)
				"Door", "Event", "Decoration", "Switch":
					var boundary_y := content_origin_y + (row + 1.0) * cell_size.y
					var position := Vector2(
						content_origin_x + (column + 0.5) * cell_size.x + group_index * group_width,
						horizontal_plate_surface_y(boundary_y)
					)
					if type == "Door":
						build_door(position, absi(signed_value))
					elif type == "Switch":
						build_door_switch(position, absi(signed_value))
					elif type == "Decoration":
						build_from_list(position, absi(signed_value) - 1, "decorations", "Decorations", "", "visual")
					elif signed_value < 0:
						build_story_event(position, signed_value)
					else:
						event_cells.append({"number": signed_value, "position": position})
						event_interactions.append(null)
						if signed_value == sleep_event:
							story_bed_position = position

func horizontal_plate_surface_y(boundary_y: float) -> float:
	# Outer plates are clamped inside the world. Ground objects against the
	# resulting top edge, not against the original grid boundary.
	var plate_center_y := clampf(
		boundary_y,
		content_origin_y + platform_half_thickness,
		content_origin_y + content_height - platform_half_thickness
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
	if background_mode == "plain_panels" or quiet_plaster:
		build_plain_panel_background(group_index)
		return
	var wall_names: Array = paths.get("walls", [])
	if background_mode != "image" and not wall_names.is_empty():
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
	var texture := DataLoader.texture("Background", str(names[background_index]))
	if texture == null:
		return
	var background := Sprite2D.new()
	background.name = "ImageBackground%d" % group_index
	background.texture = texture
	background.centered = true
	var background_left := content_origin_x + group_index * group_width
	background.position = Vector2(
		background_left + group_width * 0.5,
		content_origin_y + content_height * 0.5
	)
	# Crop the source without stretching it. A per-level vertical focus lets a
	# short room favor its wall/floor instead of centering on ceiling beams.
	var texture_size := texture.get_size()
	var target_aspect := group_width / content_height
	var source_aspect := texture_size.x / texture_size.y
	var source_rect := Rect2(Vector2.ZERO, texture_size)
	if target_aspect > source_aspect:
		var crop_height := texture_size.x / target_aspect
		var focus_center := texture_size.y * background_focus_y
		var crop_y := clampf(focus_center - crop_height * 0.5, 0.0, texture_size.y - crop_height)
		source_rect = Rect2(0.0, crop_y, texture_size.x, crop_height)
	else:
		var crop_width := texture_size.y * target_aspect
		source_rect = Rect2((texture_size.x - crop_width) * 0.5, 0.0, crop_width, texture_size.y)
	background.region_enabled = true
	background.region_rect = source_rect
	background.scale = Vector2(group_width / source_rect.size.x, content_height / source_rect.size.y)
	background.z_index = -100
	add_child(background)

func build_plain_panel_background(group_index: int) -> void:
	var background := ColorRect.new()
	background.name = "PlainPanelBackground%d" % group_index
	background.position = Vector2(content_origin_x + group_index * group_width, content_origin_y)
	background.size = Vector2(group_width, content_height)
	background.mouse_filter = Control.MOUSE_FILTER_IGNORE
	background.z_index = -100
	var material := ShaderMaterial.new()
	material.shader = preload("res://godot/shaders/room_panels.gdshader")
	material.set_shader_parameter("room_size", background.size)
	material.set_shader_parameter("panel_width", cell_size.x * 3.0)
	material.set_shader_parameter("quiet_plaster", quiet_plaster)
	for uniform_name in ["lantern_light", "window_center", "bed_feet", "pot_feet", "table_feet"]:
		var point: Array = background_lighting.get(uniform_name, [])
		if point.size() == 2:
			material.set_shader_parameter(uniform_name, Vector2(float(point[0]), float(point[1])))
	background.material = material
	add_child(background)

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
	var current_y := content_origin_y
	var content_bottom := content_origin_y + content_height
	var tile_index := 0
	while current_y < content_bottom:
		var texture := wall_textures[tile_index % wall_textures.size()]
		var tile_scale := group_width / texture.get_width()
		var tile_height := minf(texture.get_height() * tile_scale, content_bottom - current_y)
		var sprite := Sprite2D.new()
		sprite.texture = texture
		sprite.region_enabled = true
		sprite.region_rect = Rect2(0.0, 0.0, texture.get_width(), tile_height / tile_scale)
		sprite.scale = Vector2.ONE * tile_scale
		sprite.position = Vector2(
			content_origin_x + group_index * group_width + group_width * 0.5,
			current_y + tile_height * 0.5
		)
		layer.add_child(sprite)
		current_y += tile_height
		tile_index += 1

func build_plate(position: Vector2, signed_value: int, vertical: bool, group_index: int, gate_id: String) -> void:
	var names: Array = paths.get("plateV" if vertical else "plateH", [])
	var index := absi(signed_value) - 1
	if index < 0 or index >= names.size():
		return
	var asset_name := str(names[index])
	var has_gate := asset_name.begins_with("Gate")
	var switch_number := int(gate_switches.get(gate_id, 0))
	if has_gate and (GameState.is_destroyed(gate_id) or (
		switch_number > 0 and GameState.is_destroyed("switch_%d" % switch_number)
	)):
		return
	var body := StaticBody2D.new()
	body.position = position
	body.collision_layer = 1
	body.collision_mask = 2
	var horizontal_gate := not vertical and asset_name.begins_with("Gate")
	var gate_width_scale: Dictionary = config.get("gateWidthScale", {})
	var sprite := add_sprite(body, "PlateV" if vertical or horizontal_gate else "PlateH", asset_name)
	sprite.visible = not hide_plates and not invisible_plates.has(asset_name)
	# All imported 2D grid artwork uses the same Unity-style scale.
	var texture_size := sprite.texture.get_size()
	var plate_size := texture_size * sprite.scale.abs()
	if horizontal_gate:
		var hatch_width := minf(cell_size.x * float(gate_width_scale.get("horizontal", 1.0)), group_width)
		plate_size = Vector2(hatch_width, platform_half_thickness * 2.0)
		var hatch := DataLoader.texture("Decorations", horizontal_gate_decoration) if not horizontal_gate_decoration.is_empty() else null
		if hatch != null:
			sprite.texture = hatch
			var top_depth := -TIMBER_DEPTH.y if timber_finish else 0.0
			var hatch_art_size := plate_size + Vector2(0.0, top_depth)
			sprite.scale *= hatch_art_size / sprite_visible_rect(sprite).size
			sprite.position = -sprite_visible_rect(sprite).get_center() - Vector2(0.0, top_depth * 0.5)
		else:
			sprite.rotation = PI * 0.5
			sprite.scale = Vector2(plate_size.y, plate_size.x) / texture_size
	var is_gate := vertical and asset_name.begins_with("Gate")
	if is_gate:
		plate_size.x = maxf(vertical_gate_min_width, plate_size.x * float(gate_width_scale.get("vertical", 1.0)))
	if is_gate and switch_number > 0 and not switch_gate_decoration.is_empty():
		var switch_gate_texture := DataLoader.texture("Decorations", switch_gate_decoration)
		if switch_gate_texture != null:
			sprite.texture = switch_gate_texture
	elif not gate_decoration.is_empty() and vertical and METAL_GATE_COLORS.has(asset_name):
		var gate_texture := DataLoader.texture("Decorations", gate_decoration)
		if gate_texture != null:
			sprite.texture = gate_texture
			sprite.scale = plate_size / gate_texture.get_size()
			sprite.self_modulate = METAL_GATE_COLORS[asset_name]
	if (timber_finish or quiet_plaster) and asset_name == "Wood":
		var finish := ShaderMaterial.new()
		finish.shader = preload("res://godot/shaders/timber_plate.gdshader")
		finish.set_shader_parameter("vertical_post", vertical)
		sprite.material = finish
		if vertical:
			sprite.scale.y *= cell_size.y / plate_size.y
			if sprite.visible:
				add_timber_post_side(body, plate_size.x, cell_size.y)
		else:
			sprite.scale.x *= cell_size.x / plate_size.x
			if sprite.visible:
				add_timber_floor_surface(body, cell_size.x, plate_size.y)
	# Keep the complete outer boundary visible. Interior walls remain centered
	# on their JSON grid line.
	if vertical or horizontal_gate:
		var group_left := content_origin_x + group_index * group_width
		position.x = clampf(position.x, group_left + plate_size.x * 0.5, group_left + group_width - plate_size.x * 0.5)
	if vertical:
		if is_gate:
			var half_row := cell_size.y * 0.5
			var ceiling_bottom := horizontal_plate_surface_y(position.y - half_row) + platform_half_thickness * 2.0
			var floor_top := horizontal_plate_surface_y(position.y + half_row)
			plate_size.y = maxf(1.0, floor_top - ceiling_bottom)
			position.y = (ceiling_bottom + floor_top) * 0.5
			var visible_rect := sprite_visible_rect(sprite)
			sprite.scale *= plate_size / visible_rect.size
			sprite.position = -sprite_visible_rect(sprite).get_center()
	else:
		position.y = clampf(
			position.y,
			content_origin_y + plate_size.y * 0.5,
			content_origin_y + content_height - plate_size.y * 0.5
		)
	body.position = position
	var shape := CollisionShape2D.new()
	var rectangle := RectangleShape2D.new()
	rectangle.size = plate_size
	shape.shape = rectangle
	body.add_child(shape)
	if has_gate:
		body.set_script(GATE_SCRIPT)
		body.collision_layer = 5
		body.set("key_name", asset_name.trim_prefix("Gate"))
		body.set("persistent_id", gate_id)
		body.set("switch_controlled", switch_number > 0)
	elif asset_name.begins_with("GoDown"):
		body.set_script(EXIT_SCRIPT)
		body.collision_layer = 5
		body.set("direction", -1 if asset_name.contains("Revert") else 1)
	elif asset_name == "GoUp":
		body.set_script(EXIT_SCRIPT)
		body.collision_layer = 5
		body.set("direction", -1)
	add_child(body)

func add_timber_post_side(parent: Node2D, width: float, height: float) -> void:
	var top := Vector2(width * 0.5, -height * 0.5)
	var bottom := top + Vector2(0, height)
	var side := Polygon2D.new()
	side.name = "TimberPostSide"
	side.z_index = -1
	side.polygon = PackedVector2Array([top, bottom, bottom + TIMBER_DEPTH, top + TIMBER_DEPTH])
	side.vertex_colors = PackedColorArray([
		Color("#4e3023"), Color("#4e3023"), Color("#2c201b"), Color("#2c201b"),
	])
	parent.add_child(side)

func add_timber_floor_surface(parent: Node2D, width: float, thickness: float) -> void:
	var half_width := width * 0.5
	var front_left := Vector2(-half_width, -thickness * 0.5)
	var front_right := front_left + Vector2(width, 0)
	var surface := Polygon2D.new()
	surface.name = "TimberFloorSurface"
	surface.z_index = -1
	surface.polygon = PackedVector2Array([
		front_left, front_right, front_right + TIMBER_DEPTH, front_left + TIMBER_DEPTH,
	])
	surface.vertex_colors = PackedColorArray([
		Color("#86634b"), Color("#86634b"), Color("#514035"), Color("#514035"),
	])
	parent.add_child(surface)

	var seam := Line2D.new()
	seam.width = 1.0
	seam.default_color = Color("#624936")
	seam.points = PackedVector2Array([front_left + TIMBER_DEPTH * 0.55, front_right + TIMBER_DEPTH * 0.55])
	surface.add_child(seam)

func build_object(position: Vector2, signed_value: int, id: String) -> void:
	var index := absi(signed_value) - 1
	if index == 0:
		if not has_fallback_player_position:
			fallback_player_position = position
			has_fallback_player_position = true
		return
	# Story actors can reuse a cell that held a defeated enemy or a pickup.
	if index > 0 and index < 10:
		build_story_character(position, index)
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
	var sprite_name := asset_name if not DataLoader.sprite_path("Enemies", asset_name).is_empty() else "EnemyBone"
	var sprite := add_sprite(enemy, "Enemies", sprite_name)
	var visible_rect := sprite_visible_rect(sprite)
	var body_size := Vector2(80, 120)
	sprite.scale *= minf(body_size.x / visible_rect.size.x, body_size.y / visible_rect.size.y)
	visible_rect = sprite_visible_rect(sprite)
	sprite.position = Vector2(-visible_rect.get_center().x, -visible_rect.end.y)
	add_collision(enemy, body_size)
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
	sprite.position = Vector2(-visible_rect.get_center().x, -visible_rect.end.y - background_prop_inset)
	add_collision_at(pickup, visible_rect.size, Vector2(0, -visible_rect.size.y * 0.5))
	pickup.call("setup", asset_name, id)

func build_door(position: Vector2, target_floor: int) -> void:
	var destination: Dictionary = door_destinations.get(str(target_floor), {})
	var door := Area2D.new()
	door.name = "Door"
	door.set_script(DOOR_SCRIPT)
	door.position = position
	door.collision_layer = 4
	door.collision_mask = 0
	add_child(door)
	var sprite := add_sprite(door, "Decorations", str(destination.get("decoration", door_decoration)))
	var visible_rect := sprite_visible_rect(sprite)
	sprite.position = Vector2(-visible_rect.get_center().x, -visible_rect.end.y - background_prop_inset)
	sprite.z_index = -1
	# Detection only: Area2D never blocks the player's movement.
	add_collision_at(door, visible_rect.size, Vector2(0, -visible_rect.size.y * 0.5))
	door.call("setup", target_floor)
	var destination_stage := GameState.current_stage
	if destination.has("location"):
		destination_stage = DataLoader.stage_for_location(str(destination.location))
	var destination_floor := DataLoader.floor_number(destination_stage, destination.get("floor", target_floor))
	door.set("target_stage", destination_stage)
	door.set("target_floor", destination_floor)
	if destination_floor < 0:
		door.remove_from_group("interactable")
	door.set("required_event", int(destination.get("requiresEvent", 0)))
	door.set("unlock_event", int(destination.get("unlockEvent", 0)))
	door.set("required_location_progress", int(destination.get("requiresLocationProgress", 0)))
	door.set("key_name", str(destination.get("key", "")))
	door.set("persistent_id", "door_%d" % target_floor)
	door.set("required_gates", destination.get("requiresGates", []))
	if target_floor == teacher_exit_floor:
		teacher_exit_door = door as CastleDoor
	var arrival_from: Array = destination.get("arrivalFrom", []).map(func(floor_number): return DataLoader.floor_number(GameState.current_stage, floor_number))
	if destination_stage == GameState.current_stage and not has_door_player_position and (
		destination_floor == GameState.previous_floor or arrival_from.has(GameState.previous_floor)
	):
		door_player_position = position
		has_door_player_position = true

func build_door_switch(position: Vector2, switch_number: int) -> void:
	var id := "switch_%d" % switch_number
	if GameState.is_destroyed(id):
		return
	var trigger := DOOR_SWITCH_SCRIPT.new()
	trigger.position = position
	trigger.persistent_id = id
	trigger.activated.connect(_open_switch_gates.bind(switch_number))
	trigger.collision_layer = 4
	trigger.collision_mask = 0
	var sprite := add_sprite(trigger, "Triggers", switch_sprite)
	var bounds := sprite_visible_rect(sprite)
	sprite.scale *= minf(56.0 / bounds.size.x, 64.0 / bounds.size.y)
	bounds = sprite_visible_rect(sprite)
	sprite.position = Vector2(-bounds.get_center().x, -bounds.end.y - background_prop_inset)
	add_collision_at(trigger, bounds.size, Vector2(0.0, -bounds.size.y * 0.5))
	add_child(trigger)

func _open_switch_gates(switch_number: int) -> void:
	for child in get_children():
		if child is CastleGate and int(gate_switches.get(child.persistent_id, 0)) == switch_number:
			child.open()

func build_story_event(position: Vector2, story_floor: int) -> void:
	var event := Area2D.new()
	event.name = "StoryEvent%d" % story_floor
	event.set_script(STORY_EVENT_SCRIPT)
	event.position = position
	event.collision_layer = 0
	event.collision_mask = 2
	add_collision_at(
		event,
		Vector2(cell_size.x * 0.8, cell_size.y * 0.8),
		Vector2(0.0, -cell_size.y * 0.4)
	)
	add_child(event)
	event.call("setup", story_floor)

func spawn_player_at_entrance() -> void:
	if player != null:
		return
	if not has_door_player_position and not has_fallback_player_position:
		push_warning("Floor has no matching entrance door and no Object value 1 player spawn.")
		return
	player = PLAYER_SCENE.instantiate()
	player.floor_height = cell_size.y
	player.position = door_player_position if has_door_player_position else fallback_player_position
	add_child(player)

func build_story_character(position: Vector2, index: int) -> void:
	var names: Array = paths.get("characters", [])
	if index < 0 or index >= names.size():
		return
	var character_name := str(names[index])
	var character := STORY_SCRIPT.new() as StoryInteraction
	character.position = position
	var visual_size := Vector2(80, 120)
	add_character_visual(character)
	add_collision_at(character, visual_size, Vector2(0, -visual_size.y * 0.5))
	add_child(character)
	character.setup_pose(-1.0)
	if disable_npc_interaction or not FileAccess.file_exists(DataLoader.story_path(GameState.current_stage, str(GameState.current_floor))):
		character.set("interaction_enabled", false)
		character.input_pickable = false
		character.remove_from_group("interactable")
	if character_name == "StorySage" and teacher_exit_floor >= 0:
		teacher = character
	elif character_name == "StoryFather":
		father = character
		father.interaction_enabled = false
		father.hide()
		father.remove_from_group("interactable")
		father.collision_layer = 0
		father.input_pickable = false

func add_character_visual(parent: Node) -> Node:
	var visual := CHARACTER_VISUAL_SCENE.instantiate()
	parent.add_child(visual)
	return visual

func build_visual(position: Vector2, asset_name: String, folder: String, solid: bool) -> void:
	var node: Node2D = StaticBody2D.new() if solid else Node2D.new()
	add_child(node)
	var sprite := add_sprite(node, folder, asset_name)
	var visual_size := sprite.texture.get_size() * sprite.scale.abs()
	node.position = Vector2(position.x, position.y - visual_size.y * 0.5)
	if solid:
		add_collision_at(node, visual_size, Vector2.ZERO)
	else:
		sprite.position.y -= background_prop_inset
func on_event_story_completed(stage: int, floor: int, event_number: int) -> void:
	if stage != GameState.current_stage or floor != GameState.current_floor:
		return
	if event_number <= 0:
		return
	completed_events[event_number] = true
	refresh_event_interactions()

func on_event_unlocked(stage: int, floor: int, _event_number: int) -> void:
	if stage != GameState.current_stage or floor != GameState.current_floor:
		return
	refresh_event_interactions()

func on_location_progress_changed(location: String) -> void:
	if location == DataLoader.location_for_stage(GameState.current_stage):
		refresh_event_interactions()

func refresh_event_interactions() -> void:
	for index in range(event_cells.size()):
		var cell := event_cells[index]
		var number := int(cell.number)
		var interaction := event_interactions[index]
		if completed_events.has(number) or (once_events.any(func(event_number): return int(event_number) == number)
			and GameState.has_completed_story_event(GameState.current_stage, GameState.current_floor, number)):
			if is_instance_valid(interaction):
				interaction.queue_free()
			continue
		var required_event := int(event_requirements.get(str(number), 0))
		var unlock_event := int(event_unlock_requirements.get(str(number), 0))
		var location := DataLoader.location_for_stage(GameState.current_stage)
		if int(event_progress_requirements.get(str(number), number)) > maxi(1, GameState.progress_for_location(location)):
			continue
		var required_lock: Dictionary = event_lock_requirements.get(str(number), {})
		if not required_lock.is_empty() and not GameState.destroyed.has(GameState.object_id(
			GameState.current_stage, int(required_lock.floor), str(required_lock.id)
		)):
			continue
		if is_instance_valid(interaction) or not GameState.meets_story_requirements(
			GameState.current_stage, GameState.current_floor, required_event, unlock_event
		):
			continue
		interaction = STORY_SCRIPT.new()
		interaction.name = "StoryEvent%d" % number
		interaction.event_number = number
		interaction.require_proximity = true
		interaction.position = cell.position
		var size := Vector2(cell_size.x, maxf(cell_size.y, 100.0))
		add_collision_at(interaction, size, Vector2(0, -size.y * 0.5))
		add_child(interaction)
		event_interactions[index] = interaction

func play_room_story_cue(cue: String) -> void:
	var action: Tween
	match cue:
		"teacherleave":
			action = teacher.play_exit_cue(cue)
		"teacherback":
			if story_bed_position != Vector2.ZERO:
				action = teacher.walk_to(story_bed_position + Vector2(210.0, 0.0), 0.65)
			else:
				action = teacher.play_exit_cue(cue)
		"playersleep":
			if is_instance_valid(player) and story_bed_position != Vector2.ZERO:
				action = player.walk_to_story_bed(story_bed_position)
		"fatherarrive":
			if is_instance_valid(father) and story_bed_position != Vector2.ZERO:
				action = father.walk_to(story_bed_position + Vector2(265.0, 0.0))
		"playerwake":
			if is_instance_valid(player) and story_bed_position != Vector2.ZERO:
				action = player.get_up_from_story_bed(story_bed_position + Vector2(200.0, 0.0))
		"moverock":
			if is_instance_valid(rescue_rock) and not GameState.is_destroyed(RESCUE_ROCK_ID):
				if is_instance_valid(player):
					player.facing = 1.0 if rescue_rock.position.x >= player.position.x else -1.0
					player.cast_story_spell()
				action = create_tween()
				action.tween_interval(0.42)
				action.tween_property(rescue_rock, "position", rescue_rock.position + rescue_rock_offset, 1.0).set_trans(Tween.TRANS_CUBIC)
				action.tween_callback(func(): GameState.mark_destroyed(RESCUE_ROCK_ID))
	if action != null:
		await action.finished
	StoryManager.finish_story_cue()

func build_rock_rescue(config: Dictionary) -> void:
	var cell: Array = config.cell
	var anchor := Vector2(
		content_origin_x + (float(cell[0]) + 0.5) * cell_size.x,
		horizontal_plate_surface_y(content_origin_y + (float(cell[1]) + 1.0) * cell_size.y)
	)
	var grandmother := Node2D.new()
	grandmother.position = anchor + Vector2(-18.0, 0.0)
	add_child(grandmother)
	var figure := add_sprite(grandmother, "Decorations", "Frontier/Grandmother")
	var figure_rect := sprite_visible_rect(figure)
	figure.scale *= 86.0 / figure_rect.size.y
	figure_rect = sprite_visible_rect(figure)
	figure.position = Vector2(-figure_rect.get_center().x, -figure_rect.end.y)

	rescue_rock_offset = Vector2(cell_size.x * float(config.get("slideCells", 1.0)), 0.0)
	rescue_rock = Node2D.new()
	rescue_rock.name = "RescueRock"
	rescue_rock.position = anchor + Vector2(27.0, 0.0)
	rescue_rock.z_index = 1
	if GameState.is_destroyed(RESCUE_ROCK_ID):
		rescue_rock.position += rescue_rock_offset
	add_child(rescue_rock)
	var rock := add_sprite(rescue_rock, "Decorations", "Frontier/CaveRocks")
	var rock_rect := sprite_visible_rect(rock)
	rock.scale *= 156.0 / rock_rect.size.x
	rock_rect = sprite_visible_rect(rock)
	rock.position = Vector2(-rock_rect.get_center().x, -rock_rect.end.y)

func build_backdrop(position: Vector2, signed_value: int) -> void:
	var names: Array = paths.get("decorations", [])
	var index := absi(signed_value) - 1
	if index < 0 or index >= names.size():
		return
	var node := Node2D.new()
	node.position = position
	node.z_index = -20
	add_child(node)
	add_sprite(node, "Decorations", str(names[index]))

func add_sprite(parent: Node, folder: String, asset_name: String, fallback := Color.WHITE) -> Sprite2D:
	var sprite := Sprite2D.new()
	sprite.texture = DataLoader.texture(folder, asset_name)
	if sprite.texture == null:
		var image := Image.create(64, 96, false, Image.FORMAT_RGBA8)
		image.fill(fallback)
		sprite.texture = ImageTexture.create_from_image(image)
	var scale_value := float(asset_scales.get("%s/%s" % [folder, asset_name], GRID_ASSET_SCALE))
	sprite.scale = Vector2.ONE * scale_value
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
