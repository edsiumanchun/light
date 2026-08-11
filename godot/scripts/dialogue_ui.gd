extends Control

@onready var speaker: Label = $Panel/Margin/VBox/Speaker
@onready var dialogue: RichTextLabel = $Panel/Margin/VBox/Dialogue

func _ready() -> void:
	StoryManager.opened.connect(func(): visible = true)
	StoryManager.closed.connect(func(): visible = false)
	StoryManager.line_changed.connect(show_line)
	visible = false
	mouse_filter = Control.MOUSE_FILTER_IGNORE

func show_line(name: String, text: String) -> void:
	speaker.text = name
	dialogue.text = text
