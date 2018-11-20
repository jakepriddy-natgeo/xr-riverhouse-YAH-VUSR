Shader "VR/VRReticleWorld"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_InnerDiameter("InnerDiameter", Range(0, 10.0)) = 1.5
		_OuterDiameter("OuterDiameter", Range(0.00872665, 10.0)) = 2.0
		_DistanceInMeters("DistanceInMeters", Range(0.0, 100.0)) = 2.0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"


			uniform float4 _Color;
			uniform float _InnerDiameter;
			uniform float _OuterDiameter;
			uniform float _DistanceInMeters;


			struct vertexInput {
				float4 vertex : POSITION;
			};

			struct fragmentInput {
				float4 position : SV_POSITION;
			};

			fragmentInput vert(vertexInput i) {
				float scale = lerp(_OuterDiameter, _InnerDiameter, i.vertex.z);

				float4 vert_out = float4(i.vertex.x * scale, i.vertex.y * scale, _DistanceInMeters, 1.0);

				fragmentInput o;
				o.position = UnityObjectToClipPos(vert_out);
				return o;
			}

			fixed4 frag(fragmentInput i) : SV_Target{
				fixed4 ret = _Color;
			return ret;
			}

			ENDCG
		}
	}
}
