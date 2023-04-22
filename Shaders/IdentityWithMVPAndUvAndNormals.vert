#version 330 core

layout (location = 0) in vec3 vPos;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec2 vUv;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec3 fSourcePos;
out vec3 fNormal;
out vec3 fPos;
out vec2 fUv;

void main() {
  // Apply the MVP matrices in the correct order!
  gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
  //We want to know the fragment's position in World space, so we multiply ONLY by uModel and not uView or uProjection
  fPos = vec3(uModel * vec4(vPos, 1.0));
  //The Normal needs to be in World space too, but needs to account for Scaling of the object
  fNormal = mat3(transpose(inverse(uModel))) * vNormal;
  //Pass the texture coordinates straight through to the fragment shader
  fUv = vUv;  
  fSourcePos = vPos;
}
