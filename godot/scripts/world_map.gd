extends Control

const BOARD_SIZE := Vector2(1060, 548)
const INK := Color("#f1deb6")
const MUTED := Color("#c2ac85")
const GOLD := Color("#c39a55")
const TERRAIN_RECT := Rect2(25, 101, 699, 372)
const TERRAIN_BOUNDS := Rect2(94.19, 39.935, 0.82, 0.345)
const TERRAIN_PATH := "res://Resources/Sprites/UI/DunhuangRelief.png"

var board: Control
var locations: Array = []
var travel_buttons: Dictionary = {}
var selected_location: Label

func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS
	mouse_filter = Control.MOUSE_FILTER_STOP
	set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	var config := DataLoader.load_json(DataLoader.JSON_ROOT + "/Game/WorldMap.json")
	locations = config.get("locations", [])
	build_map(config)
	resized.connect(fit_board)
	fit_board()
	hide()

func build_map(config: Dictionary) -> void:
	var backdrop := ColorRect.new()
	backdrop.color = Color(0.07, 0.045, 0.035, 0.94)
	add_child(backdrop)
	backdrop.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	board = Control.new()
	board.size = BOARD_SIZE
	add_child(board)
	var font := SystemFont.new()
	font.font_names = PackedStringArray(["PingFang TC", "Noto Sans CJK TC", "Microsoft JhengHei", "sans-serif"])
	var map_theme := Theme.new()
	map_theme.default_font = font
	board.theme = map_theme

	add_panel(Rect2(Vector2.ZERO, BOARD_SIZE), Color("#31251e"), GOLD)
	add_panel(Rect2(24, 100, 1012, 374), Color("#413226"), Color("#98754c"))
	var terrain := TextureRect.new()
	terrain.texture = load(TERRAIN_PATH) as Texture2D
	terrain.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
	terrain.mouse_filter = Control.MOUSE_FILTER_IGNORE
	place(terrain, TERRAIN_RECT)
	add_panel(Rect2(741, 126, 271, 318), Color("#2f2620"), Color("#98754c"))
	var illustration := Node2D.new()
	var grotto: Dictionary = config.get("grottoAnchor", {})
	illustration.draw.connect(func(): draw_routes(illustration, grotto))
	board.add_child(illustration)

	add_caption("敦煌行旅圖", Rect2(34, 20, 410, 43), 34, INK)
	add_caption("敦煌地形與故事路線", Rect2(35, 66, 450, 24), 17, MUTED)
	add_panel(Rect2(40, 241, 140, 35), Color("#392b22"), GOLD)
	add_caption("往 " + str(config.get("west", "")), Rect2(40, 241, 140, 35), 17, INK, HORIZONTAL_ALIGNMENT_CENTER)
	add_caption("↑ 北", Rect2(650, 119, 56, 27), 18, INK, HORIZONTAL_ALIGNMENT_CENTER)
	add_panel(Rect2(39, 433, 349, 26), Color("#392b22"), Color("#98754c"))
	add_caption("◇ 故事地點約略位置    地形：Mapzen / USGS", Rect2(46, 433, 337, 26), 13, INK)
	add_caption("敦煌石窟一帶", Rect2(757, 145, 241, 30), 22, GOLD)
	for index in range(config.get("grottoSites", []).size()):
		var site: String = config.grottoSites[index]
		add_caption("◇  " + site, Rect2(752, 195 + index * 51, 222, 32), 19, INK)
		if index < config.grottoSites.size() - 1:
			add_caption("↓", Rect2(756, 224 + index * 51, 20, 19), 15, GOLD)

	for entry in locations:
		var bounds := landmark_label_bounds(entry)
		if entry.has("floor"):
			add_travel_button(entry, bounds)
		else:
			add_panel(bounds, Color("#46362b"), Color("#91744f"))
			var label := add_caption(str(entry.get("shortName", entry.name)), bounds, 18, INK, HORIZONTAL_ALIGNMENT_CENTER)
			label.mouse_filter = Control.MOUSE_FILTER_PASS
			label.tooltip_text = str(entry.name)
	if not grotto.is_empty():
		var bounds := landmark_label_bounds(grotto)
		add_panel(bounds, Color("#46362b"), Color("#91744f"))
		add_caption(str(grotto.name), bounds, 18, INK, HORIZONTAL_ALIGNMENT_CENTER)

	selected_location = add_caption("", Rect2(35, 490, 770, 40), 18, MUTED)
	var close_button := add_button("關閉  /  Esc", Rect2(871, 25, 142, 38))
	close_button.pressed.connect(close_map)

