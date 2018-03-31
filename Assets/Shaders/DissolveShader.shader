Shader "Custom/DissolveShader" {
	Properties{
		_ColorBase("Color", Color) = (.34, .85, .92, 1)
		_ColorBorder("Border Color", Color) = (.34, .85, .92, 1)
		_Border("Border Amount", Range(0.0, 1.0)) = 0.5
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		//if you're not planning on using shadows, remove "addshadow" for better performance
#pragma surface surf Lambert addshadow
		struct Input {		
		float2 uv_SliceGuide;
		float _SliceAmount;
	};
	sampler2D _SliceGuide;
	float _SliceAmount;
	float4 _ColorBase;
	float4 _ColorBorder;
	float _Border;
	void surf(Input IN, inout SurfaceOutput o) {
		o.Albedo = _ColorBase;
		o.Albedo = _ColorBase;
		if (tex2D(_SliceGuide, IN.uv_SliceGuide).r - _SliceAmount < 0
			&& abs(tex2D(_SliceGuide, IN.uv_SliceGuide).r - _SliceAmount) < _Border) {
			o.Albedo = _ColorBorder;
		}else if (tex2D(_SliceGuide, IN.uv_SliceGuide).r - _SliceAmount < 0) clip(-1);				
	}
	ENDCG
	}
		Fallback "Diffuse"
}