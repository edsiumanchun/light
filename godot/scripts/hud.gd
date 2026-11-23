extends CanvasLayer

@onready var stats: Label = $Margin/VBox/Stats
@onready var help: Label = $Margin/VBox/Help
@onready var hp_bar: TextureProgressBar = $HPBar
@onready var mp_bar: TextureProgressBar = $MPBar
@onready var hp_text: Label = $HPText
@onready var mp_text: Label = $MPText
@onready var copper_text: Label = $Keys/Copper
@onready var silver_text: Label = $Keys/Silver
@onready var gold_text: Label = $Keys/Gold
@onready var interact_button: TextureButton = $InteractButton
@onready var interact_label: Label = $InteractButton/Label
@onready var avatar_button: TextureButton = $Avatar
@onready var dashboard: Control = $Dashboard
@onready var dashboard_dismiss: Button = $Dashboard/Dismiss
@onready var dashboard_close: Button = $Dashboard/Panel/Margin/VBox/Header/Close

var dashboard_values: Dictionary = {}
const WORLD_MAP := preload("res://godot/scripts/world_map.gd")
var world_map: Control
var map_button: Button
var test_controls: VBoxContainer

func _ready() -> void:
	GameState.stats_changed.connect(refresh)
	GameState.floor_changed.connect(func(_stage, _floor): refresh())
	StoryManager.story_selected.connect(refresh)
	StoryManager.closed.connect(refresh)
	StoryManager.opened.connect(func(): interact_label.text = "Next")
	StoryManager.opened.connect(close_dashboard)
	StoryManager.closed.connect(func(): interact_label.text = "Interact")
	help.visible = false
	interact_label.text = "Interact"
	interact_button.pressed.connect(on_interact_pressed)
	avatar_button.pressed.connect(toggle_dashboard)
	dashboard_dismiss.pressed.connect(close_dashboard)
	dashboard_close.pressed.connect(close_dashboard)
	for item_name in ["Greed", "Hatred", "Delusion", "Copper", "Silver", "Gold"]:
		dashboard_values[item_name] = dashboard.get_node("Panel/Margin/VBox/%s/Value" % item_name)
	dashboard.visible = false
	world_map = WORLD_MAP.new()
	world_map.name = "WorldMap"
	world_map.z_index = 50
	add_child(world_map)
	map_button = Button.new()
	map_button.text = "地圖 / M"
	map_button.custom_minimum_size = Vector2(112, 36)
	add_child(map_button)
	map_button.set_anchors_and_offsets_preset(Control.PRESET_TOP_RIGHT)
	map_button.offset_left = -132
	map_button.offset_right = -20
	map_button.offset_top = 18
	map_button.offset_bottom = 54
	map_button.pressed.connect(open_world_map)
	GameState.world_map_requested.connect(open_world_map)
	build_test_controls()
	refresh()

