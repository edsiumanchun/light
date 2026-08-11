class_name CastlePlayer
extends CharacterBody2D

@export var speed := 260.0
@export var jump_velocity := -520.0
@export var max_jumps := 2

var jumps := 0
var interaction_targets: Array[Node] = []

func _ready() -> void:
	add_to_group("player")

func _physics_process(delta: float) -> void:
	if StoryManager.active:
		velocity = Vector2.ZERO
		return
	if not is_on_floor():
		velocity.y += float(ProjectSettings.get_setting("physics/2d/default_gravity", 980.0)) * delta
	else:
		jumps = 0

	var direction := Input.get_axis("move_left", "move_right")
	velocity.x = move_toward(velocity.x, direction * speed, speed * 8.0 * delta)
	if direction != 0.0:
		$Visual.scale.x = absf($Visual.scale.x) * signf(direction)

	if Input.is_action_just_pressed("jump") and jumps < max_jumps:
		velocity.y = jump_velocity
		jumps += 1
	if Input.is_action_just_pressed("interact"):
		interact()
	move_and_slide()

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
