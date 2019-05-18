using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LightServer {

    public float range;
    public Color color;
    public float intensity;

    public float shadowStrength;
    public float shadowBias;
    public float shadowNormalBias;
    public float shadowNearPlane;

    public void Copy(Light light) {
        range = light.range;
        color = light.color;
        intensity = light.intensity;

        shadowStrength = light.shadowStrength;
        shadowBias = light.shadowBias;
        shadowNormalBias = light.shadowNormalBias;
        shadowNearPlane = light.shadowNearPlane;
    }

}
