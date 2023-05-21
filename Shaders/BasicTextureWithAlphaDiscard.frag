#version 330 core

// The coordinate this fragment should map to
in vec2 vTexCoord;

uniform sampler2D uTexture;

out vec4 outColor;

void main() {
  vec4 texColor = texture(uTexture, vTexCoord);
  if (texColor.a < 0.1) {
    discard;
  }
  outColor = texColor;
}

