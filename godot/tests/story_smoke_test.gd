extends SceneTree

func _initialize() -> void:
	call_deferred("run_test")

func run_test() -> void:
	var story := root.get_node_or_null("StoryManager")
	if story == null:
		push_error("StoryManager autoload is missing")
		quit(1)
		return
	story.start_story("", 0)
	if not story.active or story.lines.is_empty():
		push_error("Story did not open or contained no playable dialogue")
		quit(1)
		return
	var count: int = story.lines.size()
	for index in range(count):
		story.next_line()
	if story.active:
		push_error("Story did not close after its final line")
		quit(1)
		return
	print("Story smoke test passed: %d dialogue lines advanced and closed." % count)
	quit()
