shader_type canvas_item;

uniform bool _isUsing;

void fragment()
{
	vec4 pixelColor = texture(TEXTURE, UV);
	COLOR = pixelColor;
	if(_isUsing)
	{
		vec4 color = vec4(255,255,255,255);
		COLOR = vec4(color.r,color.g,color.b,COLOR.a);
	}
}
