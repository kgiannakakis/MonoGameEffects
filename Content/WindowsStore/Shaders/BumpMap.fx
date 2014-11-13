float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.5;

float4x4 WorldInverseTranspose;

float3 DiffuseLightDirection = float3(1, 0, 0);
float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float3 FillLightDirection = float3(0, 0, 1);
float4 FillColor = float4(1, 1, 1, 1);
float FillIntensity = 0.5;

float Shininess = 200;
float4 SpecularColor = float4(1, 1, 1, 1);    
float SpecularIntensity = 1;
float3 ViewVector = float3(1, 0, 0);

texture ModelTexture;
sampler2D textureSampler = sampler_state {
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

float BumpConstant = 1;
texture NormalMap;
sampler2D bumpSampler = sampler_state {
    Texture = (NormalMap);
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float3 Tangent : TANGENT;
    float3 Binormal : BINORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinate : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float3 Tangent : TEXCOORD2;
    float3 Binormal : TEXCOORD3;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.Normal = mul(input.Normal, (float3x3) WorldInverseTranspose);
    output.Tangent = mul(input.Tangent, (float3x3) WorldInverseTranspose);
    output.Binormal = mul(input.Binormal, (float3x3) WorldInverseTranspose);

    output.TextureCoordinate = input.TextureCoordinate;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Calculate the normal, including the information in the bump map
    float4 bump4 = BumpConstant * (tex2D(bumpSampler, input.TextureCoordinate) - float4(0.5, 0.5, 0.5, 0));
    float3 bump = bump4.xyz;
    float3 bumpNormal = input.Normal + (bump.x * input.Tangent + bump.y * input.Binormal);
    bumpNormal = normalize(bumpNormal);
    
    // Calculate the diffuse light component with the bump map normal
    float diffuseIntensity = dot(normalize(DiffuseLightDirection), bumpNormal);
    if(diffuseIntensity < 0)
        diffuseIntensity = 0;
        
    // Calculate the fill light component with the bump map normal
    float fillIntensity = dot(normalize(FillLightDirection), bumpNormal);
    if(fillIntensity < 0)
        fillIntensity = 0;        

    // Calculate the specular light component with the bump map normal
    float3 light = normalize(DiffuseLightDirection);
    float3 r = normalize(2 * dot(light, bumpNormal) * bumpNormal - light);
    float3 v = normalize(mul(normalize(float4(ViewVector, 0)), World)).xyz;
    float dotProduct = dot(r, v);

    float4 specular = SpecularIntensity * SpecularColor * max(pow(abs(dotProduct), Shininess), 0) * diffuseIntensity;

    // Calculate the texture color
    float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1;

    // Combine all of these values into one (including the ambient light)
    return saturate(textureColor * diffuseIntensity + + textureColor * fillIntensity + AmbientColor * AmbientIntensity + specular);
}

technique BumpMapped
{
    pass Pass1
    {
        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}