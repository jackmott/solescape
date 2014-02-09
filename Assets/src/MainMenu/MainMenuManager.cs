using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MainMenuManager : MonoBehaviour {

    const int width = 1024;
    const int height = 512;
    
    
    public GameObject menuPlanetPrefab;
    public GameObject waterPrefab;
    public GameObject roguePlanetObj;
                
    MenuPlanet rogueMP;

    PlanetGenerator pgThread;
    Thread thread;



	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
        
        

        List<PlanetInfo> planets = GameState.LoadPlanets();

        int count = 0;
        foreach (PlanetInfo pi in planets)
        {
            GameObject planet = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(-2.5f + count * 1.5f, 1.35f, -7), Quaternion.identity);
            GameObject water = (GameObject)Instantiate(waterPrefab, new Vector3(-2.5f + count * 1.5f, 1.35f, -7), Quaternion.identity);
            water.transform.parent = planet.transform;
            water.transform.localScale *= .99f;
            water.name = "Water";
            planet.AddComponent("MenuPlanet");            
            MenuPlanet mp = planet.GetComponent<MenuPlanet>();
            mp.GeneratePlanetNoise(width, height, pi);            
            print("planet added");
            count++;
        }
        
        pgThread = new PlanetGenerator(width, height);
        thread = new Thread(new ThreadStart(pgThread.start));
        thread.Start();
        roguePlanetObj = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(0, .2f, -7), Quaternion.identity);
        GameObject rogueWater = (GameObject)Instantiate(waterPrefab, new Vector3(0, .2f, -7), Quaternion.identity);
        rogueWater.transform.parent = roguePlanetObj.transform;
        rogueWater.transform.localScale *= .99f;
        rogueWater.name = "Water";
        roguePlanetObj.AddComponent("MenuPlanet");
        rogueMP = roguePlanetObj.GetComponent<MenuPlanet>();
	}

   
    
    // Update is called once per frame
    void Update()
    {
        if (pgThread.IsReady())
        {
            rogueMP.SetPlanet(width,height,pgThread.GetPlanetColors(), pgThread.GetPlanetInfo());
            pgThread.Finished();
        }
	}

    void OnDestroy()
    {
        print("on destroy");
        if (thread.IsAlive)
        {
            print("aborting thread");
            thread.Abort();
        }
        print("done");
    }

    void OnApplicationQuit()
    {
        print("application quit");
        
        if (thread.IsAlive)
        {
            print("aborting thread");
            thread.Abort();
        }
        print("done");
    }

    public void Exit()
    {
        print("Quit");
        Application.Quit();
    }



}
