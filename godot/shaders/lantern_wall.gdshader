shader_type canvas_item;
render_mode unshaded;

uniform vec4 source_region;
uniform vec2 lantern_0;
uniform vec2 lantern_1;

float lantern_glow(vec2 point, vec2 center) {
	vec2 distance_to_lantern = (point - center) / vec2(0.24, 0.46);
	return 1.0 - smoothstep(0.0, 1.0, length(distance_to_lantern));
}

void fragment() {
	vec2 room_uv = (UV - source_region.xy) / source_region.zw;
	float glow = max(lantern_glow(room_uv, lantern_0), lantern_glow(room_uv, lantern_1));
	vec3 wood = COLOR.rgb;
	vec3 shadow = wood * vec3(0.38, 0.36, 0.36);
	vec3 lit = wood * vec3(1.05, 0.83, 0.61) + vec3(0.075, 0.031, 0.008);
	COLOR = vec4(mix(shadow, lit, glow), COLOR.a);
}
