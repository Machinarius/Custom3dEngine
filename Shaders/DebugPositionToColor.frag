#version 330 core

// The coordinate this fragment should map to
in vec3 fSourcePos;

uniform sampler2D uTexture;

out vec4 outColor;

void main() {
  vec4 texColor = vec4(normalize(fSourcePos), 1.0);
  outColor = texColor;
}

