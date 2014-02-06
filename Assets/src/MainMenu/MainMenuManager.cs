using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour {

    const int width = 256;
    const int height = 128;
    public GameObject menuPlanetPrefab;

    public GameObject roguePlanetObj;

    static Texture2D roguePlanetTex; 
    // set the pixel values
    static float[] rogueColors = new float[width*height];
    PlanetInfo roguePlanet;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
        roguePlanetTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        roguePlanet = new PlanetInfo();

        List<PlanetInfo> planets = GameState.LoadPlanets();

        int count = 0;
        foreach (PlanetInfo pi in planets)
        {
            GameObject planet = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(-2 + count * 2, 1.35f, -7), Quaternion.identity);
            planet.AddComponent("MenuPlanet");            
            MenuPlanet mp = planet.GetComponent<MenuPlanet>();
            mp.LoadInfo(pi);
            mp.Generate3DPerlinMap(width,height);
            print("planet added");
            count++;
        }

        Invoke("SetupRoguePlanet", 1.5f);
	}

    private void SetupRoguePlanet()
    {
        roguePlanet.coalReserves = Random.Range(500, 40000);
        roguePlanet.oilFactor = Random.Range(0, 4);
        roguePlanet.windFactor = Random.Range(0, 4);
        roguePlanet.sunFactor = Random.Range(0.1f, 4);
        roguePlanet.pollutionClearance = Random.Range(0, 40);
        roguePlanet.startPollution = 0;
        roguePlanet.maxPollution = Random.Range(2500, 10000);
        roguePlanet.startEnergy = 20;
        roguePlanet.population = Random.Range(50, 200);
        roguePlanet.iq = Random.Range(1, 2);
        roguePlanet.gameLength = Random.Range(750, 1500);
        roguePlanet.planetSize = Random.Range(250, 1000);
        roguePlanet.rotationSpeed = Random.Range(.5f, 2);
        roguePlanet.windZones = Random.Range(0, 10);
        roguePlanet.octaves = Random.Range(1, 6);
        roguePlanet.gain = Random.Range(2f, 7.0f);
        roguePlanet.lacunarity = Random.Range(2f, 7.0f);

        int numColors = Random.Range(3, 15);
        Color[] colors = new Color[numColors];
        float[] ranges = new float[numColors - 1];
        float percentRemaining = 1f;
        float minPercent = .01f;
        float alpha = 0;
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), alpha);
            alpha = 1;
        }


        for (int i = 0; i < ranges.Length - 1; i++)
        {

            if (i == 0) // water
            {
                float percent = Random.Range(minPercent, .7f);
                ranges[i] = percent;
                percentRemaining -= percent;
            }
            else // else
            {

                int remainingColorsCount = ranges.Length - i - 1;
                float maxPercent = percentRemaining - (minPercent * remainingColorsCount);
                float percent = Random.Range(minPercent, maxPercent);
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
        roguePlanet.colorRamp = r;

        Invoke("UpdateRoguePlanet", 1.5f);

    }



    private void UpdateRoguePlanet()
    {
              
        if (roguePlanetObj != null)
        {
            Destroy(roguePlanetObj);
        }
        roguePlanetObj = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(0, .2f, -7), Quaternion.identity);
        roguePlanetObj.AddComponent("MenuPlanet");
        MenuPlanet rogueMP = roguePlanetObj.GetComponent<MenuPlanet>();
        rogueMP.LoadInfo(roguePlanet);
        rogueMP.Generate3DPerlinMap(roguePlanetTex, rogueColors);
        Invoke("SetupRoguePlanet", 1.5f);   
    }

    // Update is called once per frame
    void Update()
    {
	
	}

    public void Exit()
    {
        print("Quit");
        Application.Quit();
    }



}
