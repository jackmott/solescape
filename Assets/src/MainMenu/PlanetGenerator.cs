using UnityEngine;
using System.Collections;
using System.Threading;


public class PlanetGenerator  {


    public PlanetInfo planetInfo;
   
    int width;
    int height;
    float[] floatColors;
    bool generateClouds=false;
    System.Random rand;
    Color[] colors;
    Color[] cloudColors;

    private bool ready = false;
    Noise noise;

    public PlanetGenerator(int width, int height)
    {
       SharedConstructor(width,height);
    }

    public PlanetGenerator(int width, int height,bool generateClouds)
    {
        this.generateClouds = generateClouds;
        SharedConstructor(width, height);
    }

    private void SharedConstructor(int width, int height)
    {
        planetInfo = new PlanetInfo();
        rand = new System.Random();        
        floatColors = new float[width * height];        
        noise = new Noise();
        this.width = width;
        this.height = height;
    }

    //generate random planets, forever
    public void start()
    {
        Debug.Log("start() pg");
        while (true)
        {
            if (!ready)
            {                
                generateRandomPlanet();
                Thread.Sleep(3000);
            }
        }

    }

    
    //generate 1 planet, with the current planetinfo
    public void startPlanetInfo()
    {
        Debug.Log("startPlanetInfo() pg");   
        generatePlanet(planetInfo);
    }

    public void generatePlanet(PlanetInfo pi)
    {
        
        planetInfo = pi;        
        Generate3DPerlinMap();
    }

    public void generateRandomPlanet()
    {
        RandomInfo();
        generatePlanet(planetInfo);
    }

    public Color[] GetPlanetColors()
    {
        if (!ready) return null;                
        return colors;
    }

    public Color[] GetCloudColors()
    {
        if (!ready) return null;
        return cloudColors;
    }

    public PlanetInfo GetPlanetInfo()
    {
        if (!ready) return null;
        return planetInfo;
    }

    public bool IsReady()
    {
        return ready;
    }

    public void Finished()
    {
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
        planetInfo.planetName = "QuiGon";

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
        Debug.Log("generate3dperlin() pg");   

        float pi = 3.14159265359f;
        float twopi = pi * 2.0f;

        float offsetx = (float)Range(-200f, 200f);
        float offsety = (float)Range(-200f, 200f);

        float min = 999;
        float max = -999;

        int octaves = planetInfo.octaves;
        float gain = planetInfo.gain;
        float lacunarity = planetInfo.lacunarity;
        float stretch = planetInfo.stretch;

        float x3d, y3d, z3d, theta, phi, color;
        int row;

        float sinPhi;

        for (int y = 0; y < height; y++)
        {
            //such fast! MUCH SPEED!
            row = y * width;
            phi = pi * (y / (float)height);
            z3d = -Mathf.Cos(phi);
            sinPhi = Mathf.Sin(phi);
            for (int x = 0; x < width; x++)
            {
                theta = twopi * (x / (float)width);
                
                x3d = Mathf.Cos(theta) * sinPhi;
                y3d = Mathf.Sin(theta) * sinPhi;
                
                color = noise.fbm3(x3d * 2 + offsetx, y3d * stretch + offsety, z3d * 2 , octaves, gain, lacunarity);

                if (color < min) min = color;
                if (color > max) max = color;
                floatColors[row + x] = color;

            }
        }

        //GameObject.Find("Water").renderer.material.color = planetInfo.colorRamp.colors[0];
        colors = noise.rescaleAndColorArray(floatColors, min, max, planetInfo.colorRamp.gradient);
        if (generateClouds)
            cloudColors = noise.rescaleArray(floatColors,min,max);
        ready = true;
        Debug.Log("ENDgenerate3dperlin() pg");   
    }

}
