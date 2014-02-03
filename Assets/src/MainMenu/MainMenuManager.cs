using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour {

    public GameObject menuPlanetPrefab;

	// Use this for initialization
	void Start () {
        List<PlanetInfo> planets = GameState.LoadPlanets();

        int count = 0;
        foreach (PlanetInfo pi in planets)
        {
            GameObject planet = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(-2 + count * 2, .7f, -7), Quaternion.identity);
            planet.AddComponent("MenuPlanet");            
            MenuPlanet mp = planet.GetComponent<MenuPlanet>();
            mp.LoadInfo(pi);
            print("planet added");
            count++;
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Exit()
    {
        print("Quit");
        Application.Quit();
    }



}
