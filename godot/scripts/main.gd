extends Node2D

const INTERACTION_ZOOM := Vector2(1.7, 1.7)
const CAMERA_FADE_SECONDS := 0.45
const TIME_SKIP_HOLD_SECONDS := 1.2

@onready var level: LevelBuilder = $Level
@onready var camera: Camera2D = $Camera2D

var player: CastlePlayer
var camera_tween: Tween
var camera_fade: ColorRect

func _ready() -> void:
	var fade_layer := CanvasLayer.new()
	fade_layer.layer = 100
	add_child(fade_layer)
	camera_fade = ColorRect.new()
	camera_fade.color = Color(0, 0, 0, 0)
	camera_fade.mouse_filter = Control.MOUSE_FILTER_IGNORE
	fade_layer.add_child(camera_fade)
	camera_fade.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	GameState.floor_changed.connect(_on_floor_changed)
	StoryManager.opening.connect(_focus_on_player)
	StoryManager.leaving_dialogue.connect(_show_story_action)
	StoryManager.time_skip_requested.connect(_show_time_skip)
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
	if camera_tween != null and camera_tween.is_valid():
		camera_tween.kill()
	camera_fade.color.a = 0.0
	player = level.build(stage, floor)
	if player != null:
		camera.zoom = level.gameplay_zoom()
		camera.position = gameplay_camera_position()
	if level.init_story > 0:
		_start_initial_story.call_deferred(stage, level.init_story)

func _start_initial_story(stage: int, story_number: int) -> void:
	if stage != GameState.current_stage or StoryManager.active:
		return
	StoryManager.start_initial_story(story_number)

func _on_floor_changed(stage: int, floor: int) -> void:
	load_floor(stage, floor)

func _focus_on_player() -> void:
	if not is_instance_valid(player):
		StoryManager.reveal_dialogue()
		return
	var focus := player.global_position + Vector2(0, -70)
	if is_instance_valid(level.teacher_exit_door) and (level.sleep_event == 0 or StoryManager.current_event_number != level.sleep_event):
		var half_width := level.view_size.x / INTERACTION_ZOOM.x * 0.5
		focus.x = maxf(focus.x, level.teacher_exit_door.global_position.x - half_width + 48.0)
	fade_camera_to(focus, INTERACTION_ZOOM)
	camera_tween.tween_callback(StoryManager.reveal_dialogue)

func _restore_camera() -> void:
	fade_camera_to(gameplay_camera_position(), level.gameplay_zoom())

func _show_story_action() -> void:
	_restore_camera()
	camera_tween.tween_callback(StoryManager.run_current_cue)

func _show_time_skip() -> void:
	fade_camera_to(gameplay_camera_position(), level.gameplay_zoom(), TIME_SKIP_HOLD_SECONDS)
	camera_tween.tween_callback(StoryManager.finish_story_cue)

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

func fade_camera_to(target_position: Vector2, target_zoom: Vector2, hold_seconds := 0.0) -> void:
	if camera_tween != null and camera_tween.is_valid():
		camera_tween.kill()
	camera_tween = create_tween().set_trans(Tween.TRANS_QUAD).set_ease(Tween.EASE_IN_OUT)
	camera_tween.tween_property(camera_fade, "color:a", 1.0, CAMERA_FADE_SECONDS)
	camera_tween.tween_callback(func() -> void:
		camera.position = target_position
		camera.zoom = target_zoom
		camera.force_update_scroll()
	)
	if hold_seconds > 0.0:
		camera_tween.tween_interval(hold_seconds)
	camera_tween.tween_property(camera_fade, "color:a", 0.0, CAMERA_FADE_SECONDS)
