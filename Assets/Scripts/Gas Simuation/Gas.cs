using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gas {

    public static List<Gas> gases = new List<Gas>();

    public static Gas NITROGEN = new Gas("Nitrogen", 0, 14.0067f);
    public static Gas OXYGEN = new Gas("Oxygen", 1, 15.999f);
    public static Gas ARGON = new Gas("Argon", 2, 39.948f);
    public static Gas CARBON_DIOXIDE = new Gas("Carbon Dioxide", 3, 44.01f);

    private string name;
    private int index;
    private float atomicMass;

    public Gas(string name, int index, float atomicMass) {
        gases.Add(this);
        this.name = name;
        this.index = index;
        this.atomicMass = atomicMass;
    }

    public string GetName() {
        return name;
    }

    public int GetIndex() {
        return index;
    }

    public float GetAtomicMass() {
        return atomicMass;
    }

}
