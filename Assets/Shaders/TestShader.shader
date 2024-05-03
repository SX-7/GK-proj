// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/TestShader" {
	Properties{
		_FontTex("FontTexture", 2D) = "white" {}
        _FontTextColumns("FontTexture Columns", Int) = 6
        _FontTextRows("FontTexture Rows", Int) = 6
        _StringCharacterCount("Length of String", Int) = 3
        _StringOffset("String offset", Vector) = (0.5,0.5,0,0)
        _StringScale("String scale", Vector) = (0.25,0.25,0,0)
        _CharWidth("Character width", Float) = 1.0
        _Color("Color", Color) = (1,1,1,1)
		
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }

		Pass {
			CGPROGRAM
			#pragma vertex VertProg
			#pragma fragment FragProg
			#include "UnityCG.cginc"

			float4 _Tint;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct Interpolators {
				float4 position :SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 screenPos : TEXCOORD2;
			};

			struct VertexData {
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};


			sampler2D _FontTex;
			float4 _FontTex_ST;

			float4 _Color;

			int _FontTextColumns;
			int _FontTextRows;

			int _StringCharacterCount;

			float _String_Chars[512];

			float4 _StringOffset;
			float4 _StringScale;

			float _CharWidth;

			Interpolators VertProg(VertexData v) {
				Interpolators i;
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);
				i.position = UnityObjectToClipPos(v.position);
				i.screenPos = ComputeScreenPos(i.position);
				return i;
			}
						
			float4 FragProg(Interpolators i) : SV_TARGET{
				clip(_StringCharacterCount - 1);
				fixed4 col;

				// Determine what character in the string this pixel is in
                // And what UV of that character we are in
                float charIndex = 0;
                float2 inCharUV = float2(0,0);

                // Avoid i.uv.x = 1 and indexing charIndex[_StringCharacterCount] 
                i.screenPos.x = fmod(i.screenPos.x,1);

                // Scale and offset uv
                i.screenPos = (i.screenPos - _StringOffset.xy) / _StringScale.xy + 0.5;
				i.screenPos.x = fmod(i.screenPos.x+_Time.y/2,1);
				i.screenPos.y = fmod(i.screenPos.y+_Time.y/2,1);

                // Find where in the char to sample
                inCharUV = float2(
                    modf(i.screenPos.x * _StringCharacterCount, charIndex),
                    i.screenPos.y);

                // Scale inCharUV.x based on charWidth factor
                inCharUV.x = (inCharUV.x-0.5f)/_CharWidth + 0.5f;

                // Clamp char uv
                // alternatively you could clip if outside (0,0)-(1,1) rect
                //inCharUV = clamp(inCharUV, 0, 1);

                // Get char uv in font texture space
                float fontIndex = _String_Chars[charIndex];
                float fontRow = floor(fontIndex / _FontTextColumns);
                float fontColumn = floor(fontIndex % _FontTextColumns);

                float2 fontUV = float2(
                        (fontColumn + inCharUV.x) / _FontTextColumns,
                        1.0 - (fontRow + 1.0 - inCharUV.y) / _FontTextRows);

                // Sample the font texture at that uv
                col = tex2D(_FontTex, fontUV);

                // Modify by color:
                col = col * _Color;
				return col;
				//return tex2D(_MainTex,i.screenPos.xy*_MainTex_ST.xy+_Time.xx+_MainTex_ST.zw)*_Tint;
			}
			
			ENDCG
		}
	}
}