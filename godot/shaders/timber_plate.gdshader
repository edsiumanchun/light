shader_type canvas_item;
render_mode unshaded;

uniform bool vertical_post = false;

void fragment() {
	vec2 wood_uv = vertical_post ? UV.yx : UV;
	float edge = wood_uv.y;
	float fiber = sin(wood_uv.y * 76.0 + 0.8 * sin(wood_uv.x * 13.0));
	float grain = 0.03 * fiber + 0.025 * sin(wood_uv.y * 151.0 + wood_uv.x * 8.0);
	vec3 top = vec3(0.47, 0.32, 0.23);
	vec3 face = vec3(0.31, 0.19, 0.14);
	vec3 shade = vec3(0.14, 0.09, 0.075);
	vec3 color = mix(top, face, smoothstep(0.08, 0.24, edge));
	color = mix(color, shade, smoothstep(0.61, 0.97, edge));
	float joint = 1.0 - smoothstep(0.015, 0.045, min(wood_uv.x, 1.0 - wood_uv.x));
	float trim = smoothstep(0.31, 0.38, edge) * (1.0 - smoothstep(0.44, 0.50, edge));
	color += vec3(0.035, 0.022, 0.016) * trim;
	COLOR = vec4(color * (1.0 + grain - (vertical_post ? 0.18 : 0.04) * joint), 1.0);
}
