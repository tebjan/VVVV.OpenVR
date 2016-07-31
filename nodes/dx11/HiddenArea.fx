//@author: vux
//@help: standard constant shader
//@tags: color
//@credits: 
 
cbuffer cbPerDraw : register( b0 )
{
	float4x4 tP : PROJECTION;
};


cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float Alpha <float uimin=0.0; float uimax=1.0;> = 1; 
	float4 cAmb <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
};

struct VS_IN
{
	float4 PosO : POSITION;
};

struct vs2ps
{
    float4 PosWVP: SV_POSITION;
};

vs2ps VS(VS_IN input)
{
    vs2ps Out = (vs2ps)0;
    Out.PosWVP  = mul(input.PosO, tW);
    return Out;
}

float4 PS(vs2ps In): SV_Target
{
    return cAmb;
}





technique10 Constant
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}




