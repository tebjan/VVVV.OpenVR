//@author: vux
//@help: basic pixel based lightning with directional light
//@tags: shading, blinn
//@credits: vvvv group

// -----------------------------------------------------------------------------
// PARAMETERS:
// -----------------------------------------------------------------------------

//transforms
float4x4 tW: WORLD;        //the models world matrix
float4x4 tV: VIEW;         //view matrix as set via Renderer (EX9)
float4x4 tWV: WORLDVIEW;
float4x4 tWVP: WORLDVIEWPROJECTION;
float4x4 tP: PROJECTION;   //projection matrix as set via Renderer (EX9)
float4x4 tWIT: WORLDINVERSETRANSPOSE;

//light properties
float3 lDir <string uiname="Light Direction";> = {0, -5, 2};        //light direction in world space
float4 lAmb  <bool color=true; String uiname="Ambient Color";>  = {0.15, 0.15, 0.15, 1};
float4 lDiff <bool color=true; String uiname="Diffuse Color";>  = {0.85, 0.85, 0.85, 1};
float4 lSpec <bool color=true; String uiname="Specular Color";> = {0.35, 0.35, 0.35, 1};
float lPowerIn <String uiname="Power"; float uimin=0.0;> = 25.0;     //shininess of specular highlight

//phong directional function
float4 PhongDirectional(float3 NormV, float3 ViewDirV, float3 LightDirV, float4 SpecColor)
{
    //In.TexCd = In.TexCd / In.TexCd.w; // for perpective texture projections (e.g. shadow maps) ps_2_0

	float lPower = lPowerIn * SpecColor.r;
	
	float4 amb = float4(lAmb.rgb, 1);
    //halfvector
    float3 H = normalize(ViewDirV + LightDirV);

    //compute blinn lighting
    float3 shades = lit(dot(NormV, LightDirV), dot(NormV, H), lPower);

    float4 diff = lDiff * shades.y;
    diff.a = 1;

    //reflection vector (view space)
    float3 R = normalize(2 * dot(NormV, LightDirV) * NormV - LightDirV);

    //normalized view direction (view space)
    float3 V = normalize(ViewDirV);

    //calculate specular light
    float4 spec = pow(max(dot(R, V),0), lPower*.2) * lSpec;

    return (amb + diff) + spec;
}

Texture2D texture2d <string uiname="Diffuse Texture"; >;
Texture2D texture2dSpec <string uiname="Specular Texture"; >; 
SamplerState g_samLinear
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 tTex <bool uvspace=true; string uiname="Texture Transform";>;
float4x4 tColor <string uiname="Color Transform";>;

struct vs2ps
{
    float4 PosWVP: SV_POSITION;
    float4 TexCd : TEXCOORD0;
    float3 LightDirV: TEXCOORD1;
    float3 NormV: TEXCOORD2;
    float3 ViewDirV: TEXCOORD3;
};

// -----------------------------------------------------------------------------
// VERTEXSHADERS
// -----------------------------------------------------------------------------

vs2ps VS(
    float4 PosO: POSITION,
    float3 NormO: NORMAL,
    float4 TexCd : TEXCOORD0)
{
    //inititalize all fields of output struct with 0
    vs2ps Out = (vs2ps)0;

    //inverse light direction in view space
    Out.LightDirV = normalize(-mul(lDir, tV));
    
    //normal in view space
    Out.NormV = normalize(mul(mul(NormO, (float3x3)tWIT),(float3x3)tV).xyz);

    //position (projected)
    Out.PosWVP  = mul(PosO, tWVP);
    Out.TexCd = mul(TexCd, tTex);
    Out.ViewDirV = -normalize(mul(PosO, tWV));
    return Out;
}

// -----------------------------------------------------------------------------
// PIXELSHADERS:
// -----------------------------------------------------------------------------

float Alpha <float uimin=0.0; float uimax=1.0;> = 1;

float4 PS(vs2ps In): SV_Target
{
    float4 col = texture2d.Sample(g_samLinear, In.TexCd.xy);
	float4 colSpec = texture2dSpec.Sample(g_samLinear, In.TexCd.xy);
    col.rgb *= PhongDirectional(In.NormV, In.ViewDirV, In.LightDirV, colSpec);
    col.a *= Alpha;
    

    return mul(col, tColor);
}


// -----------------------------------------------------------------------------
// TECHNIQUES:
// -----------------------------------------------------------------------------
technique10 GouraudDirectional
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}
