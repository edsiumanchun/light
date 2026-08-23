extends Control

const PORTRAIT_DIR := "res://Resources/Sprites/Story"
const SIDE_PADDING := 0.0
const CHAT_WIDTH := 480.0
const PORTRAIT_WIDTH := 360.0
const INACTIVE_PORTRAIT_ALPHA := 0.35
const PORTRAIT_ALIASES := {
	"ghostmaria": "mariaghost",
	"soilder": "soldier",
}

@onready var left_portrait: TextureRect = $LeftPortrait
@onready var right_portrait: TextureRect = $RightPortrait
@onready var chat_background: Panel = $ChatBackground
@onready var panel: PanelContainer = $Panel
@onready var speaker: Label = $Panel/Margin/VBox/Speaker
@onready var dialogue: RichTextLabel = $Panel/Margin/VBox/Dialogue

var portrait_paths: Dictionary = {}

func _ready() -> void:
	_index_portraits()
	StoryManager.opened.connect(func(): visible = true)
	StoryManager.closed.connect(_close)
	StoryManager.line_changed.connect(show_line)
	visible = false
	mouse_filter = Control.MOUSE_FILTER_IGNORE

func show_line(name: String, text: String, left_name: String, right_name: String) -> void:
	speaker.text = name
	dialogue.text = text
	var active_key := _portrait_key(name)
	var left_key := _portrait_key(left_name)
	var right_key := _portrait_key(right_name)
	left_portrait.texture = _portrait_texture(left_key)
	right_portrait.texture = _portrait_texture(right_key)
	var show_left := left_portrait.texture != null
	var show_right := right_portrait.texture != null
	var left_active := show_left and left_key == active_key
	var right_active := show_right and right_key == active_key
	if not left_active and not right_active:
		left_active = show_left
		right_active = not left_active and show_right
	left_portrait.visible = show_left
	right_portrait.visible = show_right
	left_portrait.modulate = Color.WHITE if left_active else Color(1.0, 1.0, 1.0, INACTIVE_PORTRAIT_ALPHA)
	right_portrait.modulate = Color.WHITE if right_active else Color(1.0, 1.0, 1.0, INACTIVE_PORTRAIT_ALPHA)
	_layout_dialogue(show_left, show_right)

func _index_portraits() -> void:
	for filename in DirAccess.get_files_at(PORTRAIT_DIR):
		if filename.get_extension().to_lower() in ["png", "jpg", "jpeg", "webp"]:
			portrait_paths[filename.get_basename().to_lower()] = "%s/%s" % [PORTRAIT_DIR, filename]

func _portrait_key(portrait_name: String) -> String:
	var key := portrait_name.strip_edges().to_lower()
	return str(PORTRAIT_ALIASES.get(key, key))

func _portrait_texture(key: String) -> Texture2D:
	var path := str(portrait_paths.get(key, ""))
	return load(path) as Texture2D if not path.is_empty() else null

func _layout_dialogue(show_left: bool, show_right: bool) -> void:
	var chat_half_width := CHAT_WIDTH * 0.5
	panel.anchor_left = 0.5
	panel.anchor_right = 0.5
	panel.offset_left = -chat_half_width
	panel.offset_right = chat_half_width
	chat_background.anchor_left = 0.0
	chat_background.anchor_right = 1.0
	chat_background.offset_left = 0.0
	chat_background.offset_right = 0.0
	if show_left:
		left_portrait.anchor_left = 0.5
		left_portrait.anchor_right = 0.5
		left_portrait.offset_right = -chat_half_width - SIDE_PADDING
		left_portrait.offset_left = left_portrait.offset_right - PORTRAIT_WIDTH
	if show_right:
		right_portrait.anchor_left = 0.5
		right_portrait.anchor_right = 0.5
		right_portrait.offset_left = chat_half_width + SIDE_PADDING
		right_portrait.offset_right = right_portrait.offset_left + PORTRAIT_WIDTH

func _close() -> void:
	visible = false
	left_portrait.texture = null
	right_portrait.texture = null
	left_portrait.visible = false
	right_portrait.visible = false
