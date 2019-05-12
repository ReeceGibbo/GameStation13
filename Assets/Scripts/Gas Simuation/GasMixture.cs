using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasMixture {

    public float[] gases;

    public GasMixture() {
        gases = new float[Gas.gases.Count];

        for (int g = 0; g < gases.Length; g++) {
            gases[g] = 0f;
        }
    }

    public float GetAccurateTotal() {
        float total = 0f;

        for (int g = 0; g < gases.Length; g++) {
            total += gases[g] / Gas.gases[g].GetAtomicMass();
        }

        return total;
    }

}
