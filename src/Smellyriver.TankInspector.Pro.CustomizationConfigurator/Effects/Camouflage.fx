sampler2D input : register(s0);
float4 baseColor : register(c0);
float4 color1 : register(c1);
float4 color2 : register(c2);
float4 color3 : register(c3);
float4 color4 : register(c4);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(input, uv);
	float4 result;


	result.rgba = color1 * color.r * color1.a
		+ color2 * color.g * color2.a
		+ color3 * color.b * color3.a
		+ color4 * color.a * color4.a;

	result.rgb = baseColor.rgb * (1 - result.a) + result.rgb;
	result.a = 1.0f;

	return result;
}