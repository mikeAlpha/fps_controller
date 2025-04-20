Shader "Custom/FoliageShader"
{
    Properties
    {
        _DiffuseTex ("Diffuse Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        Cull Off // Two-Sided Rendering

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            StructuredBuffer<float4> _PositionBuffer;
            sampler2D _DiffuseTex, _AlphaTex;
            float _AlphaThreshold;

            struct appdata
            {
                uint instanceID : SV_InstanceID; // Get instance ID
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata IN)
            {
                v2f OUT;

                // Get position from buffer
                float4 instancePos = _PositionBuffer[IN.instanceID];

                // Apply instance position to vertex
                OUT.pos = UnityObjectToClipPos(IN.vertex + instancePos);
                OUT.uv = IN.uv;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_DiffuseTex, IN.uv);
                float alpha = tex2D(_AlphaTex, IN.uv).r;

                if (alpha < _AlphaThreshold) discard; // Alpha clipping

                return col;
            }
            ENDCG
        }
    }
}
