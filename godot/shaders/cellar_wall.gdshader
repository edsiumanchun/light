shader_type canvas_item;
render_mode unshaded;

uniform vec2 room_size = vec2(1048.0, 510.0);

float grain(vec2 point) {
	return fract(52.9829189 * fract(dot(point, vec2(0.06711056, 0.00583715))));
}

void fragment() {
	vec2 point = UV * room_size;
	vec2 courses = point / vec2(92.0, 35.0);
	courses.x += mod(floor(courses.y), 2.0) * 0.5;
	vec2 joint = fract(courses);
	vec2 edge = min(joint, 1.0 - joint);
	float face = smoothstep(0.018, 0.042, edge.x) * smoothstep(0.035, 0.095, edge.y);
	float variation = grain(mod(floor(courses), vec2(8.0, 6.0)));
	float texture_grain = (grain(floor(point)) - 0.5) * 0.025;
	float bevel = 0.05 * (1.0 - smoothstep(0.05, 0.22, joint.y));
	vec3 brick = vec3(0.22, 0.17, 0.13) + variation * 0.045 + texture_grain + bevel;
	vec3 wall = mix(vec3(0.085, 0.06, 0.048), brick, face);

	float post = mod(point.x, 262.0);
	float timber_face = 1.0 - smoothstep(19.0, 23.0, post);
	float fibers = sin(post * 2.1 + sin(point.y * 0.06)) * 0.013;
	vec3 timber = vec3(0.25, 0.139, 0.079) + fibers - post * 0.003;
	wall = mix(wall, timber, timber_face);

	float light = smoothstep(0.32, 0.9, UV.y);
	COLOR = vec4(wall * mix(vec3(0.47, 0.46, 0.45), vec3(1.05, 0.87, 0.72), light), 1.0);
}
