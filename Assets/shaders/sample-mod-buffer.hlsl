//Make sure its only included once!
#ifndef CUSTOM_COLOR_INCLUDED
#define CUSTOM_COLOR_INCLUDED

sampler2D g_ModBuffer;

void SampleModeBuffer_float(float2 screenUv, out float color)
{
    color = tex2D(g_ModBuffer, screenUv);
}
#endif