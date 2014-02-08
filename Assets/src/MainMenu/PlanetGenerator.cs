using UnityEngine;
using System.Collections;
using System.Threading;


public class PlanetGenerator  {


    PlanetInfo planetInfo;
    Texture2D planetTex;
    int width;
    int height;
    float[] colors;
    Color[] Colors;
    System.Random rand;

    public bool ready = false;
    Noise noise;

    public PlanetGenerator(int width, int height, Texture2D planetTex)
    {
        rand = new System.Random();
        this.planetTex = planetTex;
        colors = new float[width * height];
        Colors = new Color[width * height];
        noise = new Noise();
        this.width = width;
        this.height = height;
    }

    public void start()
    {
        while (true)
        {
            if (!ready)
            {                
                generatePlanet();
                Thread.Sleep(3000);
            }
        }

    }

    public void generatePlanet(PlanetInfo pi)
    {
        
        planetInfo = pi;        
        Generate3DPerlinMap();
    }

    public void generatePlanet()
    {
        RandomInfo();
        generatePlanet(planetInfo);
    }

    public void LoadPlanet(MenuPlanet planet)
    {
        if (!ready) return;
        planet.LoadInfo(planetInfo);
        planetTex.SetPixels(Colors);
        planetTex.Apply();
        planet.renderer.material.mainTexture = planetTex;
        ready = false;
    }

    private int Range(int min, int max)
    {
        return rand.Next(min, max);
    }

    private float Range(float min, float max)
    {
        float r = (float)rand.NextDouble();
        float range = max - min;
        r = r * range;
        float offset = min;
        r = r + offset;
        return r;
    }

    private void RandomInfo()
    {
        

        planetInfo.coalReserves = Range(500, 40000);
        planetInfo.oilFactor = Range(0, 4);
        planetInfo.windFactor = Range(0, 4);
        planetInfo.sunFactor = Range(0.1f, 4);
        planetInfo.pollutionClearance = Range(0, 40);
        planetInfo.startPollution = 0;
        planetInfo.maxPollution = Range(2500, 10000);
        planetInfo.startEnergy = 20;
        planetInfo.population = Range(50, 200);
        planetInfo.iq = Range(1, 2);
        planetInfo.gameLength = Range(750, 1500);
        planetInfo.planetSize = Range(250, 1000);
        planetInfo.rotationSpeed = Range(.5f, 2);
        planetInfo.windZones = Range(0, 10);
        planetInfo.octaves = Range(1, 6);
        planetInfo.gain = Range(2f, 7.0f);
        planetInfo.lacunarity = Range(2f, 7.0f);

        int numColors = Range(3, 15);
        Color[] colors = new Color[numColors];
        float[] ranges = new float[numColors - 1];
        float percentRemaining = 1f;
        float minPercent = .01f;
        float alpha = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Range(0f, 1f), Range(0f, 1f), Range(0f, 1f), alpha);
            alpha = 1;
        }


        for (int i = 0; i < ranges.Length - 1; i++)
        {

            if (i == 0) // water
            {
                float percent = Range(minPercent, .7f);
                ranges[i] = percent;
                percentRemaining -= percent;
            }
            else // else
            {

                int remainingColorsCount = ranges.Length - i - 1;
                float maxPercent = percentRemaining - (minPercent * remainingColorsCount);
                float percent = Range(minPercent, maxPercent);
                ranges[i] = percent;
                percentRemaining -= percent;
            }

        }
        ranges[ranges.Length - 1] = percentRemaining;

        float sum = 0;
        for (int i = 0; i < ranges.Length; i++)
        {

            sum += ranges[i];
        }


        ColorRamp r = new ColorRamp(colors, ranges);
        planetInfo.colorRamp = r;

        

    }
   
    private void Generate3DPerlinMap()
    {


        float pi = 3.14159265359f;
        float twopi = pi * 2.0f;

        float offsetx = (float)Range(-200f, 200f);
        float offsety = (float)Range(-200f, 200f);

        float min = 999;
        float max = -999;

        for (int y = 0; y < height; y++)
        {
            int row = y * width;
            for (int x = 0; x < width; x++)
            {

                float theta = twopi * (x / (float)width);
                float phi = pi * (y / (float)height);

                float x3d = Mathf.Cos(theta) * Mathf.Sin(phi);
                float y3d = Mathf.Sin(theta) * Mathf.Sin(phi);
                float z3d = -Mathf.Cos(phi);



                float color = noise.fbm3(x3d * 2 + offsetx, y3d * 2 + offsety, z3d * 2, planetInfo.octaves, planetInfo.gain, planetInfo.lacunarity);

                if (color < min) min = color;
                if (color > max) max = color;
                colors[row + x] = color;


            }
        }

        //GameObject.Find("Water").renderer.material.color = planetInfo.colorRamp.colors[0];
        Colors = noise.rescaleAndColorArrayMenu(colors, min, max, planetInfo.colorRamp.colors);
        ready = true;
    }

}
