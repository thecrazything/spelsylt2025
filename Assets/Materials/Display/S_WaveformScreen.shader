Shader "Custom/S_WaveformScreenShader_Lit"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _WaveAmplitude("Wave Amplitude", Float) = 0.3
        _WaveFrequency("Wave Frequency", Float) = 2.0
        _WaveType("Wave Type (0=Sine, 1=Square, 2=Triangle, 3=Sawtooth)", Float) = 0
        _WaveOffsetX("Wave Offset X", Float) = 0.0
        _WaveOffsetY("Wave Offset Y", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

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
                float4 _BaseMap_ST;
                float _WaveAmplitude;
                float _WaveFrequency;
                float _WaveType;
                float _WaveOffsetX;
                float _WaveOffsetY;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float thickness = 0.02;
                float speed = 1.0;

                float x = IN.uv.x + _WaveOffsetX;
                float y = IN.uv.y;
                float t = _Time.y;

                float phase = x * _WaveFrequency * 6.2831853 + t * speed;
                float waveY = 0.0;
                float alpha = 0.0;

                float epsilon = 0.05; // Smoothing window for transitions

                if (_WaveType < 0.5)
                {
                    // Sine wave
                    waveY = 0.5 + _WaveAmplitude * sin(phase) + _WaveOffsetY;
                    alpha = smoothstep(thickness, 0.0, abs(y - waveY));
                }
                else if (_WaveType < 1.5)
                {
                    // Smoothed square wave
                    float s = sin(phase);
                    float sq = lerp(-1.0, 1.0, smoothstep(-epsilon, epsilon, s));
                    waveY = 0.5 + _WaveAmplitude * sq + _WaveOffsetY;
                    alpha = smoothstep(thickness, 0.0, abs(y - waveY));
                }
                else if (_WaveType < 2.5)
                {
                    // Triangle wave
                    waveY = 0.5 + _WaveAmplitude * (abs(frac(phase / 6.2831853) * 2.0 - 1.0) * 2.0 - 1.0) + _WaveOffsetY;
                    alpha = smoothstep(thickness, 0.0, abs(y - waveY));
                }
                else
                {
                    // Sawtooth wave
                    float saw = frac(phase / 6.2831853);
                    float st = saw * 2.0 - 1.0;
                    float leftBlend = smoothstep(0.0, epsilon, saw);
                    float rightBlend = 1.0 - smoothstep(1.0 - epsilon, 1.0, saw);
                    float blend = leftBlend * rightBlend;
                    st = lerp(-1.0, st, blend);
                    waveY = 0.5 + _WaveAmplitude * st + _WaveOffsetY;
                    alpha = smoothstep(thickness, 0.0, abs(y - waveY));
                }

                SurfaceData surfaceData;
                surfaceData.albedo                = _BaseColor.rgb;
                surfaceData.metallic              = 0.0;
                surfaceData.specular              = 0.0;
                surfaceData.smoothness            = 0.0;
                surfaceData.normalTS              = float3(0, 0, 1);
                surfaceData.emission              = _BaseColor.rgb * alpha * 100;
                surfaceData.occlusion             = 1.0;
                surfaceData.alpha                 = alpha * _BaseColor.a;
                surfaceData.clearCoatMask         = 0.0;
                surfaceData.clearCoatSmoothness   = 0.0;

                InputData inputData = (InputData)0;
                inputData.positionWS = float3(0,0,0);
                inputData.normalWS = float3(0,0,1);
                inputData.viewDirectionWS = float3(0,0,1);

                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                return color;
            }
            ENDHLSL
        }
    }
}