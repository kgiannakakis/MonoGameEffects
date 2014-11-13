float4x4 World;
float4x4 View;
float4x4 Projection;
texture BasicTexture;

sampler BasicTextureSampler = sampler_state {
	texture = <BasicTexture>;
	MinFilter = Anisotropic; 
	MagFilter = Anisotropic; 
	MipFilter = Linear; 
	AddressU = Wrap; 
	AddressV = Wrap; 
};

struct VertexShaderInput
{
    float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.UV = input.UV;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 tex = tex2D(BasicTextureSampler, input.UV);
    tex.rgb = dot(tex.rgb, float3(0.3, 0.59, 0.11));
    return tex;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}