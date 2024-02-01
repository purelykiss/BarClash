shader_type canvas_item;

uniform bool _isUsing;
uniform vec4 color = vec4(1,1,1,1);

void fragment()
{
	vec4 pixelColor = texture(TEXTURE, UV);
	
	COLOR = pixelColor;
	if(_isUsing)
	{
		if(COLOR.a != float(0))
			COLOR = vec4(color.r,color.g,color.b,color.a);
		else
			COLOR = vec4(color.r,color.g,color.b,COLOR.a);
	}
}