func build_test_controls() -> void:
	test_controls = VBoxContainer.new()
	add_child(test_controls)
	test_controls.set_anchors_and_offsets_preset(Control.PRESET_BOTTOM_LEFT)
	test_controls.offset_left = 20
	test_controls.offset_right = 158
	test_controls.offset_top = -174
	test_controls.offset_bottom = -18

	var jump_dialog := AcceptDialog.new()
	jump_dialog.title = "Jump to Floor"
	jump_dialog.dialog_hide_on_ok = false
	add_child(jump_dialog)
	var fields := VBoxContainer.new()
	fields.custom_minimum_size.x = 240
	jump_dialog.add_child(fields)
	var floor_input := LineEdit.new()
	floor_input.placeholder_text = "Floor in current location (e.g. 0a)"
	fields.add_child(floor_input)
	var error_label := Label.new()
	fields.add_child(error_label)
	jump_dialog.register_text_enter(floor_input)
	jump_dialog.confirmed.connect(func():
		if GameState.jump_to_floor_for_testing(floor_input.text.strip_edges().to_lower()):
			jump_dialog.hide()
		else:
			error_label.text = "That floor does not exist here."
	)
	add_test_button("Jump to Floor").pressed.connect(func():
		error_label.text = ""
		floor_input.text = DataLoader.floor_label(GameState.current_stage, GameState.current_floor)
		jump_dialog.popup_centered()
		floor_input.grab_focus()
		floor_input.select_all()
	)

	var reset_dialog := ConfirmationDialog.new()
	reset_dialog.title = "Reset Progress"
	reset_dialog.dialog_text = "Reset all progress, items, and unlocks in this save slot?"
	add_child(reset_dialog)
	reset_dialog.confirmed.connect(GameState.reset_progress_for_testing)
	add_test_button("Reset All").pressed.connect(func(): reset_dialog.popup_centered())
	var unlock_all_button := add_test_button("Unlock All + Keys")
	unlock_all_button.tooltip_text = "Unlock all available destinations and bypass door and gate requirements. Top up each key type to 999."
	unlock_all_button.pressed.connect(GameState.unlock_all_stages_for_testing)
	var lighting_button := add_test_button("日夜預覽 · 原景")
	lighting_button.tooltip_text = "切換原景、日景與夜景。適用於靈藏的住處、月牙客棧與敦煌邊境外景。"
	lighting_button.pressed.connect(GameState.cycle_lighting_preview)
	GameState.day_night_preview_changed.connect(func():
		var labels := ["原景", "日景", "夜景"]
		lighting_button.text = "日夜預覽 · " + labels[GameState.lighting_preview]
	)

func add_test_button(label: String) -> Button:
	var button := Button.new()
	button.text = label
	button.custom_minimum_size.y = 36
	test_controls.add_child(button)
	return button

func _process(_delta: float) -> void:
	test_controls.visible = not StoryManager.active and not world_map.visible
	map_button.visible = not StoryManager.active
	if StoryManager.active:
		interact_button.visible = StoryManager.dialogue_visible
		return
	var player := get_tree().get_first_node_in_group("player") as CastlePlayer
	var target := player.nearest_interaction() if player != null else null
	interact_button.visible = target != null
	var door := target as CastleDoor
	interact_label.text = "Interact"
	if door != null:
		if door.is_locked():
			interact_label.text = "Locked"
		elif door.opens_world_map:
			interact_label.text = "Open Map"
	elif target is CastleLevelExit and target.target_floor >= 0:
		interact_label.text = "Locked" if target.is_locked() else "Enter Cellar"

func _unhandled_key_input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and not event.echo and event.physical_keycode == KEY_M:
		open_world_map()
		get_viewport().set_input_as_handled()

func open_world_map() -> void:
	if not StoryManager.active:
		close_dashboard()
		world_map.open_map()

func on_interact_pressed() -> void:
	if StoryManager.active:
		StoryManager.next_line()
		return
	var player := get_tree().get_first_node_in_group("player")
	if player != null and player.has_method("interact"):
		player.interact()

func toggle_dashboard() -> void:
	dashboard.visible = not dashboard.visible
	if dashboard.visible:
		refresh()

func close_dashboard() -> void:
	dashboard.visible = false

func refresh() -> void:
	var stage := GameState.current_stage
	var floor := GameState.current_floor
	var location: String = world_map.location_name_for(stage, floor)
	stats.text = "%s · Floor %s" % [location, DataLoader.floor_label(stage, floor)] if not location.is_empty() else "Stage %d   Floor %d" % [stage, floor]
	if StoryManager.active:
		stats.text += " · Story %s" % StoryManager.current_story_id
	hp_bar.max_value = maxf(GameState.hp_max, 1)
	hp_bar.value = GameState.hp
	mp_bar.max_value = maxf(GameState.mp_max, 1)
	mp_bar.value = GameState.mp
	hp_text.text = "%d / %d" % [GameState.hp, GameState.hp_max]
	mp_text.text = "%d / %d" % [GameState.mp, GameState.mp_max]
	copper_text.text = "×%d" % GameState.inventory.get("Copper", 0)
	silver_text.text = "×%d" % GameState.inventory.get("Silver", 0)
	gold_text.text = "×%d" % GameState.inventory.get("Gold", 0)
	for item_name in dashboard_values:
		var value_label: Label = dashboard_values[item_name]
		value_label.text = str(GameState.inventory.get(item_name, 0))
