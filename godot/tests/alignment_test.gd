extends SceneTree

func _initialize() -> void:
	var player_scene := load("res://godot/scenes/player.tscn") as PackedScene
	var player := player_scene.instantiate()
	root.add_child(player)
	var player_shape := player.get_node("CollisionShape2D") as CollisionShape2D
	var capsule := player_shape.shape as CapsuleShape2D
	if player_shape.position != Vector2(0, -60) or capsule.height != 120.0:
		push_error("Player collider is not grounded to the character height")
		quit(1)
		return
	var visual := player.get_node("Visual")
	var camera := visual.get_node("Viewport3D/Camera3D") as Camera3D
	if not is_equal_approx(camera.position.y, 1.095):
		push_error("Character visual camera is not ground-aligned")
		quit(1)
		return
	var builder := load("res://godot/scripts/level_builder.gd")
	if builder == null:
		push_error("Level builder failed to compile")
		quit(1)
		return
	var level_builder = builder.new()
	root.add_child(level_builder)
	level_builder.set("view_size", Vector2(1200, 675))
	level_builder.set("world_size", Vector2(1200, 850))
	var nested_grid: Array = level_builder.call("asset_grid", {"grid": [[1, 2], [3, 4]]})
	var legacy_grid: Array = level_builder.call("asset_grid", {"gridArray0": [1, 2], "gridArray1": [3, 4]})
	if nested_grid.size() != 2 or legacy_grid != nested_grid:
		push_error("Nested and legacy grid formats are not compatible")
		quit(1)
		return
	var floor_one := DataLoader.floor_data(0, 1)
	var detected_size: Vector2i = level_builder.call("detect_grid_size", floor_one)
	if detected_size != Vector2i(9, 4):
		push_error("Dynamic grid size should detect the current 9-column, 4-row grids")
		quit(1)
		return
	level_builder.call(
		"configure_grid",
		floor_one,
		floor_one.get("layout", {}),
		floor_one.get("camera", {})
	)
	if not level_builder.get("camera_follow_player_x") or not is_equal_approx(level_builder.get("world_size").x, 1200.0):
		push_error("Horizontal floor did not enable X following or expand to its grid width")
		quit(1)
		return
	level_builder.set("world_size", Vector2(1200, 1020))
	level_builder.set("content_height", 1020.0)
	level_builder.set("platform_half_thickness", 5.0)
	level_builder.set("camera_visible_rows", 4.0)
	level_builder.set("cell_size", Vector2(131, 170))
	level_builder.set("view_size", Vector2(1200, 675))
	var zoom: Vector2 = level_builder.call("gameplay_zoom")
	if not is_equal_approx(675.0 / zoom.y, 685.0):
		push_error("Camera height does not include exactly half a PlateH allowance")
		quit(1)
		return
	var interior_surface: float = level_builder.call("horizontal_plate_surface_y", 850.0)
	var bottom_surface: float = level_builder.call("horizontal_plate_surface_y", 1020.0)
	if not is_equal_approx(interior_surface, 845.0) or not is_equal_approx(bottom_surface, 1010.0):
		push_error("Objects are not aligned with the actual top surface of horizontal plates")
		quit(1)
		return
	level_builder.set("content_height", 680.0)
	level_builder.call("build_tiled_wall_background", ["Blue0", "Blue1", "Blue2"], 0)
	var wall_layer: Node = level_builder.get_node_or_null("WallBackground0")
	if wall_layer == null or wall_layer.get_child_count() < 4:
		push_error("Wall background did not cover the complete stage height")
		quit(1)
		return
	level_builder.call("build", 0, 0)
	var door := level_builder.get_node_or_null("Door") as Area2D
	if door == null or not door.is_in_group("interactable"):
		push_error("JSON door was not created as an interactable Area2D")
		quit(1)
		return
	if door.collision_layer != 4 or door.collision_mask != 0 or int(door.get("target_floor")) != 1:
		push_error("Door is blocking movement or did not load its JSON destination")
		quit(1)
		return
	level_builder.call("build", 0, 1)
	var door_targets: Array[int] = []
	for child in level_builder.get_children():
		if child is Area2D and child.get("target_floor") != null and int(child.get("target_floor")) > 1:
			door_targets.append(int(child.get("target_floor")))
	door_targets.sort()
	if door_targets != [2, 3, 4]:
		push_error("Door grid values did not create destinations 2, 3, and 4")
		quit(1)
		return
	level_builder.call("build", 0, 2)
	if level_builder.get_node_or_null("LeftBackgroundMask") == null or level_builder.get_node_or_null("RightBackgroundMask") == null:
		push_error("Centered level did not mask both outside background margins")
		quit(1)
		return
	print("Alignment test passed: grounded objects and tiled wall background.")
	quit()
