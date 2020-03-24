// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outline/Outline"
{
    Properties
    {
        _Color("Main Color", Color) = (0.5,0.5,0,1)
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("Outline color", Color) = (0,0,0,1)
        _OutlineWidth("Outline width", Range(0,2)) = 1.06
    }

    CGINCLUDE
    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
	};
    struct v2f
    {
        float4 pos : POSITION;
        float3 normal : NORMAL;
	};

    float _OutlineWidth;
    float4 _OutlineColor;
    
    v2f vert(appdata v)
    {
        v.vertex.xyz *= _OutlineWidth / half3(length(unity_ObjectToWorld._m00_m10_m20),length(unity_ObjectToWorld._m01_m11_m21),length(unity_ObjectToWorld._m02_m12_m22)) +1;
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        return o;
	}

    ENDCG

    SubShader
    {
        Tags{ "Queue" = "Transparent"}
        Pass
        {
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag(v2f i) : COLOR
            {
                return _OutlineColor;
			}
            ENDCG
		}
        Pass
        {
            ZWrite On

            Material
            {
                Diffuse[_Color]
                Ambient[_Color]     
			}
            Lighting On
            SetTexture[_MainTex]
            {
                ConstantColor[_Color]     
			}
            SetTexture[_MainTex]
            {
                Combine previous * primary DOUBLE     
			}
		}

    }
}
