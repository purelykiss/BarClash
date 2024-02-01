shader_type canvas_item;

uniform bool _isUsing;
uniform vec4 borderColor = vec4(1,1,1,1);

void fragment()
{
	vec4 pixelColor = texture(TEXTURE, UV);
	vec4 pixelColor1 = texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE.x, 0));
	vec4 pixelColor2 = texture(TEXTURE, UV + vec2(-TEXTURE_PIXEL_SIZE.x, 0));
	vec4 pixelColor3 = texture(TEXTURE, UV + vec2(0, TEXTURE_PIXEL_SIZE.y));
	vec4 pixelColor4 = texture(TEXTURE, UV + vec2(0, -TEXTURE_PIXEL_SIZE.y));
	float checker = pixelColor1.a + pixelColor2.a + pixelColor3.a + pixelColor4.a;
	
	if(_isUsing)
	{
		if(pixelColor.a == float(0) && checker > float(0))
			COLOR = vec4(borderColor.r,borderColor.g,borderColor.b,borderColor.a);
		else
			COLOR = pixelColor;
	}
	else
		COLOR = pixelColor;
}
