using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public WorldObject[] surrounded;

    private List<Tile> surroundedTiles;

    public bool isContained = false;

    // Gas Properties
    public GasMixture gasMixture;
    public float tileSize = 2500f; // 1m * 2.5m * 1m = 2500 litres
    public float temperature = 298.15f; // 25 degrees in kelvin, average room temperature
    public float pressure = 0; // in kPa

    void Start() {
        // Get surrounding tiles, because we will need to update them
        surroundedTiles = new List<Tile>();
        surroundedTiles.Add(GetSurroundedTile(transform.forward));
        surroundedTiles.Add(GetSurroundedTile(-transform.right));
        surroundedTiles.Add(GetSurroundedTile(-transform.forward));
        surroundedTiles.Add(GetSurroundedTile(transform.right));

        gasMixture = new GasMixture();
    }

    void FixedUpdate() {
        UpdatePressure();
    }

    private void CalculatePressure() {
        // Carbon atmomic mass = 44, we have 22g of carbon (.gases[g]), number of moles = 22g / 44;
        float totalMoles = 0f;

        for (int g = 0; g < gasMixture.gases.Length; g++) {
            totalMoles += gasMixture.gases[g] / Gas.gases[g].GetAtomicMass();
        }

        pressure = (totalMoles * 8.3145f * temperature) / tileSize;
    }

    private void UpdatePressure() {
        for (int g = 0; g < gasMixture.gases.Length; g++) {
            float maximumGiveAmount = gasMixture.gases[g] / (0.1f * (1f + surroundedTiles.Count));

            foreach (Tile tile in surroundedTiles) {
                if (tile != null) {
                    if (gasMixture.gases[g] > tile.gasMixture.gases[g]) {
                        float difference = gasMixture.gases[g] - tile.gasMixture.gases[g];

                        if (difference <= maximumGiveAmount) {
                            tile.gasMixture.gases[g] += 0.1f * (difference / 2f);
                            gasMixture.gases[g] -= 0.1f * (difference / 2f);
                        }
                        else {
                            tile.gasMixture.gases[g] += maximumGiveAmount;
                            gasMixture.gases[g] -= maximumGiveAmount;
                        }
                    }
                }
            }
        }

        CalculatePressure();
    }

    private Tile GetSurroundedTile(Vector3 direction) {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.red, Mathf.Infinity, true);

        if (Physics.Raycast(ray, out hit, 1.2f)) {
            if (hit.transform != null) {
                Tile tile = hit.transform.GetComponent<Tile>();

                if (tile != null) {
                    return tile;
                }
            }
        }

        return null;
    }

    public void UpdateContainment() {
        /*
        for (int x = 0; x < surrounded.Length; x++) {
            if (surrounded[x] != null && surrounded[x].airTight) {
                isContained = true;
                continue;
            } else {
                isContained = false;
                return;
            }
        }
        */
    }

    public void AddPressure(float kPa, int gasIndex) {
        gasMixture.gases[gasIndex] += (((kPa * tileSize) / (8.3145f * temperature)) * Gas.gases[gasIndex].GetAtomicMass());
    }

    public string GetTileInformation() {
        string tileInfo = "";
        tileInfo += "Tile Name: " + transform.name + "\n";
        tileInfo += "Tile Size: " + tileSize + "\n";
        tileInfo += "Temperature: " + temperature + "\n";
        tileInfo += "Pressure: " + pressure + "\n";
        tileInfo += "\n";
        tileInfo += "Gases in Grams" + "\n";
        for (int g = 0; g < gasMixture.gases.Length; g++) {
            tileInfo += Gas.gases[g].GetName() + ": " + gasMixture.gases[g] + "g" + "\n";
        }

        return tileInfo;
    }
}
