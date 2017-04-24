Shader "Noise/PerlinNoise"
{
	Properties
	{
		[PerRendererData]_Splerp("Splerp", 2D) = "white" {}
		[PerRendererData]_PerlinHash("PerlinHash", 2D) = "white" {}
		[PerRendererData]_Gradients("Gradients", 2D) = "white" {}

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
			uniform sampler2D _Gradients;
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

			//Computes the dropoff
			float smootherstep(float t) {
				return t * t * t * (t * (t * 6 - 15) + 10);
			}

			float perm(float h) {
				return tex2D(_PerlinHash, h / 256.0).x * 256;
			}

			float grad(float h, float t) {
				return tex2D(_Gradients, h).x * t;
			}

			float grad(float h, float2 t) {
				return dot(tex2D(_Gradients, h).xy, t);
			}

			float grad(float h, float3 t) {
				return dot(tex2D(_Gradients, h).xyz, t);
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
				float3 t0 = IN.uvw - i0;
				float3 t1 = t0 - 1;

				//Get hash value at lattice points
				float h0 = perm(i0.x).r;
				float h1 = perm(i1.x).r;

				#if defined(RENDERTYPE_1D)
					float g0 = grad(h0, t0.x);
					float g1 = grad(h1, t1.x);
				#endif

				#if defined(RENDERTYPE_2D)
					float h00 = perm(h0 + i0.y);
					float h01 = perm(h0 + i1.y);
					float h10 = perm(h1 + i0.y);
					float h11 = perm(h1 + i1.y);

					float g00 = grad(h00, float2(t0.x, t0.y));
					float g01 = grad(h01, float2(t0.x, t1.y));
					float g10 = grad(h10, float2(t1.x, t0.y));
					float g11 = grad(h11, float2(t1.x, t1.y));
				#endif
				#if defined(RENDERTYPE_3D)
					float h00 = perm(h0 + i0.y);
					float h01 = perm(h0 + i1.y);
					float h10 = perm(h1 + i0.y);
					float h11 = perm(h1 + i1.y);
					float h000 = perm(h00 + i0.z);
					float h001 = perm(h00 + i1.z);
					float h010 = perm(h01 + i0.z);
					float h011 = perm(h01 + i1.z);
					float h100 = perm(h10 + i0.z);
					float h101 = perm(h10 + i1.z);
					float h110 = perm(h11 + i0.z);
					float h111 = perm(h11 + i1.z);

					float g000 = grad(h000, float3(t0.x, t0.y, t0.z));
					float g001 = grad(h001, float3(t0.x, t0.y, t1.z));
					float g010 = grad(h010, float3(t0.x, t1.y, t0.z));
					float g011 = grad(h011, float3(t0.x, t1.y, t1.z));
					float g100 = grad(h100, float3(t1.x, t0.y, t0.z));
					float g101 = grad(h101, float3(t1.x, t0.y, t1.z));
					float g110 = grad(h110, float3(t1.x, t1.y, t0.z));
					float g111 = grad(h111, float3(t1.x, t1.y, t1.z));
				#endif

				t0.x = smootherstep(t0.x);
				#if defined(RENDERTYPE_2D) || defined(RENDERTYPE_3D)
					t0.y = smootherstep(t0.y);
				#endif
				#if defined(RENDERTYPE_3D)
					t0.z = smootherstep(t0.z);
				#endif

				#if defined(RENDERTYPE_1D)	
					float noise = lerp(g0, g1, t0.x) * 2;
				#endif
				#if defined(RENDERTYPE_2D)
					float noise = lerp(lerp(g00, g10, t0.x), lerp(g01, g11, t0.x), t0.y) * 1.4142135; // * sqrt2
				#endif
				#if defined(RENDERTYPE_3D)
					float noise = lerp(
						lerp(
							lerp(g000, g100, t0.x),
							lerp(g010, g110, t0.x),
							t0.y),
						lerp(
							lerp(g001, g101, t0.x),
							lerp(g011, g111, t0.x),
							t0.y),
						t0.z);
				#endif
				#if defined(RANGE_ADJUST)
					noise = (noise / 2) + 0.5;
				#endif
				return _Amplitude * float4(noise, noise, noise, 1);
			}
			ENDCG
		}
	}
}
