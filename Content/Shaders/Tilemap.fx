#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;	
};

matrix projection;
matrix view;

float4 ambientColor;

struct VertexShaderOutput
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput SimVertShader(float4 inPos : POSITION, float2 inTexCoord : TEXCOORD0)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	matrix mvMatrix = mul(view, projection);
	output.Position = mul(inPos, mvMatrix);
	output.TexCoord = inTexCoord;

	return output;
}

float4 SimPixShader(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(SpriteTextureSampler, input.TexCoord);
	return color * ambientColor;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL SimVertShader();
		PixelShader = compile PS_SHADERMODEL SimPixShader();
	}
};