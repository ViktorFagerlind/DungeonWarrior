Shader "Custom/LightUpShader_02"
{
  Properties
  {
    [PerRendererData] _MainTex ("Sprite Texture", 2D)     = "white" {}
                      _CelRamp ("Cel shading ramp", 2D)   = "white" {}
                      _Color ("Tint", Color) = (1,1,1,1)
  }

  SubShader
  {
    Tags
    { 
      "Queue"             = "Transparent"
      "IgnoreProjector"   = "False"
      "RenderType"        = "Transparent"
      "PreviewType"       = "Plane"
      "CanUseSpriteAtlas" = "True"
    }

    Cull Off
    Lighting On
    ZWrite Off
    Fog { Mode Off }
    Blend SrcAlpha OneMinusSrcAlpha

    CGPROGRAM
    #pragma surface surf CustomLambert alpha vertex:vert

    sampler2D _MainTex;
    fixed4    _Color;

    struct Input
    {
      float2 uv_MainTex;
      fixed4 color;
    };

    half4 LightingCustomLambert (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) 
    {
      half NdotL = dot (s.Normal, lightDir);
      half4 c;
      c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * 
      atten * 3);
      c.a   = s.Alpha;
      return c;
    }

    void vert (inout appdata_full v)
    {
      v.normal  = float3(0,0,-1);
      v.tangent = float4(-1, 0, 0, 1);
    }

    void surf (Input IN, inout SurfaceOutput o)
    {
      fixed4 c  = tex2D(_MainTex, IN.uv_MainTex) * _Color;
      o.Albedo  = c.rgb;
      o.Alpha   = c.a;
    }
    ENDCG
  }

}