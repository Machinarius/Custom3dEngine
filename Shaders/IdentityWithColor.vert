#version 330 core

// Inputs given to this specific shader
layout (location = 0) in vec3 vPosition;
// This will be manipulated here, then passed on to the
// fragment shader. Fragment shaders can not receive arguments
// directly.
layout (location = 1) in vec4 vColor;

// Uniform given to all shaders
uniform float uBlue;

// Output given to the Fragment shader
out vec4 fColor;

void main() {
  gl_Position = vec4(vPosition, 1.0);
  // This is using three techniques:
  // - .rb is "swizzling". It is an alias to .xz, extracting a vec2 with the R and B components of the color
  // - / 2 is divding the resulting vec2 by 2
  // - The resulting vector is "spread" unto the 4 coordinates required for a vec4
  //   meaning that the halved Red and Blue channels become the new Red and Green
  //   channels for this new color
  vec4 color = vec4(vColor.rb / 2, uBlue, vColor.a);
  fColor = color;
}
