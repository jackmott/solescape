using UnityEngine;
using System.Collections;

public class Persistence : MonoBehaviour {

    public PlanetInfo pi;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
	// Use this for initialization
	void Start () {
     
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
