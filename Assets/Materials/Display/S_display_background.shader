Shader "Custom/S_display_background"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
        _GridEnabled("Grid Enabled", Float) = 1
        _EmissiveStrength("Emissive Strength", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : NORMAL;
                float3 positionWS : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _GridEnabled;
                float _EmissiveStrength;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                if (_GridEnabled < 0.5)
                {
                    return half4(0,0,0,1);
                }

                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

                float3 normalWS = normalize(IN.normalWS);
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float NdotL = saturate(dot(normalWS, -lightDir));
                float3 litColor = color.rgb * (mainLight.color.rgb * NdotL);

                float2 gridUV = IN.uv;
                float gridSpacing = 0.1;
                float gridThickness = 0.01;

                float2 grid = abs(frac(gridUV / gridSpacing) - 0.5);
                float gridLine = step(grid.x, gridThickness) + step(grid.y, gridThickness);
                float lineIntensity = gridLine * 0.2;

                // CRT vignette: brighter in center, fades at edges
                float2 center = gridUV - 0.5;
                float dist = length(center) / 0.7071;
                float vignette = 1.0 - smoothstep(0.0, 1.0, dist);

                // Apply vignette to grid lines
                float fadedLineIntensity = lineIntensity * vignette;

                float3 gridEmissive = float3(0.2, 0.8, 0.2) * fadedLineIntensity * _EmissiveStrength;
                float3 finalColor = lerp(litColor, float3(0.2, 0.8, 0.2), fadedLineIntensity);
                float3 colorOut = finalColor + gridEmissive;

                // Subtle CRT green glow: faint green background, strongest in center
                float greenGlow = 0.07 * vignette;
                colorOut += float3(0.15, 0.35, 0.15) * greenGlow;

                return half4(colorOut, 1);
            }
            ENDHLSL
        }
    }
}