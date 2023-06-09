#version 330 core

// The coordinate this fragment should map to
in vec2 fUv;

uniform sampler2D uTexture;

out vec4 outColor;

void main() {
  vec4 texColor = texture(uTexture, fUv);
  if (texColor.a < 0.1) {
    discard;
  }
  outColor = texColor;
}

