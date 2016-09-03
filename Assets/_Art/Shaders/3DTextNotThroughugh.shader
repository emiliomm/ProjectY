//Shader que muestra el gameObject 3DText sin atravesar la geometría de la escena
//No funciona con la fuente por defecto(Arial), usar otra
//Hay que arrastrar la textura de la fuente al material
//Author: Eric Haines (Eric5h5)
Shader "3DTextNotThroughugh" {
	 Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
	}
 
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Lighting Off Cull Back ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			Color [_Color]
			SetTexture [_MainTex] {
				combine primary, texture * primary
			}
		}
	}
}