func draw_routes(canvas: Node2D, grotto: Dictionary) -> void:
	var route := Color("#f5d088")
	for index in range(locations.size() - 1):
		var start := map_point(locations[index].coordinates)
		var end := map_point(locations[index + 1].coordinates)
		canvas.draw_dashed_line(start, end, Color("#48311f"), 4.0, 8.0, true, true)
		canvas.draw_dashed_line(start, end, route, 1.5, 8.0, true, true)
	draw_arrow(canvas, Vector2(176, 289), Vector2(50, 289), route)
	for location in locations:
		draw_landmark(canvas, location)
	if not grotto.is_empty():
		draw_landmark(canvas, grotto)
		var grotto_pin := map_point(grotto.coordinates)
		canvas.draw_dashed_line(grotto_pin + Vector2(10, 0), Vector2(741, grotto_pin.y), route, 1.2, 6.0, true, true)
	# Longitudes are linear in this Mercator crop. Ten kilometers near 40°N
	# spans about 0.117 degrees of longitude.
	var scale_width := TERRAIN_RECT.size.x * 0.117 / TERRAIN_BOUNDS.size.x
	var scale_start := Vector2(575, 444)
	canvas.draw_line(scale_start, scale_start + Vector2(scale_width, 0), INK, 2.0, true)
	add_scale_ticks(canvas, scale_start, scale_width)

func add_scale_ticks(canvas: Node2D, start: Vector2, width: float) -> void:
	for x in [0.0, width]:
		var tick := start + Vector2(x, 0)
		canvas.draw_line(tick - Vector2(0, 4), tick + Vector2(0, 4), INK, 2.0, true)
	canvas.draw_string(board.get_theme_default_font(), start - Vector2(0, 10), "10 km", HORIZONTAL_ALIGNMENT_LEFT, -1, 14, INK)

func draw_landmark(canvas: Node2D, entry: Dictionary) -> void:
	var pin := map_point(entry.coordinates)
	var label_center := landmark_label_bounds(entry).get_center()
	canvas.draw_line(pin, label_center, INK, 1.0, true)
	if entry.get("approximate", false):
		canvas.draw_colored_polygon(PackedVector2Array([
			pin + Vector2(0, -7), pin + Vector2(7, 0),
			pin + Vector2(0, 7), pin + Vector2(-7, 0)
		]), INK)
		canvas.draw_circle(pin, 2.4, Color("#392b22"), true, -1.0, true)
	else:
		canvas.draw_circle(pin, 6.0, INK, true, -1.0, true)
		canvas.draw_circle(pin, 3.0, Color("#392b22"), true, -1.0, true)

func landmark_label_bounds(entry: Dictionary) -> Rect2:
	var offset: Array = entry.labelOffset
	var center := map_point(entry.coordinates) + Vector2(float(offset[0]), float(offset[1]))
	var width := 70.0 if entry.has("shortName") else 146.0
	return Rect2(center - Vector2(width * 0.5, 17), Vector2(width, 34))

func map_point(coordinates: Array) -> Vector2:
	var west_fraction := (float(coordinates[0]) - TERRAIN_BOUNDS.position.x) / TERRAIN_BOUNDS.size.x
	var north := mercator_y(TERRAIN_BOUNDS.end.y)
	var south := mercator_y(TERRAIN_BOUNDS.position.y)
	var north_fraction := inverse_lerp(north, south, mercator_y(float(coordinates[1])))
	return TERRAIN_RECT.position + TERRAIN_RECT.size * Vector2(west_fraction, north_fraction)

func mercator_y(latitude: float) -> float:
	return log(tan(PI * 0.25 + deg_to_rad(latitude) * 0.5))

func draw_arrow(canvas: Node2D, start: Vector2, end: Vector2, color: Color) -> void:
	var direction := start.direction_to(end)
	canvas.draw_line(start, end, color, 2.0, true)
	canvas.draw_line(end, end - direction.rotated(0.55) * 9.0, color, 2.0, true)
	canvas.draw_line(end, end - direction.rotated(-0.55) * 9.0, color, 2.0, true)

