extends CanvasLayer

@onready var stats: Label = $Margin/VBox/Stats
@onready var help: Label = $Margin/VBox/Help
@onready var interact_button: Button = $InteractButton

func _ready() -> void:
	GameState.stats_changed.connect(refresh)
	GameState.floor_changed.connect(func(_stage, _floor): refresh())
	StoryManager.opened.connect(func(): interact_button.text = "Next")
	StoryManager.closed.connect(func(): interact_button.text = "Interact")
	help.text = "A/D or arrows: move   Space: jump   E/Enter: interact"
	interact_button.text = "Interact"
	interact_button.pressed.connect(on_interact_pressed)
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

func refresh() -> void:
	stats.text = "Stage %d  Floor %d     HP %d/%d  MP %d/%d     Keys C:%d S:%d G:%d" % [
		GameState.current_stage, GameState.current_floor,
		GameState.hp, GameState.hp_max, GameState.mp, GameState.mp_max,
		GameState.inventory.get("Copper", 0), GameState.inventory.get("Silver", 0),
		GameState.inventory.get("Gold", 0)
	]
