#version 330 core

out vec4 gl_Color;

in vec3 fNormal;
in vec3 fPos;

uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 lightPos;
uniform vec3 cameraPos;

void main() {
  float ambientStrength = 0.1;
  vec3 ambient = ambientStrength * lightColor;

  vec3 normal = normalize(fNormal);
  vec3 lightDirection = normalize(lightPos - fPos);
  float diffuseFactor = max(dot(normal, lightDirection), 0.0);
  vec3 diffuse = diffuseFactor * lightColor;

  float specularStrength = 0.5;
  vec3 viewDirection = normalize(cameraPos - fPos);
  vec3 reflectDirection = reflect(-lightDirection, normal);
  float specularFactor = pow(max(dot(viewDirection, reflectDirection), 0.0), 32);
  vec3 specular = specularStrength * specularFactor * lightColor;

  // The resulting colour should be the amount of ambient colour + the amount of additional colour provided by the diffuse of the lamp
  vec3 result = (ambient + diffuse + specular) * objectColor;

  gl_Color = vec4(result, 1.0);
}
