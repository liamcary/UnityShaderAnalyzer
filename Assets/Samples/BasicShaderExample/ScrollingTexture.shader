Shader "Examples/ScrollingTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
        _ScalarY("Y Scalar", Float) = 1.0
        _ScrollSpeed ("Scroll Speed", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" "IgnoreProjector"="True" }

        Pass
        {
            Cull Back
            ZWrite Off
            ZTest On
            Lighting Off

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform float4 _Tint;
            uniform float _ScalarY;
            uniform float _ScrollSpeed;

            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
               
                v2f o;

                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 uv = float2 (sin(i.worldPos.x * 10), i.worldPos.y *_ScalarY + _Time.y * _ScrollSpeed);
                fixed4 col = tex2D(_MainTex, uv) * _Tint;

                return col;
            }
            ENDCG
        }
    }
}
