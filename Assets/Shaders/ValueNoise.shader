Shader "Noise/ValueNoise"
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
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile RENDERTYPE_1D RENDERTYPE_2D RENDERTYPE_3D
			#pragma multi_compile RANGE_ADJUST RANGE_NOADJUST

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

			//vertex to fragment
			struct VertexOutput
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 uvw : UVW;
			};

			//Spline Interpolation using _Splerp Texture
			float splerp(float start, float end, float t)
			{
				return start + (end - start) * tex2D(_Splerp, t);
			}

			VertexOutput vert(VertexInput IN)
			{
				VertexOutput OUT;
				OUT.position = UnityObjectToClipPos(IN.localPosition);
				OUT.uv = IN.uv;
				OUT.uvw = IN.localPosition * _Resolution * _Frequency;
				OUT.uvw = mul(_WorldTransform, OUT.uvw);
				return OUT;
			}

			float4 frag(VertexOutput IN) : SV_Target
			{
				//Calculate Integer Lattice Points and interpolants t
				float3 i0 = floor(IN.uvw);
				float3 i1 = i0 + 1;
				float3 t = IN.uvw - i0;
				t.x = tex2D(_Splerp, t.x);
#if defined(RENDERTYPE_2D) || defined(RENDERTYPE_3D)
				t.y = tex2D(_Splerp, t.y);
#endif
#if defined(RENDERTYPE_3D)
				t.z = tex2D(_Splerp, t.z);
#endif
				//Move Integer Lattice Points between 0 and 1 for HashTexture lookup
				i0 /= 256;
				i1 /= 256;

				//Get hash value at lattice points
				float h0 = tex2D(_PerlinHash, i0.x).r;
				float h1 = tex2D(_PerlinHash, i1.x).r;

				#if defined(RENDERTYPE_2D) || defined(RENDERTYPE_3D)
					float h00 = tex2D(_PerlinHash, h0 + i0.y);
					float h01 = tex2D(_PerlinHash, h0 + i1.y);
					float h10 = tex2D(_PerlinHash, h1 + i0.y);
					float h11 = tex2D(_PerlinHash, h1 + i1.y);
				#endif
				#if defined(RENDERTYPE_3D)
					float h000 = tex2D(_PerlinHash, h00 + i0.z);
					float h001 = tex2D(_PerlinHash, h00 + i1.z);
					float h010 = tex2D(_PerlinHash, h01 + i0.z);
					float h011 = tex2D(_PerlinHash, h01 + i1.z);
					float h100 = tex2D(_PerlinHash, h10 + i0.z);
					float h101 = tex2D(_PerlinHash, h10 + i1.z);
					float h110 = tex2D(_PerlinHash, h11 + i0.z);
					float h111 = tex2D(_PerlinHash, h11 + i1.z);
				#endif
				#if defined(RENDERTYPE_1D)	
					float noise = lerp(h0, h1, t.x);
				#endif
				#if defined(RENDERTYPE_2D)
					float noise = lerp(lerp(h00, h10, t.x), lerp(h01, h11, t.x), t.y);
				#endif
				#if defined(RENDERTYPE_3D)
					float noise = lerp(
						lerp(
							lerp(h000, h100, t.x),
							lerp(h010, h110, t.x),
							t.y),
						lerp(
							lerp(h001, h101, t.x),
							lerp(h011, h111, t.x),
							t.y),
						t.z);
				#endif
				#if defined(RANGE_ADJUST)
					noise = (noise - 0.5) * 2;
				#endif
				return _Amplitude * float4(noise, noise, noise, 1);
			}
			ENDCG
		}
	}
}
