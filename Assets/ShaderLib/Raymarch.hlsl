bool RM_IsPerspectiveProjection()
{
#if defined(SHADERPASS) && (SHADERPASS != SHADERPASS_SHADOWS)
    return (unity_OrthoParams.w == 0);
#else
    return UNITY_MATRIX_P[3][3] == 0;
#endif
}

float3 RM_GetCurrentViewPosition()
{    
    return _WorldSpaceCameraPos;
#if defined(SHADERPASS) && (SHADERPASS != SHADERPASS_SHADOWS)
    return _WorldSpaceCameraPos;
#else    
    return UNITY_MATRIX_I_V._14_24_34;
#endif
}

float3 RM_GetWorldSpaceNormalizeViewDir(float3 positionWS)
{
    if (RM_IsPerspectiveProjection())
    {        
        float3 V = positionWS - RM_GetCurrentViewPosition();
        return normalize(V);
    }
    else return normalize(-UNITY_MATRIX_V[2].xyz);
}