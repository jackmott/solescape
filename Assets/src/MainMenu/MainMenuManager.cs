using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class MainMenuManager : MonoBehaviour {

    const int width = 1024;
    const int height = 512;
    public GameObject menuPlanetPrefab;

    public GameObject roguePlanetObj;

    
    // set the pixel values
    
    

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
            planet.AddComponent("MenuPlanet");            
            MenuPlanet mp = planet.GetComponent<MenuPlanet>();
            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32,false);
            PlanetGenerator pg = new PlanetGenerator(width, height,tex);
            pg.generatePlanet(pi);
            pg.LoadPlanet(mp);
            print("planet added");
            count++;
        }
        Texture2D rtex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        pgThread = new PlanetGenerator(width, height,rtex);
        thread = new Thread(new ThreadStart(pgThread.start));
        thread.Start();

        roguePlanetObj = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(0, .2f, -7), Quaternion.identity);
        roguePlanetObj.AddComponent("MenuPlanet");
        rogueMP = roguePlanetObj.GetComponent<MenuPlanet>();
	}

   
    
    // Update is called once per frame
    void Update()
    {
        if (pgThread.ready)
        {    
            pgThread.LoadPlanet(rogueMP);
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
