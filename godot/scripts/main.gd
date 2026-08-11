extends Node2D

@onready var level: LevelBuilder = $Level
@onready var camera: Camera2D = $Camera2D

func _ready() -> void:
	GameState.floor_changed.connect(_on_floor_changed)
	load_floor(GameState.current_stage, GameState.current_floor)

func load_floor(stage: int, floor: int) -> void:
	var player := level.build(stage, floor)
	if player != null:
		camera.position = Vector2(600, 337.5)

func _on_floor_changed(stage: int, floor: int) -> void:
	load_floor(stage, floor)
