Shader "Custom/FogShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_FogColor ("Fog color", Color) = (1,1,1,1)
		_FogStart ("Fog start", float) = 0
		_FogEnd ("Fog end", float) = 100
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members zWorld)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float3 texcoord : TEXCOORD0;
			};
			
			fixed4 	_FogColor;
			float 	_FogStart;
			float 	_FogEnd;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
        OUT.texcoord.x = IN.texcoord.x;
        OUT.texcoord.y = IN.texcoord.y;
        OUT.texcoord.z = OUT.vertex.z;
				OUT.color = IN.color;
	
//				OUT.zWorld = OUT.vertex.z;
	
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
        float2 tex;
        tex.x = IN.texcoord.x;
        tex.y = IN.texcoord.y;
				fixed4 originalColor = tex2D(_MainTex, IN.texcoord) * IN.color;
				fixed4 outputColor;
				
//				outputColor = originalColor * originalColor.a;
				
//				float weight = (50.0 - 50*OUT.vertex.z) / (100);
//				OUT.color = lerp (IN.color, _FogColor, weight);
        float weight = clamp ((IN.texcoord.z - _FogStart) / (_FogEnd - _FogStart), 0.0, 1.0);
				outputColor = lerp (originalColor * originalColor.a, _FogColor * originalColor.a, weight);
				
				outputColor.a = originalColor.a;
				
//				float weight = _FogStart; //(50.0 - _FogStart) / (_FogEnd - _FogStart);
				
				return outputColor;
			}
		ENDCG
		}
	}
}
