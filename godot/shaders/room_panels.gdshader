shader_type canvas_item;
render_mode unshaded;

uniform vec2 room_size = vec2(1179.0, 300.0);
uniform float panel_width = 393.0;
uniform vec2 lantern_light = vec2(-1000.0);
uniform vec2 window_center = vec2(-1000.0);
uniform vec2 bed_feet = vec2(-1000.0);
uniform vec2 pot_feet = vec2(-1000.0);
uniform vec2 table_feet = vec2(-1000.0);

float soft_pool(vec2 p, vec2 center, vec2 radius) {
	vec2 offset = (p - center) / radius;
	return 1.0 - smoothstep(0.0, 1.0, dot(offset, offset));
}

float grain(vec2 p) {
	return fract(52.9829189 * fract(dot(p, vec2(0.06711056, 0.00583715))));
}

vec3 timber(float across, float along) {
	float bevel = 0.68 + 0.55 * smoothstep(0.0, 0.25, across)
		- 0.64 * smoothstep(0.63, 1.0, across);
	float fibers = sin(across * 76.0 + sin(along * 0.09)) * 0.04;
	return vec3(0.28, 0.17, 0.105) * (bevel + fibers);
}

void fragment() {
	vec2 p = UV * room_size;
	float floor_y = room_size.y * 0.82;
	float post_x = mod(p.x, panel_width);
	float post_width = 14.0;
	float bay = clamp((post_x - post_width) / max(panel_width - post_width, 1.0), 0.0, 1.0);
	float light = sin(bay * 3.14159) * (0.38 + 0.62 * sin(clamp(p.y / floor_y, 0.0, 1.0) * 3.14159));
	float mottling = sin(p.x * 0.085 + sin(p.y * 0.049)) * sin(p.y * 0.071) * 0.018;
	vec3 color = mix(vec3(0.31, 0.225, 0.17), vec3(0.53, 0.39, 0.27), light + mottling);
	float ceiling_shadow = smoothstep(13.0, 43.0, p.y);
	float post_shadow = smoothstep(0.0, 22.0, max(post_x - post_width, 0.0));
	color *= (0.67 + 0.33 * ceiling_shadow) * (0.65 + 0.35 * post_shadow);

	if (p.y > floor_y) {
		float depth = (p.y - floor_y) / (room_size.y - floor_y);
		float row = sqrt(depth) * 4.0;
		float seam = smoothstep(0.0, 0.10, fract(row));
		float sheen = 0.9 + 0.14 * sin(bay * 3.14159);
		color = vec3(0.33, 0.21, 0.135) * sheen * mix(0.62, 1.2, depth) * mix(0.7, 1.0, seam);
	} else if (p.y < 14.0) {
		color = timber(p.y / 14.0, p.x);
	} else if (post_x < post_width) {
		color = timber(post_x / post_width, p.y);
	} else if (p.y > floor_y - 21.0) {
		color = timber((p.y - floor_y + 21.0) / 21.0, p.x) * 0.84;
	}

	float amber = soft_pool(p, lantern_light, vec2(160.0, 105.0));
	float blue = soft_pool(p, window_center + vec2(0.0, 57.0), vec2(192.0, 120.0));
	color += vec3(0.23, 0.12, 0.035) * amber + vec3(0.025, 0.052, 0.095) * blue;
	float sill_shadow = soft_pool(p, window_center + vec2(3.0, 65.0), vec2(137.0, 19.0));
	float bed_shadow = soft_pool(p, bed_feet, vec2(164.0, 13.0));
	float pot_shadow = soft_pool(p, pot_feet, vec2(42.0, 10.0));
	float table_shadow = soft_pool(p, table_feet, vec2(62.0, 10.0));
	float floor_shadow = max(max(bed_shadow, pot_shadow), table_shadow);
	color *= 1.0 - max(sill_shadow * 0.40, floor_shadow * 0.58);

	float plaster = sin(p.x * 0.23 + sin(p.y * 0.17)) * sin(p.y * 0.19 + sin(p.x * 0.1));
	float surface_detail = (grain(floor(p)) - 0.5) * 0.018 + plaster * 0.009;
	COLOR = vec4(color + surface_detail, 1.0);
}
