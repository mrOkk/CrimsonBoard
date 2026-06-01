Shader "CrimsonBoard/EnemyDissolve"
{
    Properties
    {
        _BaseColor    ("Base Color", Color) = (1,1,1,1)
        _BaseMap      ("Base Map", 2D) = "white" {}
        _DissolveMap  ("Dissolve Map (Noise)", 2D) = "white" {}
        [Range(0,1)]
        _DissolveAmount ("Dissolve Amount", Float) = 0
        _EdgeWidth    ("Edge Width", Range(0,0.2)) = 0.05
        [HDR]
        _EdgeColor    ("Edge Color", Color) = (2,0.5,0,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "TransparentCutout"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "AlphaTest"
        }

        // ── Forward Lit ──────────────────────────────────────────────────────────
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);     SAMPLER(sampler_BaseMap);
            TEXTURE2D(_DissolveMap); SAMPLER(sampler_DissolveMap);

            CBUFFER_START(UnityPerMaterial)
                half4  _BaseColor;
                float4 _BaseMap_ST;
                float4 _DissolveMap_ST;
                float  _DissolveAmount;
                float  _EdgeWidth;
                half4  _EdgeColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 baseUV      : TEXCOORD0;
                float2 dissolveUV  : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float3 positionWS  : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
                float  fogFactor   : TEXCOORD5;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   normInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS  = posInputs.positionCS;
                OUT.positionWS  = posInputs.positionWS;
                OUT.normalWS    = normInputs.normalWS;
                OUT.baseUV      = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.dissolveUV  = TRANSFORM_TEX(IN.uv, _DissolveMap);
                OUT.shadowCoord = GetShadowCoord(posInputs);
                OUT.fogFactor   = ComputeFogFactor(posInputs.positionCS.z);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float noise = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, IN.dissolveUV).r;
                // Gamma-decode to restore full 0-1 range (counteracts sRGB→linear conversion)
                noise = pow(max(noise, 0.001), 0.4545);
                // Discard pixels below the dissolve threshold
                clip(noise - _DissolveAmount);

                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.baseUV) * _BaseColor;

                // Glow band at the dissolve edge
                float edgeMask = saturate(1.0 - (noise - _DissolveAmount) / max(_EdgeWidth, 0.001));
                baseColor.rgb  = lerp(baseColor.rgb, _EdgeColor.rgb, edgeMask);

                InputData inputData = (InputData)0;
                inputData.positionWS              = IN.positionWS;
                inputData.normalWS                = normalize(IN.normalWS);
                inputData.viewDirectionWS         = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                inputData.shadowCoord             = IN.shadowCoord;
                inputData.fogCoord                = IN.fogFactor;
                inputData.vertexLighting          = half3(0, 0, 0);
                inputData.bakedGI                 = SampleSH(inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);
                inputData.shadowMask              = half4(1, 1, 1, 1);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo     = baseColor.rgb;
                surfaceData.metallic   = 0;
                surfaceData.smoothness = 0.5;
                surfaceData.normalTS   = half3(0, 0, 1);
                surfaceData.occlusion  = 1;
                surfaceData.alpha      = 1;

                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                color.rgb = MixFog(color.rgb, IN.fogFactor);
                return color;
            }
            ENDHLSL
        }

        // ── Shadow Caster ────────────────────────────────────────────────────────
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On ZTest LEqual ColorMask 0

            HLSLPROGRAM
            #pragma vertex   shadowVert
            #pragma fragment shadowFrag
            #pragma multi_compile_shadowcaster

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_DissolveMap); SAMPLER(sampler_DissolveMap);

            CBUFFER_START(UnityPerMaterial)
                half4  _BaseColor;
                float4 _BaseMap_ST;
                float4 _DissolveMap_ST;
                float  _DissolveAmount;
                float  _EdgeWidth;
                half4  _EdgeColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 dissolveUV : TEXCOORD0;
            };

            Varyings shadowVert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                float3 posWS    = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionCS  = TransformWorldToHClip(ApplyShadowBias(posWS, normalWS, _MainLightPosition.xyz));
                OUT.dissolveUV  = TRANSFORM_TEX(IN.uv, _DissolveMap);
                return OUT;
            }

            half4 shadowFrag(Varyings IN) : SV_Target
            {
                float noise = SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, IN.dissolveUV).r;
                noise = pow(max(noise, 0.001), 0.4545);
                clip(noise - _DissolveAmount);
                return 0;
            }
            ENDHLSL
        }
    }
}
