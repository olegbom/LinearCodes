#version 330 core
layout (location = 0) in vec4 vertex; // <vec2 position, vec2 texCoords>

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 projection;
uniform vec4 texturePos; //<x, y, width, height>

void main()
{
    TexCoords = vertex.zw;
    gl_Position = projection * model * vec4(texturePos.xy + vertex.xy*texturePos.zw, 0.0, 1.0);
}