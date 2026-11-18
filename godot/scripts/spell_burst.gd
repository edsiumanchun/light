extends Node2D

const CHARGE_SECONDS := 0.42
const BURST_SECONDS := 1.25
const WIND_COLOR := Color("#76f1ff")
const GOLD_COLOR := Color("#ffda88")

var elapsed := 0.0
var streaks: Array[Vector4] = []
var light: PointLight2D

func _ready() -> void:
	z_index = 12
	var additive := CanvasItemMaterial.new()
	additive.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
	additive.light_mode = CanvasItemMaterial.LIGHT_MODE_UNSHADED
	material = additive

	var gradient := Gradient.new()
	gradient.colors = PackedColorArray([Color.WHITE, Color(1.0, 0.9, 0.7, 0.0)])
	var glow := GradientTexture2D.new()
	glow.width = 256
	glow.height = 256
	glow.gradient = gradient
	glow.fill = GradientTexture2D.FILL_RADIAL
	glow.fill_from = Vector2(0.5, 0.5)
	glow.fill_to = Vector2(1.0, 0.5)
	light = PointLight2D.new()
	light.texture = glow
	light.color = Color("#ffe3ad")
	light.energy = 0.0
	add_child(light)

	var random := RandomNumberGenerator.new()
	random.randomize()
	for index in range(88):
		streaks.append(Vector4(
			random.randf_range(-1.35, 1.35), random.randf_range(100.0, 365.0),
			random.randf_range(0.0, 0.32), random.randf_range(0.8, 3.3)
		))

func play() -> Tween:
	var tween := create_tween()
	tween.tween_method(_set_elapsed, 0.0, CHARGE_SECONDS + BURST_SECONDS, CHARGE_SECONDS + BURST_SECONDS)
	tween.tween_callback(queue_free)
	return tween

func _set_elapsed(value: float) -> void:
	elapsed = value
	var burst := elapsed - CHARGE_SECONDS
	light.energy = pow(elapsed / CHARGE_SECONDS, 2.0) * 0.85 if burst < 0.0 else 1.5 * exp(-burst * 4.5)
	queue_redraw()

func _draw() -> void:
	var burst := elapsed - CHARGE_SECONDS
	var charge := clampf(elapsed / CHARGE_SECONDS, 0.0, 1.0)
	var pulse := charge if burst < 0.0 else exp(-burst * 4.3)
	# Soft falloff around a tight white-gold center, even on unlit sprites.
	for layer in range(8, 0, -1):
		draw_circle(Vector2.ZERO, layer * (5.0 + pulse * 6.0), _tint(GOLD_COLOR, pulse * 0.07), true, -1.0, true)
	draw_circle(Vector2.ZERO, 3.0 + pulse * 11.0, _tint(Color.WHITE, pulse), true, -1.0, true)
	if burst < 0.0:
		for index in range(4):
			var angle := index * TAU / 4.0 + elapsed * 11.0
			var radius := lerpf(46.0, 13.0, charge)
			draw_arc(Vector2.ZERO, radius, angle, angle + 0.85, 12, _tint(WIND_COLOR, charge * 0.7), 2.0, true)
		return

	for index in range(3):
		var wave := (burst - index * 0.09) / 0.70
		if wave < 0.0 or wave >= 1.0:
			continue
		var radius := 12.0 + 115.0 * ease(wave, 0.6)
		draw_set_transform(Vector2(radius * 0.87, 0.0), 0.0, Vector2(0.30, 1.0))
		draw_arc(Vector2.ZERO, radius, -2.0, 2.0, 36, _tint(WIND_COLOR, (1.0 - wave) * 0.75), 3.0, true)
		draw_set_transform(Vector2.ZERO)

	for index in range(8):
		var travel := clampf(burst / 0.98, 0.0, 1.0)
		var points := PackedVector2Array()
		var colors := PackedColorArray()
		for step in range(22):
			var trail := 1.0 - step / 21.0
			var distance := maxf(0.0, ease(travel, 0.65) - trail * 0.44) * (170.0 + index * 16.0)
			var phase := index * TAU / 8.0 + distance * 0.032 - burst * 7.0
			var y := sin(phase) * minf(distance * 0.55, 43.0)
			points.append(Vector2(distance, y))
			colors.append(_tint(WIND_COLOR, sin(trail * PI) * (1.0 - travel) * 0.72))
		draw_polyline_colors(points, colors, 2.5, true)

	for index in range(streaks.size()):
		var streak := streaks[index]
		var age := burst - streak.z
		if age < 0.0 or age >= 0.90:
			continue
		var direction := Vector2.from_angle(streak.x)
		var distance := streak.y * (1.0 - exp(-age * 2.8)) * 0.8
		var head := direction * distance
		head.y += sin(distance * 0.07 + index) * minf(distance * 0.2, 9.0)
		var opacity := (1.0 - age / 0.90) * minf(age * 25.0, 1.0)
		var color := GOLD_COLOR if index % 4 == 0 else WIND_COLOR
		draw_line(head - direction * (6.0 + streak.w * 7.0), head, _tint(color, opacity), streak.w, true)
		draw_circle(head, streak.w * 0.75, _tint(Color.WHITE, opacity), true, -1.0, true)

func _tint(color: Color, opacity: float) -> Color:
	return Color(color, opacity)
