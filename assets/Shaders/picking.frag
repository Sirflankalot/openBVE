﻿#version 150

uniform int uObjectIndex;

void main(void)
{
	gl_FragData[0].r = float(uObjectIndex);
}
