#version 330 core

// Prefixed with f to simbolize this being an input coming from 
// the fragment shader
in vec4 fColor;

// The output of a fragment shader is the final color that fragment
// will have
out vec4 gl_Color;

void main() {
  gl_Color = fColor;
}
