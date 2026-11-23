shader_type canvas_item;
render_mode unshaded;

uniform int grade = 0;

void fragment() {
	vec3 color = COLOR.rgb;
	if (grade == 1) {
		color = pow(max(color, vec3(0.0)), vec3(0.58));
		color = color * vec3(1.41, 1.25, 1.04) + vec3(0.045, 0.033, 0.018);
	} else if (grade == 2) {
		float luminance = dot(color, vec3(0.21, 0.72, 0.07));
		color = mix(color, vec3(luminance), 0.56) * vec3(0.32, 0.40, 0.59);
	}
	COLOR = vec4(color, COLOR.a);
}