func add_caption(content: String, bounds: Rect2, font_size: int, color: Color, align := HORIZONTAL_ALIGNMENT_LEFT) -> Label:
	var caption := Label.new()
	caption.text = content
	caption.horizontal_alignment = align
	caption.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	caption.add_theme_font_size_override("font_size", font_size)
	caption.add_theme_color_override("font_color", color)
	place(caption, bounds)
	return caption

func panel_style(fill: Color, border: Color) -> StyleBoxFlat:
	var style := StyleBoxFlat.new()
	style.bg_color = fill
	style.border_color = border
	style.set_border_width_all(1)
	style.set_corner_radius_all(5)
	return style

func add_panel(bounds: Rect2, fill: Color, border: Color) -> void:
	var panel := Panel.new()
	panel.mouse_filter = Control.MOUSE_FILTER_IGNORE
	panel.add_theme_stylebox_override("panel", panel_style(fill, border))
	place(panel, bounds)

func add_button(title: String, bounds: Rect2) -> Button:
	var button := Button.new()
	button.text = title
	button.add_theme_font_size_override("font_size", 19)
	button.add_theme_color_override("font_color", INK)
	button.add_theme_stylebox_override("normal", panel_style(Color("#57402b"), GOLD))
	button.add_theme_stylebox_override("hover", panel_style(Color("#715333"), INK))
	button.add_theme_stylebox_override("pressed", panel_style(Color("#453120"), GOLD))
	button.add_theme_stylebox_override("disabled", panel_style(Color("#46362b"), Color("#91744f")))
	place(button, bounds)
	return button

func place(control: Control, bounds: Rect2) -> void:
	control.position = bounds.position
	control.size = bounds.size
	board.add_child(control)

func add_travel_button(entry: Dictionary, bounds: Rect2) -> void:
	if not entry.has("floor"):
		return
	var button := add_button(str(entry.get("shortName", entry.name)), bounds)
	button.tooltip_text = str(entry.name)
	travel_buttons[button] = entry
	button.pressed.connect(func(): travel_to(entry))

func travel_to(entry: Dictionary) -> void:
	var floor := int(entry.floor)
	if not can_travel(entry):
		return
	close_map()
	GameState.travel_to(DataLoader.stage_for_location(str(entry.location)), floor, 4, -1, int(entry.get("arrivalFrom", -1)))

func can_travel(entry: Dictionary) -> bool:
	var location := str(entry.get("location", ""))
	var stage := DataLoader.stage_for_location(location)
	var floor := int(entry.floor)
	var discovery := str(entry.get("discovery", ""))
	if not discovery.is_empty() and not GameState.unlock_all_for_testing and not GameState.applied_location_unlocks.has(discovery):
		return false
	return stage >= 0 and GameState.progress_for_location(location) >= 1 and (
		stage != GameState.current_stage or floor != GameState.current_floor
	) and FileAccess.file_exists(DataLoader.floor_path(stage, floor))

func open_map() -> void:
	if visible or StoryManager.active or get_tree().paused:
		return
	var location := location_name_for(GameState.current_stage, GameState.current_floor)
	for button: Button in travel_buttons:
		var entry: Dictionary = travel_buttons[button]
		button.disabled = not can_travel(entry)
	selected_location.text = "所在：%s    ｜    金框地點可前往" % ("旅途中" if location.is_empty() else location)
	get_tree().paused = true
	show()

func location_name_for(stage: int, floor: int) -> String:
	for entry: Dictionary in locations:
		if str(entry.get("location", "")) == DataLoader.location_for_stage(stage):
			return str(entry.get("currentLabel", entry.name))
	return ""

func close_map() -> void:
	if visible:
		hide()
		get_tree().paused = false

func fit_board() -> void:
	var factor := minf(1.0, minf((size.x - 24.0) / BOARD_SIZE.x, (size.y - 24.0) / BOARD_SIZE.y))
	board.scale = Vector2.ONE * maxf(factor, 0.01)
	board.position = (size - BOARD_SIZE * board.scale) * 0.5

func _input(event: InputEvent) -> void:
	if visible and event is InputEventKey and event.pressed and not event.echo:
		if event.physical_keycode == KEY_M or event.keycode == KEY_ESCAPE:
			close_map()
			get_viewport().set_input_as_handled()
