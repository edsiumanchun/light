Shader "Custom/BrightLaserIlluminated"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Emission ("Emission", Color) = (0,0,0,1)
        _Width ("Width", Range(0.01, 2.0)) = 0.1 // Increased maximum width to 2.0
        _Intensity ("Intensity", Range(0.0, 20.0)) = 4.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Blend One One

        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _Emission;
        half _Width;
        half _Intensity;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            
            // Brighter laser-like effect with larger width
            half dist = 1.0 - abs(IN.uv_MainTex.y - 0.5) / _Width;
            o.Emission = _Emission.rgb * pow(dist, _Intensity) * 10.0; // Increased emission intensity
            o.Alpha = dist;
        }
        ENDCG
    }
    FallBack "Diffuse"
}