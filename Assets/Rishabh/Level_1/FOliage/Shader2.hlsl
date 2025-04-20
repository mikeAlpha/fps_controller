#ifndef INSTANCE_POSITION_INCLUDED
#define INSTANCE_POSITION_INCLUDED

void InstancePosition_float(float4 posBuffer, out float3 position)
{
    position = posBuffer.xyz;
}

#endif