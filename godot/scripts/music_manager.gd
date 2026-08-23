extends Node

signal mute_changed(muted: bool)

const AUDIO_DIR := "res://Resources/Audios"
const PLAYBACK_VOLUME := 0.5

var player: AudioStreamPlayer
var current_track := ""

func _ready() -> void:
	player = AudioStreamPlayer.new()
	player.name = "MusicPlayer"
	add_child(player)
	player.finished.connect(_on_track_finished)
	_apply_mute()
	# Provide music even on floors whose migrated JSON has no Audio entry.
	play_track("bgm")

func play_track(track_name: String) -> void:
	if track_name.is_empty() or track_name == current_track:
		if not player.playing and player.stream != null:
			player.play()
		return
	var path := "%s/%s.mp3" % [AUDIO_DIR, track_name]
	if not ResourceLoader.exists(path):
		push_warning("Missing music track: " + path)
		return
	var stream := load(path) as AudioStream
	if stream == null:
		push_warning("Unable to load music track: " + path)
		return
	current_track = track_name
	player.stream = stream
	player.play()

func set_muted(value: bool) -> void:
	if GameState.music_muted == value:
		return
	GameState.music_muted = value
	GameState.save_game()
	_apply_mute()
	mute_changed.emit(value)

func toggle_muted() -> void:
	set_muted(not GameState.music_muted)

func is_muted() -> bool:
	return GameState.music_muted

func _apply_mute() -> void:
	# Keep playback running silently so unmuting resumes immediately.
	player.volume_db = -80.0 if GameState.music_muted else linear_to_db(PLAYBACK_VOLUME)

func _on_track_finished() -> void:
	if player.stream != null:
		player.play()
