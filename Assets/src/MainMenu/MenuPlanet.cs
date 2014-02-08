using UnityEngine;
using System.Collections;

public class MenuPlanet : MonoBehaviour {

    PlanetInfo planetInfo;
    bool isMouseOver = false;

    GUIStyle style;

	// Use this for initialization
	void Start () {

        style = new GUIStyle();
        style.normal.textColor = Color.blue;
        style.fontSize = 24;
      
	
	}

    public void LoadInfo(PlanetInfo planetInfo)
    {
        this.planetInfo = planetInfo;        
    }

   

      

    void OnMouseEnter()
    {
        renderer.material.shader = Shader.Find("Unlit/Texture");
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        renderer.material.shader = Shader.Find("Diffuse");
        isMouseOver = false;
    }

    void OnMouseUp()
    {
        Persistence p = GameObject.Find("Persistence").GetComponent<Persistence>();
        p.pi = this.planetInfo;
        Application.LoadLevel("Planet");
    }
	
	// Update is called once per frame
	void Update () {

        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * 25);
	
	}

    void OnGUI()
    {
        if (isMouseOver)
        {
            
            Vector3 guiPos = Camera.main.WorldToScreenPoint(transform.position);            
                        
            GUI.Label(new Rect(guiPos.x-120, Screen.height - (guiPos.y-125), 300, 20), "Begin "+planetInfo.planetName+" Game", style);
        }
    }
}
