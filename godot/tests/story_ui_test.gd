extends SceneTree

func _initialize() -> void:
	call_deferred("run_test")

func run_test() -> void:
	var scene: PackedScene = load("res://godot/scenes/main.tscn")
	var main := scene.instantiate()
	root.add_child(main)
	await process_frame
	await process_frame
	var story := root.get_node("StoryManager")
	story.start_story("", 0)
	await process_frame
	var dialogue := main.get_node_or_null("HUD/Dialogue") as Control
	var speaker := main.get_node_or_null("HUD/Dialogue/Panel/Margin/VBox/Speaker") as Label
	var text := main.get_node_or_null("HUD/Dialogue/Panel/Margin/VBox/Dialogue") as RichTextLabel
	var button := main.get_node_or_null("HUD/InteractButton") as Button
	if dialogue == null or speaker == null or text == null or button == null:
		push_error("Dialogue UI nodes are missing")
		quit(1)
		return
	print("Dialogue visible=%s speaker='%s' text='%s'" % [dialogue.visible, speaker.text, text.text])
	if not dialogue.visible or text.text.is_empty():
		push_error("Dialogue UI did not display the opened story")
		quit(1)
		return
	var first_line := text.text
	button.pressed.emit()
	await process_frame
	if text.text == first_line:
		push_error("Interact button did not advance dialogue")
		quit(1)
		return
	print("Interact button advanced dialogue to: '%s'" % text.text)
	quit()
