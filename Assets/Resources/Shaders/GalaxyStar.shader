Shader "Galaxy/GalaxyStar"
{
	Properties
	{
		_MainTex ("Texture Image", 2D) = "white" {}
      _Color ("Color", Color) = (1,1,1,1)
      _FadeDistance("Fade Distance", Float) = 500
      _FadeRangeFactor("Fade Range Factor", Float) = 2000
		_Scaling("Scaling", Float) = 1
    _ScalingRange("Scaling range", Float) = 250
		[Enum(RenderOnTop, 0,RenderWithTest, 4)] _ZTest("Render on top", Int) = 1
	}
	SubShader
	{
		Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Fade" "DisableBatching" = "True" }

		ZWrite On
		ZTest [_ZTest]
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
        CGPROGRAM

         #pragma vertex vert
         #pragma fragment frag

         uniform sampler2D _MainTex;
		 int _KeepConstantScaling;
		 float _Scaling;
		 float _FadeDistance;
		 float _FadeRangeFactor;
     float _ScalingRange;
       fixed4 _Color;
       float dist;

		struct appdata
		{
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float4 worldPos : TEXCOORD1;
         };

         struct v2f 
		 {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 worldPos : TEXCOORD1;
         };
 
         v2f vert(appdata v)
         {
            v2f o;
            dist = distance(mul(unity_ObjectToWorld, v.vertex), _WorldSpaceCameraPos);
			   float relativeScaler = 1;
               if(dist > _ScalingRange)
               {
                    relativeScaler = 1/(1/(dist/_ScalingRange));
               }
            o.vertex = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0)) + float4(v.vertex.x, v.vertex.y, 0.0, 0.0) * relativeScaler * _Scaling);
            o.uv = v.uv;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            return o;
         }
 
         float4 frag(v2f i) : SV_Target
         {
            float dist = length(i.worldPos.xyz - _WorldSpaceCameraPos.xyz);
            if(dist > _FadeDistance)
            {
            _Color.a = 1/((1+dist/_FadeRangeFactor) * ((1+dist/_FadeRangeFactor)));
            }
            fixed4 col = tex2D(_MainTex, float2(i.uv)) * _Color;
            return col;
         }

         ENDCG
      }
   }
}