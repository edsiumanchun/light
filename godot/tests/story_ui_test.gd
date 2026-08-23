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
	var left_portrait := main.get_node_or_null("HUD/Dialogue/LeftPortrait") as TextureRect
	var right_portrait := main.get_node_or_null("HUD/Dialogue/RightPortrait") as TextureRect
	var speaker := main.get_node_or_null("HUD/Dialogue/Panel/Margin/VBox/Speaker") as Label
	var text := main.get_node_or_null("HUD/Dialogue/Panel/Margin/VBox/Dialogue") as RichTextLabel
	var button := main.get_node_or_null("HUD/InteractButton") as BaseButton
	if dialogue == null or left_portrait == null or right_portrait == null or speaker == null or text == null or button == null:
		push_error("Dialogue UI nodes are missing")
		quit(1)
		return
	if not speaker.get_theme_color("font_color").is_equal_approx(Color8(30, 150, 150)):
		push_error("Speaker name color is not RGB (30, 150, 150)")
		quit(1)
		return
	if not left_portrait.visible or not right_portrait.visible:
		push_error("Both available story portraits should be visible")
		quit(1)
		return
	var active_portrait := left_portrait if is_equal_approx(left_portrait.modulate.a, 1.0) else right_portrait
	var inactive_portrait := right_portrait if active_portrait == left_portrait else left_portrait
	if not is_equal_approx(inactive_portrait.modulate.a, 0.35):
		push_error("Inactive portrait is not alpha-dimmed")
		quit(1)
		return
	var panel := main.get_node("HUD/Dialogue/Panel") as PanelContainer
	if active_portrait.texture == null or active_portrait.z_index <= panel.z_index:
		push_error("Active portrait is missing or is not above the chat panel")
		quit(1)
		return
	if not is_equal_approx(active_portrait.offset_bottom, 0.0):
		push_error("Active portrait is not anchored to the bottom of the screen")
		quit(1)
		return
	if not is_equal_approx(panel.anchor_left, 0.5) or not is_equal_approx(panel.anchor_right, 0.5):
		push_error("Chat panel is not anchored at the horizontal center")
		quit(1)
		return
	if not is_equal_approx(panel.offset_left, -panel.offset_right):
		push_error("Chat panel is not centered on the screen")
		quit(1)
		return
	if not is_equal_approx(left_portrait.offset_right, panel.offset_left) or not is_equal_approx(right_portrait.offset_left, panel.offset_right):
		push_error("Story portraits are not attached directly to both sides of the chat panel")
		quit(1)
		return
	var chat_background := main.get_node("HUD/Dialogue/ChatBackground") as Panel
	if not is_equal_approx(chat_background.anchor_left, 0.0) or not is_equal_approx(chat_background.anchor_right, 1.0):
		push_error("Chat background does not span the full screen width")
		quit(1)
		return
	var chat_style := chat_background.get_theme_stylebox("panel") as StyleBoxFlat
	if chat_style == null or chat_style.border_width_top != 0 or chat_style.border_width_bottom != 0 or chat_style.border_width_left != 0 or chat_style.border_width_right != 0:
		push_error("Chat background should not show a border")
		quit(1)
		return
	if not right_portrait.flip_h:
		push_error("Right story portrait is not flipped horizontally")
		quit(1)
		return
	var margin := main.get_node("HUD/Dialogue/Panel/Margin") as MarginContainer
	if margin.get_theme_constant("margin_top") != 5 or margin.get_theme_constant("margin_left") != -5:
		push_error("Chat text padding is not top 5 and left -5")
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
