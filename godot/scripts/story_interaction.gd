class_name StoryInteraction
extends StaticBody2D

@export var suffix := ""
@export var story_index := 0
var require_proximity := false
var interaction_enabled := true
const WALK_SPEED := 150.0
const WALK_ANIMATION := "mixamo_com"

var home_position := Vector2.ZERO
var exit_position := Vector2.ZERO
var model: Node3D
var animation: AnimationPlayer

func _ready() -> void:
	add_to_group("interactable")

func setup_exit(destination: Vector2) -> void:
	home_position = position
	exit_position = destination
	setup_pose(-1.0)

func setup_pose(direction: float) -> void:
	model = get_node_or_null("CharacterVisual/Viewport3D/PlayerModel") as Node3D
	if model != null:
		animation = model.get_node_or_null("AnimationPlayer") as AnimationPlayer
		face_direction(direction)
		set_walking(false)

func play_exit_cue(cue: String) -> Tween:
	var leaving := cue == "teacherleave"
	var destination := exit_position if leaving else home_position
	return walk_to(destination, 0.0 if leaving else 0.65, leaving)

func walk_to(destination: Vector2, delay := 0.0, hide_on_arrival := false) -> Tween:
	var tween := create_tween()
	if delay > 0.0:
		tween.tween_interval(delay)
	tween.tween_callback(func() -> void:
		show()
		if home_position != exit_position:
			collision_layer = 5
			input_pickable = interaction_enabled
		face_direction(signf(destination.x - position.x))
		set_walking(true)
	)
	tween.tween_property(self, "position", destination, position.distance_to(destination) / WALK_SPEED)
	tween.tween_callback(func() -> void:
		set_walking(false)
		visible = not hide_on_arrival
		if hide_on_arrival:
			collision_layer = 0
			input_pickable = false
	)
	return tween

func face_direction(direction: float) -> void:
	if model != null:
		model.rotation_degrees.y = direction * 90.0

func set_walking(walking: bool) -> void:
	if animation == null or not animation.has_animation(WALK_ANIMATION):
		return
	animation.get_animation(WALK_ANIMATION).loop_mode = Animation.LOOP_LINEAR
	animation.play(WALK_ANIMATION)
	if not walking:
		animation.seek(0.0, true)
		animation.pause()

func interact(player: CastlePlayer) -> void:
	if not interaction_enabled:
		return
	if require_proximity and (player == null or not player.interaction_targets.has(self)):
		return
	StoryManager.start_story(suffix, story_index)

func _input_event(_viewport: Node, event: InputEvent, _shape_index: int) -> void:
	var clicked: bool = false
	if event is InputEventMouseButton:
		clicked = event.button_index == MOUSE_BUTTON_LEFT and event.pressed
	elif event is InputEventScreenTouch:
		clicked = event.pressed
	if clicked:
		interact(get_tree().get_first_node_in_group("player") as CastlePlayer)
		get_viewport().set_input_as_handled()
