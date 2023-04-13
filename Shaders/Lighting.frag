#version 330 core

out vec4 fragmentColor;

in vec3 fNormal;
in vec3 fPos;
in vec2 fUv;

struct Material {
  sampler2D diffuse;
  sampler2D specular;
  float shininess;
};

struct LightSource {
  vec3 position;
  vec3 ambient;
  vec3 diffuse;
  vec3 specular;
};

uniform Material material;
uniform LightSource light;
uniform vec3 cameraPos;

void main() {
  vec3 ambient = light.ambient * texture(material.diffuse, fUv).rgb;

  vec3 normal = normalize(fNormal);
  vec3 lightDirection = normalize(light.position - fPos);
  float diffuseFactor = max(dot(normal, lightDirection), 0.0);
  vec3 diffuse = light.diffuse * (diffuseFactor * texture(material.diffuse, fUv).rgb);

  vec3 viewDirection = normalize(cameraPos - fPos);
  vec3 reflectDirection = reflect(-lightDirection, normal);
  float specularFactor = pow(max(dot(viewDirection, reflectDirection), 0.0), material.shininess);
  vec3 specular = light.specular * (specularFactor * texture(material.specular, fUv).rgb);

  vec3 result = ambient + diffuse + specular;
  fragmentColor = vec4(result, 1.0);
}
