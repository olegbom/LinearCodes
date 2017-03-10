﻿#version 330 core
layout (location = 0) in vec4 vertex; // <vec2 position, vec2 texCoords>

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 projection;
uniform vec4 texturePos; //<x, y, width, height>

void main()
{
    TexCoords = texturePos.xy + vertex.zw * texturePos.zw;
    gl_Position = projection * model * vec4(vertex.xy, 0.0, 1.0);
}