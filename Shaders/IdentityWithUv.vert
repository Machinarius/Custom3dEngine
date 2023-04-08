#version 330 core

layout (location = 0) in vec3 vPos;
layout (location = 1) in vec2 vUv;

out vec2 fUv;

void main() {
  gl_Position = vec4(vPos, 1.0);
  // Forward the UV coords to the fragment shader
  fUv = vUv;
}
