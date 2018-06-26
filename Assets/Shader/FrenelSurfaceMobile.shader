Shader "Custom/FresnelSurfaceMobile" {
	Properties {
		_InnerColor ("Inner Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
		_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
		_AmbientFactor("Ambient Factor", Range(0.0, 1.0)) = 0.5
		_ScanlineTex("Scanline Texture", 2D) = "white" {}
		_ScrollY("Scroll Y Speed", Float) = 1.0
	}
	SubShader {
		Tags 
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector" = "True" 
			"RenderType" = "Transparent"
		}

		LOD 200

		Cull Off
		Blend OneMinusDstColor One
		Lighting Off
		ZWrite Off

		Pass
		{
			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
		
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD1;
				float3 objectPos : TEXCOORD2;

				float2 uv : TEXCOORD0;
			};

			float4 _InnerColor;
			float4 _RimColor;
			float _RimPower;
			float _AmbientFactor;

			sampler2D _ScanlineTex;
			float4 _ScanlineTex_ST;
			float _ScrollY;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				half2 scanlineUV = half2(0, 0);
				// arctan of param0 / param1, return [- pi / 2, pi / 2]
				half angle = atan2(v.vertex.z, v.vertex.x);
				scanlineUV.x = angle / 3.1415;
				scanlineUV.y = v.vertex.y;

				o.uv = TRANSFORM_TEX(scanlineUV, _ScanlineTex) + frac(half2(0.0, _ScrollY) * _Time.y);
				
				o.objectPos = v.vertex.xyz;
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}



			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 scanline = tex2D(_ScanlineTex, i.uv);

				half viewDotN = dot(normalize(i.viewDir), i.normal);
				half upDotN = abs(dot(half3(0, 1, 0), i.normal));

				half rim = 1.0 - abs(viewDotN);
				// rim = _AmbientFactor + (1 - _AmbientFactor) * rim;

				half power = pow(rim, _RimPower);
				power = _AmbientFactor + (1 - _AmbientFactor) * power;

				fixed4 col = _RimColor * power * 2.0;
				if(upDotN < 0.9)
					col *= scanline.r;

				return col;
			}
		
		
			ENDCG
		}
		
	}
	FallBack "Transparent/VertexLit"
}