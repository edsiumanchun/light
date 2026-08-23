extends SceneTree

func _initialize() -> void:
	call_deferred("run_test")

func run_test() -> void:
	var game_state := root.get_node("GameState")
	var original_inventory: Dictionary = game_state.inventory.duplicate(true)
	game_state.inventory = {
		"Greed": 7, "Hatred": 8, "Delusion": 9,
		"Copper": 2, "Silver": 3, "Gold": 4,
	}
	var scene := load("res://godot/scenes/main.tscn") as PackedScene
	var main := scene.instantiate()
	root.add_child(main)
	var avatar := main.get_node("HUD/Avatar") as TextureButton
	var dashboard := main.get_node("HUD/Dashboard") as Control
	if dashboard.visible:
		fail("Dashboard should start closed", original_inventory)
		return
	avatar.pressed.emit()
	if not dashboard.visible:
		fail("Avatar button did not open the dashboard", original_inventory)
		return
	var expected := {
		"Greed": "7", "Hatred": "8", "Delusion": "9",
		"Copper": "2", "Silver": "3", "Gold": "4",
	}
	for item_name in expected:
		var value := dashboard.get_node("Panel/Margin/VBox/%s/Value" % item_name) as Label
		if value.text != expected[item_name]:
			fail("Dashboard count is incorrect for %s" % item_name, original_inventory)
			return
	var close := dashboard.get_node("Panel/Margin/VBox/Header/Close") as Button
	close.pressed.emit()
	if dashboard.visible:
		fail("Dashboard close button did not close the panel", original_inventory)
		return
	game_state.inventory = original_inventory
	print("Dashboard UI test passed: avatar opens six live inventory counts and close dismisses it.")
	quit()

func fail(message: String, original_inventory: Dictionary) -> void:
	root.get_node("GameState").inventory = original_inventory
	push_error(message)
	quit(1)
