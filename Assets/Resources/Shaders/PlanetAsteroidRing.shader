Shader "Galaxy/Planets/PlanetAsteroidRing"
{
	Properties
	{
      _Color ("Color", Color) = (1,1,1,1)
      _MainTex ("Albedo (RGB)", 2D) = "white" {}
      _DensityMap ("Density Map", 2D) = "white" {}
      _Glossiness ("Smoothness", Range(0,1)) = 0.5
      _Metallic ("Metallic", Range(0,1)) = 0.0
      _MinimumRenderDistance ("Minimum Render Distance", float) = 10
      _MaximumRenderDistance ("Maximum Render Distance", float) = 20
      _InnerRingDiameter ("Inner Ring Diameter", Range(0,1)) = 0.5
	}
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _DensityMap;
            fixed4 _Color;
            float4 _MainTex_ST;

            float _MinimumRenderDistance;
            float _MaximumRenderDistance;

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
                float distance = length(_WorldSpaceCameraPos - i.worldPos);
                // sample the texture
                fixed4 col = _Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col.r = distance < _MinimumRenderDistance ? 1 : 0;
                return col;
            }
            ENDCG
        }
    }
}
