Shader "Custom/DiagonalWaveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveHeight ("Wave Height", Float) = 0.5
        _WaveFrequency ("Wave Frequency", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        sampler2D _MainTex;
        float4 _Color;
        float _WaveSpeed;
        float _WaveHeight;
        float _WaveFrequency;

        struct Input
        {
            float2 uv_MainTex;
        };

        void vert (inout appdata_full v)
        {
            float wave = sin(_Time.y * _WaveSpeed + v.vertex.y * _WaveFrequency) * _WaveHeight;
            v.vertex.x += wave;
            v.vertex.z += wave; // Apply wave to both X and Z for a diagonal effect
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
