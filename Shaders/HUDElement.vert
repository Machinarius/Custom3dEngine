#version 330 core

uniform mat4 uProjection;
uniform mat4 uModel;

layout(location = 0) in vec3 aPosition;
layout(location = 2) in vec2 aTexCoord;

out vec2 vTexCoord;

void main() { 
  gl_Position = uProjection * uModel * vec4(aPosition.xy, 0.0, 1.0);
  vTexCoord = aTexCoord;
}
