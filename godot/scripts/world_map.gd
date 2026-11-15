extends Control

const BOARD_SIZE := Vector2(1060, 548)
const INK := Color("#f1deb6")
const MUTED := Color("#c2ac85")
const GOLD := Color("#c39a55")

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
	add_panel(Rect2(724, 126, 288, 318), Color("#2f2620"), Color("#98754c"))
	var illustration := Node2D.new()
	illustration.draw.connect(func(): draw_routes(illustration))
	board.add_child(illustration)

	add_caption("敦煌行旅圖", Rect2(34, 20, 410, 43), 34, INK)
	add_caption("敦煌境內與西行路線", Rect2(35, 66, 450, 24), 17, MUTED)
	add_caption("敦煌境內", Rect2(250, 121, 416, 30), 22, GOLD, HORIZONTAL_ALIGNMENT_CENTER)
	add_caption(str(config.get("west", "")), Rect2(45, 320, 177, 36), 20, INK, HORIZONTAL_ALIGNMENT_CENTER)
	add_caption("敦煌石窟一帶", Rect2(748, 145, 241, 30), 22, GOLD)
	for index in range(config.get("grottoSites", []).size()):
		var site: String = config.grottoSites[index]
		add_caption("◇  " + site, Rect2(752, 195 + index * 51, 222, 32), 19, INK)
		if index < config.grottoSites.size() - 1:
			add_caption("↓", Rect2(756, 224 + index * 51, 20, 19), 15, GOLD)

	for entry in locations:
		var center := Vector2(float(entry.position[0]), float(entry.position[1]))
		var bounds := Rect2(center - Vector2(90, 24), Vector2(180, 48))
		if entry.has("floor"):
			add_travel_button(entry, bounds)
		else:
			add_panel(bounds, Color("#46362b"), Color("#91744f"))
			add_caption(str(entry.name), bounds, 20, INK, HORIZONTAL_ALIGNMENT_CENTER)

	add_travel_button(config.get("home", {}), Rect2(824, 491, 187, 38))
	selected_location = add_caption("", Rect2(35, 490, 770, 40), 18, MUTED)
	var close_button := add_button("關閉  /  Esc", Rect2(871, 25, 142, 38))
	close_button.pressed.connect(close_map)

func draw_routes(canvas: Node2D) -> void:
	var route := Color("#bc9760")
	for index in range(locations.size() - 1):
		var a: Array = locations[index].position
		var b: Array = locations[index + 1].position
		var start := Vector2(float(a[0]), float(a[1]))
		var end := Vector2(float(b[0]), float(b[1]))
		var direction := start.direction_to(end)
		var margin := 92.0 if not is_zero_approx(direction.x) else 27.0
		draw_arrow(canvas, start + direction * margin, end - direction * margin, route)
	draw_arrow(canvas, Vector2(247, 338), Vector2(217, 338), route)
	for ridge in range(3):
		var y := 403.0 + ridge * 14.0
		canvas.draw_polyline(PackedVector2Array([
			Vector2(45, y + 15), Vector2(105, y - 3), Vector2(161, y + 6),
			Vector2(226, y - 13), Vector2(296, y + 1), Vector2(367, y - 9),
			Vector2(434, y + 8), Vector2(509, y - 7), Vector2(570, y + 1), Vector2(654, y - 17)
		]), Color(0.78, 0.62, 0.42, 0.25), 1.0, true)

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
	var button := add_button(str(entry.name), bounds)
	travel_buttons[button] = entry
	button.pressed.connect(func(): travel_to(entry))

func travel_to(entry: Dictionary) -> void:
	var floor := int(entry.floor)
	if not can_travel(entry):
		return
	close_map()
	GameState.change_floor(floor)

func can_travel(entry: Dictionary) -> bool:
	var stage := int(entry.stage)
	var floor := int(entry.floor)
	return stage == GameState.current_stage and floor != GameState.current_floor and FileAccess.file_exists(
		"%s/Game/%d/%d.json" % [DataLoader.JSON_ROOT, stage, floor]
	)

func open_map() -> void:
	if visible or StoryManager.active or get_tree().paused:
		return
	var location := "旅途中"
	for button: Button in travel_buttons:
		var entry: Dictionary = travel_buttons[button]
		if int(entry.stage) == GameState.current_stage and int(entry.floor) == GameState.current_floor:
			location = str(entry.get("currentLabel", entry.name))
		button.disabled = not can_travel(entry)
	selected_location.text = "所在：%s    ｜    金框地點可前往" % location
	get_tree().paused = true
	show()

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
