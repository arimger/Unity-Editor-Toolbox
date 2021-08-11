Shader "Unlit/TestShader"
{
    Properties
    { 
        [CompactTexture]
        _MainTex ("Texture", 2D) = "white" {}
        [Vector2]
        _Vector1 ("Vector2", Vector) = (0.0, 0.0, 0.0)
        [Vector3]
        _Vector2 ("Vector3", Vector) = (0.0, 0.0, 0.0)
        [MinMaxSlider(20.0, 165.0)]
        _Vector3 ("MinMax Vector", Vector) = (50.0, 55.0, 0.0)
        [Indent(3)]
        _Float1 ("Float1", Float) = 0.0
        [Help(Custom Help Box , 1)]
        _Float2 ("Float2", Float) = 0.0
        _Float3 ("Float3", Float) = 0.0
        [Title(Custom Title, 4)]
        _Float ("Float", Float) = 0.5
        [Toggle][Space]
        _ToggleProperty ("Toggle", Int) = 0
        [ShowIfToggle(_ToggleProperty)]
        _ShowIfExample ("Texture", 2D) = "White" {}
        [HideIfToggle(_ToggleProperty)]
        _HideIfExample ("Range", Range(0, 1)) = 0.75
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #pragma shader_feature _TOGGLEPROPERTY_ON

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
