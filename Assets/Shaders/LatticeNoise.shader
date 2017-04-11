Shader "Noise/LatticeNoise"
{
	Properties
	{
		[PerRendererData]_Splerp("Splerp", 2D) = "white" {}
		[PerRendererData]_PerlinHash("PerlinHash", 2D) = "white" {}

		[PerRendererData]_Resolution("Resolution", Float) = 1
		[PerRendererData]_Frequency("Frequency", Float) = 1
		[PerRendererData]_Amplitude("Amplitude", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile RENDERTYPE_1D RENDERTYPE_2D RENDERTYPE_3D

			#include "UnityCG.cginc"

			uniform sampler2D _Splerp;
			uniform sampler2D _PerlinHash;
			uniform float4x4 _WorldTransform;
			uniform float _Resolution;
			uniform float _Frequency;
			uniform float _Amplitude;

			struct VertexInput
			{
				float4 localPosition : POSITION;
				float2 uv : TEXCOORD0;
			};

			// vertex to fragment
			struct VertexOutput 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 uvw : UVW;
			};

			VertexOutput vert (VertexInput IN)
			{
				VertexOutput OUT;
				OUT.position = UnityObjectToClipPos(IN.localPosition);
				OUT.uv = IN.uv;
				OUT.uvw = IN.localPosition * _Resolution * _Frequency;
				OUT.uvw = mul(_WorldTransform, OUT.uvw);
				return OUT;
			}
			
			float4 frag (VertexOutput IN) : SV_Target
			{
				//Calculate Integer Lattice Points
				float3 i0 = floor(IN.uvw);
				//Move Integer Lattice Points between 0 and 1 for HashTexture lookup
				i0 /= 256;

				#if defined(RENDERTYPE_1D)
					float noise = tex2D(_PerlinHash, i0.x).r;
				#endif
				#if defined(RENDERTYPE_2D)
					float noise = tex2D(_PerlinHash, tex2D(_PerlinHash, i0.x).r + i0.y).r;
				#endif
				#if defined(RENDERTYPE_3D)
					float noise = tex2D(_PerlinHash, tex2D(_PerlinHash, tex2D(_PerlinHash, i0.x).r + i0.y).r + i0.z).r;
				#endif
				return _Amplitude * float4(noise, noise, noise, 1);
			}
			ENDCG
		}
	}
}
