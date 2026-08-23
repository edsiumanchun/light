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

func _ready() -> void:
	GameState.stats_changed.connect(refresh)
	GameState.floor_changed.connect(func(_stage, _floor): refresh())
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
	refresh()

func _process(_delta: float) -> void:
	if StoryManager.active:
		interact_button.visible = true
		return
	var player := get_tree().get_first_node_in_group("player")
	interact_button.visible = player != null and player.has_method("can_interact") and player.can_interact()

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
	stats.text = "Stage %d   Floor %d" % [GameState.current_stage, GameState.current_floor]
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
