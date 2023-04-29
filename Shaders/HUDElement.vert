#version 330 core

uniform mat4 uProjection;
uniform mat4 uModel;

layout(location = 0) in vec3 aPosition;

void main() { 
  gl_Position = uProjection * uModel * vec4(aPosition.xy, 0.0, 1.0);
}
