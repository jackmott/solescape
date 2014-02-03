using UnityEngine;
using System.Collections;

public class BuildingInfoContainer : MonoBehaviour {

    public BuildingInfo bi;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlaceBuilding()
    {
        GameState.Instance.planet.PlaceMode(bi.className);
    }
}
