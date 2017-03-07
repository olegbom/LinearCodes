#version 330
 
uniform mat3x2 projMatrix;
uniform mat3x2 modelMatrix;

in vec2 position;

 
mat3x2 Mul(mat3x2 l, mat3x2 r)
{
	return mat3x2(l[0][0]*r[0][0] + l[1][0]*r[0][1],
			      l[0][1]*r[0][0] + l[1][1]*r[0][1],
				  l[0][0]*r[1][0] + l[1][0]*r[1][1],
				  l[0][1]*r[1][0] + l[1][1]*r[1][1],
				  l[0][0]*r[2][0] + l[1][0]*r[2][1] + l[2][0],
				  l[0][1]*r[2][0] + l[1][1]*r[2][1] + l[2][1]);
}
 
void main()
{
	vec2 pos = Mul(projMatrix,modelMatrix) * vec3(position, 1.0);
    gl_Position = vec4(pos.x, pos.y, 0.0, 1.0);
}