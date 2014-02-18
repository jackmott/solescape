using UnityEngine;
using System.Collections;


public class PlanetInfo  {

    public string planetName;
    public string skybox;
    public string normals;
    public float coalReserves;
    public float oilFactor;
    public float windFactor;
    public float sunFactor;
    public float rotationSpeed;
    public int gameLength;
    public int planetSize;
    public int population;    
    public float iq;
    public int startEnergy;
    public int startPollution;
    public float pollutionClearance;
    public int maxPollution;
    public int windZones;
    public int octaves;
    public float gain;
    public float lacunarity;
    public float stretch;

    public ColorRamp colorRamp;

    public override string ToString()
    {
        string result = planetName + ",";
        result += coalReserves+",";
        result += oilFactor+",";
        result += windFactor+",";
        result += sunFactor+",";
        result += rotationSpeed+",";
        result += gameLength+",";
        result += planetSize+",";
        result += population+",";
        result += iq+",";
        result += startEnergy+",";
        result += startPollution+",";
        result += pollutionClearance+",";
        result += maxPollution+",";
        result += windZones+",";
        result += octaves+",";
        result += gain + ",";
        result += lacunarity + ",";
        result += stretch + ",";
        result += colorRamp.ToString();
        return result;

    }

}
