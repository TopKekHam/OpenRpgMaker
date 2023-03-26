#version 460

#vert

layout(location = 0) in vec2 vPos; 
layout(location = 1) in vec2 vUv; 
layout(location = 2) in vec4 vColor; 
layout(location = 3) in float vIsSprite;

uniform mat4 uCamera;
uniform mat4 uModel;
#var 1280, 720
uniform vec2 uScreenSize;

out vec2 fUv;
out vec4 fColor;
out float fIsSprite;

vec2 SnapToPixel(vec2 normal)
{
    vec2 pos = (normal + 1f) * .5f;
    pos = floor(pos * uScreenSize);
    return (pos / uScreenSize * 2f) - 1f;
}

void main()
{
    fUv = vUv;
    fColor = vColor; 
    fIsSprite = vIsSprite;
    
    vec4 pos = uCamera * uModel * vec4(vPos, 0, 1);
    
    //pos.xy = SnapToPixel(pos.xy);
    
    gl_Position = pos;
}

#frag

in vec4 fColor;
in vec2 fUv;
in float fIsSprite;

#var
uniform sampler2D uTexture;
#var 1, 1, 1, 1
uniform vec4 uColor; 

layout(location = 0) out vec4 oColor;

void main()
{
    
    vec4 color = fColor * uColor;

    if(fIsSprite > -1) {
        //ivec2 size = textureSize(uTexture, 0);
        //color = color * texelFetch(uTexture, ivec2(fUv * size), 0);
        color = color * texture(uTexture, fUv);
    }
    
    oColor = color;
}