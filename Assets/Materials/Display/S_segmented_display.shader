Shader "Custom/S_segmented_display"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (0, 0, 0, 1)
        [HDR] _EmissiveColor("Emissive Color", Color) = (1, 1, 0, 1)
        _EmissiveIntensity("Emissive Intensity", Range(0, 10)) = 2
        _SegmentWidth("Segment Width", Range(0.01, 0.2)) = 0.08
        _DisplayNumber("Display Number (0-9)", Range(0, 9)) = 0
        _SegmentsActive("Segments Active", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EmissiveColor;
                float _EmissiveIntensity;
                float _SegmentWidth;
                float _DisplayNumber;
                float _SegmentsActive;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            bool isSegmentActive(int segment, int number)
            {
                // Corrected 7-segment patterns for numbers 0-9
                // Segment order: 0=top, 1=top-left, 2=top-right, 3=middle, 4=bottom-left, 5=bottom-right, 6=bottom
                bool patterns[10][7] = {
                    { true, true, true, false, true, true, true },   // 0
                    { false, false, true, false, false, true, false }, // 1
                    { true, false, true, true, true, false, true },  // 2
                    { true, false, true, true, false, true, true },  // 3
                    { false, true, true, true, false, true, false }, // 4
                    { true, true, false, true, false, true, true },  // 5
                    { true, true, false, true, true, true, true },   // 6
                    { true, false, true, false, false, true, false }, // 7
                    { true, true, true, true, true, true, true },    // 8
                    { true, true, true, true, false, true, true }    // 9
                };
                
                return patterns[number][segment];
            }

            float segmentDistance(float2 uv, int segment, float width)
            {
                float d = 1.0;
                float gap = 0.05;
                float bevel = 0.03;
                
                // Segment layout:
                //  0
                // 1 2
                //  3
                // 4 5
                //  6
                
                if (segment == 0) // Top
                {
                    float y = 0.85;
                    if (abs(uv.y - y) < width && uv.x > 0.2 && uv.x < 0.8) d = 0.0;
                    // Left bevel extends down to meet vertical
                    if (uv.x > 0.15 && uv.x < 0.25 && uv.y > y - width && uv.y < y + width + 0.02) d = 0.0;
                    // Right bevel
                    if (uv.x > 0.75 && uv.x < 0.85 && uv.y > y - width && uv.y < y + width + 0.02) d = 0.0;
                }
                else if (segment == 1) // Top-left
                {
                    // Connect from top bevel down through middle gap
                    if (abs(uv.x - 0.15) < width && uv.y > (0.5 + gap) && uv.y < 0.87) d = 0.0;
                }
                else if (segment == 2) // Top-right
                {
                    if (abs(uv.x - 0.85) < width && uv.y > (0.5 + gap) && uv.y < 0.87) d = 0.0;
                }
                else if (segment == 3) // Middle
                {
                    float y = 0.5;
                    if (abs(uv.y - y) < width && uv.x > 0.2 && uv.x < 0.8) d = 0.0;
                    // Left bevel
                    if (uv.x > 0.15 && uv.x < 0.25 && uv.y > y - width - 0.02 && uv.y < y + width + 0.02) d = 0.0;
                    // Right bevel
                    if (uv.x > 0.75 && uv.x < 0.85 && uv.y > y - width - 0.02 && uv.y < y + width + 0.02) d = 0.0;
                }
                else if (segment == 4) // Bottom-left
                {
                    // Connect from middle bevel down to bottom bevel
                    if (abs(uv.x - 0.15) < width && uv.y > 0.13 && uv.y < (0.5 - gap)) d = 0.0;
                }
                else if (segment == 5) // Bottom-right
                {
                    if (abs(uv.x - 0.85) < width && uv.y > 0.13 && uv.y < (0.5 - gap)) d = 0.0;
                }
                else if (segment == 6) // Bottom
                {
                    float y = 0.15;
                    if (abs(uv.y - y) < width && uv.x > 0.2 && uv.x < 0.8) d = 0.0;
                    // Left bevel
                    if (uv.x > 0.15 && uv.x < 0.25 && uv.y > y - width - 0.02 && uv.y < y + width) d = 0.0;
                    // Right bevel
                    if (uv.x > 0.75 && uv.x < 0.85 && uv.y > y - width - 0.02 && uv.y < y + width) d = 0.0;
                }
                
                return d;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                int number = int(_DisplayNumber);
                
                half3 color = _BaseColor.rgb;
                half3 emissive = half3(0, 0, 0);
                
                if (_SegmentsActive > 0.5)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        float dist = segmentDistance(uv, i, _SegmentWidth);
                        if (isSegmentActive(i, number) && dist < 0.5)
                        {
                            emissive += _EmissiveColor.rgb * _EmissiveIntensity;
                        }
                    }
                    color += emissive;
                }
                
                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}