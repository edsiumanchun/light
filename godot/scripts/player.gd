class_name CastlePlayer
extends CharacterBody2D

@export var speed := 360.0
@export var jump_velocity := -520.0
@export var max_jumps := 2

var jumps := 0
var interaction_targets: Array[Node] = []
var animation_time := 0.0
var facing := 1.0
var story_walking := false
var story_sleeping := false
var upright_story_transform := Transform3D.IDENTITY

@onready var visual: SubViewportContainer = $Visual
@onready var model: Node3D = $Visual/Viewport3D/PlayerModel
@onready var animation_player: AnimationPlayer = $Visual/Viewport3D/PlayerModel/AnimationPlayer

func _ready() -> void:
	add_to_group("player")
	var walking := animation_player.get_animation("mixamo_com")
	if walking != null:
		walking.loop_mode = Animation.LOOP_LINEAR
		animation_player.play("mixamo_com")
		animation_player.pause()

func _physics_process(delta: float) -> void:
	if StoryManager.active:
		velocity = Vector2.ZERO
		if not story_sleeping:
			animate_model(delta, facing if story_walking else 0.0)
		return
	if not is_on_floor():
		velocity.y += float(ProjectSettings.get_setting("physics/2d/default_gravity", 980.0)) * delta
	else:
		jumps = 0

	var direction := Input.get_axis("move_left", "move_right")
	velocity.x = move_toward(velocity.x, direction * speed, speed * 8.0 * delta)
	if direction != 0.0:
		facing = signf(direction)

	if Input.is_action_just_pressed("jump") and jumps < max_jumps:
		velocity.y = jump_velocity
		jumps += 1
	if Input.is_action_just_pressed("interact"):
		interact()
	move_and_slide()
	animate_model(delta, direction)

func walk_to_story_bed(destination: Vector2) -> Tween:
	story_walking = true
	facing = -1.0 if destination.x < position.x else 1.0
	var tween := create_tween()
	tween.tween_property(self, "position", destination, position.distance_to(destination) / 150.0)
	tween.tween_callback(func() -> void: story_walking = false)
	tween.tween_interval(0.08)
	tween.tween_callback(func() -> void:
		upright_story_transform = model.transform
		story_sleeping = true
		rotate_story_pose(PI * 0.5)
	)
	tween.tween_interval(0.65)
	return tween

func get_up_from_story_bed(destination: Vector2) -> Tween:
	var tween := create_tween().set_parallel()
	if story_sleeping:
		tween.tween_method(rotate_story_pose, PI * 0.5, 0.0, 0.36)
	tween.tween_property(self, "position", destination, 0.36)
	tween.chain().tween_callback(func() -> void: story_sleeping = false)
	return tween

func rotate_story_pose(angle: float) -> void:
	var pivot := Vector3(0.0, 0.95, 0.0)
	var roll := Basis(Vector3.BACK, angle)
	model.transform = Transform3D(
		roll * upright_story_transform.basis,
		pivot + roll * (upright_story_transform.origin - pivot)
	)

func animate_model(delta: float, direction: float) -> void:
	animation_time += delta
	var grounded := is_on_floor()
	var moving := absf(direction) > 0.05 and grounded
	if moving:
		if not animation_player.is_playing():
			animation_player.play("mixamo_com")
		animation_player.speed_scale = clampf(absf(velocity.x) / speed, 0.65, 1.25)
	elif animation_player.is_playing():
		animation_player.pause()
		animation_player.seek(0.0, true)
	var frequency := 2.2
	var phase := animation_time * frequency
	var bob := 0.0 if moving else sin(phase) * 0.018
	var roll := 0.0 if moving else sin(phase) * 1.2
	var pitch := -8.0 if not grounded and velocity.y < 0.0 else (6.0 if not grounded else 0.0)
	var target_yaw := 90.0 * facing
	model.position.y = lerpf(model.position.y, bob, minf(delta * 12.0, 1.0))
	model.rotation_degrees.x = lerpf(model.rotation_degrees.x, pitch, minf(delta * 9.0, 1.0))
	model.rotation_degrees.y = lerp_angle(
		deg_to_rad(model.rotation_degrees.y),
		deg_to_rad(target_yaw),
		minf(delta * 10.0, 1.0)
	) * 180.0 / PI
	model.rotation_degrees.z = lerpf(model.rotation_degrees.z, roll, minf(delta * 14.0, 1.0))

func interact() -> void:
	interaction_targets = interaction_targets.filter(func(node): return is_instance_valid(node))
	if interaction_targets.is_empty():
		return
	var target := interaction_targets[0]
	if target.has_method("interact"):
		target.interact(self)

func can_interact() -> bool:
	interaction_targets = interaction_targets.filter(func(node): return is_instance_valid(node))
	return not interaction_targets.is_empty()

func _on_detector_body_entered(body: Node) -> void:
	if body.is_in_group("interactable") and not interaction_targets.has(body):
		interaction_targets.append(body)

func _on_detector_body_exited(body: Node) -> void:
	interaction_targets.erase(body)

func _on_detector_area_entered(area: Area2D) -> void:
	if area.is_in_group("interactable") and not interaction_targets.has(area):
		interaction_targets.append(area)

func _on_detector_area_exited(area: Area2D) -> void:
	interaction_targets.erase(area)
