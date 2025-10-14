Shader "Lpk/LightModel/ToonLightBase"
{
    Properties
    {
        _BaseMap            ("Texture", 2D)                       = "white" {}
        _BaseColor          ("Color", Color)                      = (0.5,0.5,0.5,1)
        
        [Space]
        [Normal] _BumpMap   ("Normal Map", 2D)                    = "bump" {}
        _BumpScale          ("Normal Scale", Range(0, 2))         = 1.0
        
        [Space]
        _ShadowStep         ("ShadowStep", Range(0, 1))           = 0.5
        _ShadowStepSmooth   ("ShadowStepSmooth", Range(0, 1))     = 0.04
        
        [Space] 
        _SpecularStep       ("SpecularStep", Range(0, 1))         = 0.6
        _SpecularStepSmooth ("SpecularStepSmooth", Range(0, 1))   = 0.05
        [HDR]_SpecularColor ("SpecularColor", Color)              = (1,1,1,1)
        
        [Space]
        _RimStep            ("RimStep", Range(0, 1))              = 0.65
        _RimStepSmooth      ("RimStepSmooth",Range(0,1))          = 0.4
        _RimColor           ("RimColor", Color)                   = (1,1,1,1)
        
        [Space]   
        _OutlineWidth       ("OutlineWidth", Range(0.0, 1.0))     = 0.15
        _OutlineColor       ("OutlineColor", Color)               = (0.0, 0.0, 0.0, 1)

        [Space]
        [HDR]_EmissionColor ("Emission Color", Color)             = (0,0,0,0)
        _EmissionMap        ("Emission Map", 2D)                  = "white" {}
        _EmissionIntensity  ("Emission Intensity", Range(0,8))    = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            // Правильное объявление фичи для карты нормалей
            #pragma shader_feature _ _NORMALMAP
             
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_BaseMap);     SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap);     SAMPLER(sampler_BumpMap);
            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _BumpScale;
                float _ShadowStep;
                float _ShadowStepSmooth;
                float _SpecularStep;
                float _SpecularStepSmooth;
                float4 _SpecularColor;
                float _RimStepSmooth;
                float _RimStep;
                float4 _RimColor;

                float4 _EmissionColor;
                float _EmissionIntensity;
                float4 _BaseMap_ST;
                float4 _BumpMap_ST;
            CBUFFER_END

            struct Attributes
            {     
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            }; 

            struct Varyings
            {
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float4 tangentWS    : TEXCOORD2;    // w компонент содержит знак битантента
                float3 viewDirWS    : TEXCOORD3;
                float4 shadowCoord  : TEXCOORD4;
                float4 fogCoord     : TEXCOORD5;
                float3 positionWS   : TEXCOORD6;
                float4 positionCS   : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                
                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w * GetOddNegativeScale());
                output.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                output.shadowCoord = TransformWorldToShadowCoord(vertexInput.positionWS);
                output.fogCoord = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }
            
            // Правильная функция преобразования нормали из tangent space в world space
            float3 TransformTangentToWorld(float3 tangentNormal, float3 normalWS, float4 tangentWS)
            {
                // Вычисляем битангент из нормали и тангента
                float3 bitangent = cross(normalWS, tangentWS.xyz) * tangentWS.w;
                
                // Создаем матрицу преобразования из tangent space в world space
                float3x3 tangentToWorld = float3x3(
                    tangentWS.xyz,
                    bitangent,
                    normalWS
                );
                
                return mul(tangentNormal, tangentToWorld);
            }
            
            float3 SampleNormal(float2 uv, float3 normalWS, float4 tangentWS, float scale = 1.0)
            {
                #if defined(_NORMALMAP)
                    float4 normalSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, TRANSFORM_TEX(uv, _BumpMap));
                    float3 tangentNormal = UnpackNormalScale(normalSample, scale);
                    return normalize(TransformTangentToWorld(tangentNormal, normalWS, tangentWS));
                #else
                    return normalWS;
                #endif
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 uv = input.uv;
                
                // Нормализуем векторы
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);
                
                // Применяем карту нормалей
                float3 N = SampleNormal(uv, normalWS, input.tangentWS, _BumpScale);
                
                float3 L = normalize(_MainLightPosition.xyz);
                float3 H = normalize(viewDirWS + L);
                
                float NV = dot(N, viewDirWS);
                float NH = dot(N, H);
                float NL = dot(N, L);
                
                // Ремимапим NL для тонового освещения
                NL = NL * 0.5 + 0.5;

                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);

                // Тоновое освещение
                float specularNH = smoothstep((1 - _SpecularStep) - _SpecularStepSmooth,
                                              (1 - _SpecularStep) + _SpecularStepSmooth, 
                                              NH);
                
                float shadowNL = smoothstep(_ShadowStep - _ShadowStepSmooth,
                                            _ShadowStep + _ShadowStepSmooth, 
                                            NL);

                float shadow = MainLightRealtimeShadow(input.shadowCoord);
                
                // Rim lighting
                float rim = smoothstep(_RimStep - _RimStepSmooth,
                                       _RimStep + _RimStepSmooth, 
                                       1.0 - NV);
                
                float3 diffuse  = _MainLightColor.rgb * baseMap.rgb * _BaseColor.rgb * shadowNL * shadow;
                float3 specular = _SpecularColor.rgb * shadow * shadowNL * specularNH;
                float3 ambient  = rim * _RimColor.rgb + SampleSH(N) * _BaseColor.rgb * baseMap.rgb;

                // Emission
                float3 emissionTex = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uv).rgb;
                float3 emission = emissionTex * _EmissionColor.rgb * _EmissionIntensity;

                float3 finalColor = diffuse + ambient + specular + emission;
                finalColor = MixFog(finalColor, input.fogCoord);

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        // Outline pass
        Pass
        {
            Name "Outline"
            Cull Front
            Tags { "LightMode" = "SRPDefaultUnlit" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float4 fogCoord : TEXCOORD0;	
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            
            v2f vert(appdata v)
            {
                v2f o;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.pos = TransformObjectToHClip(float4(v.vertex.xyz + v.normal * _OutlineWidth * 0.1 ,1));
                o.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 finalColor = MixFog(_OutlineColor.rgb, i.fogCoord);
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.ShaderGraphLitGUI"
}