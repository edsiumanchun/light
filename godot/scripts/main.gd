extends Node2D

const INTERACTION_ZOOM := Vector2(1.7, 1.7)

@onready var level: LevelBuilder = $Level
@onready var camera: Camera2D = $Camera2D

var player: CastlePlayer
var camera_tween: Tween

func _ready() -> void:
	GameState.floor_changed.connect(_on_floor_changed)
	StoryManager.opened.connect(_focus_on_player)
	StoryManager.closed.connect(_restore_camera)
	load_floor(GameState.current_stage, GameState.current_floor)

func _process(delta: float) -> void:
	if StoryManager.active or not is_instance_valid(player):
		return
	if not level.camera_follow_player_x and not level.camera_follow_player_y:
		return
	if camera_tween != null and camera_tween.is_valid():
		return
	if level.camera_follow_player_x:
		var difference_x := player.global_position.x - camera.position.x
		if absf(difference_x) > level.camera_horizontal_dead_zone:
			var target_x := player.global_position.x - signf(difference_x) * level.camera_horizontal_dead_zone
			camera.position.x = move_toward(camera.position.x, clamp_camera_x(target_x, camera.zoom.x), 650.0 * delta)
	if level.camera_follow_player_y:
		var difference_y := player.global_position.y - camera.position.y
		if absf(difference_y) > level.camera_vertical_dead_zone:
			var target_y := player.global_position.y - signf(difference_y) * level.camera_vertical_dead_zone
			camera.position.y = move_toward(camera.position.y, clamp_camera_y(target_y, camera.zoom.y), 650.0 * delta)

func load_floor(stage: int, floor: int) -> void:
	player = level.build(stage, floor)
	if player != null:
		camera.zoom = level.gameplay_zoom()
		camera.position = gameplay_camera_position()

func _on_floor_changed(stage: int, floor: int) -> void:
	load_floor(stage, floor)

func _focus_on_player() -> void:
	if not is_instance_valid(player):
		return
	animate_camera(player.global_position + Vector2(0, -70), INTERACTION_ZOOM, 0.4)

func _restore_camera() -> void:
	animate_camera(gameplay_camera_position(), level.gameplay_zoom(), 0.32)

func gameplay_camera_position() -> Vector2:
	var target_x := level.view_size.x * 0.5
	var target_y := level.view_size.y * 0.5
	if level.camera_follow_player_x and is_instance_valid(player):
		target_x = player.global_position.x
	if level.camera_follow_player_y and is_instance_valid(player):
		target_y = player.global_position.y
	var zoom := level.gameplay_zoom()
	return Vector2(clamp_camera_x(target_x, zoom.x), clamp_camera_y(target_y, zoom.y))

func clamp_camera_x(target_x: float, zoom_x: float) -> float:
	var half_visible_width := level.view_size.x / maxf(zoom_x, 0.001) * 0.5
	if level.world_size.x <= half_visible_width * 2.0:
		return level.world_size.x * 0.5
	return clampf(target_x, half_visible_width, level.world_size.x - half_visible_width)

func clamp_camera_y(target_y: float, zoom_y: float) -> float:
	var half_visible_height := level.view_size.y / maxf(zoom_y, 0.001) * 0.5
	if level.world_size.y <= half_visible_height * 2.0:
		return level.world_size.y * 0.5
	return clampf(target_y, half_visible_height, level.world_size.y - half_visible_height)

func animate_camera(target_position: Vector2, target_zoom: Vector2, duration: float) -> void:
	if camera_tween != null and camera_tween.is_valid():
		camera_tween.kill()
	camera_tween = create_tween().set_parallel().set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_IN_OUT)
	camera_tween.tween_property(camera, "position", target_position, duration)
	camera_tween.tween_property(camera, "zoom", target_zoom, duration)
