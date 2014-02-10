using UnityEngine;
using System.Collections;

public class MenuPlanet : Planet {

    
    bool isMouseOver = false;

    GUIStyle style;

    Shader startShader;

	// Use this for initialization
	new void Start () {
        base.Start();
        style = new GUIStyle();
        style.normal.textColor = Color.blue;
        style.fontSize = 24;
        startShader = renderer.material.shader;
      	
	}

    

         
    void OnMouseEnter()
    {
        renderer.material.shader = Shader.Find("Unlit/Texture");
        isMouseOver = true;
    }

    void OnMouseExit()
    {
        renderer.material.shader = startShader;
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

        transform.RotateAround(transform.position, Vector3.up, Time.deltaTime * 10);
	
	}

    void OnGUI()
    {
        if (isMouseOver && planetInfo != null)
        {
            
            Vector3 guiPos = Camera.main.WorldToScreenPoint(transform.position);            
                        
            GUI.Label(new Rect(guiPos.x-120, Screen.height - (guiPos.y-225), 300, 20), "Begin "+planetInfo.planetName+" Game", style);
        }
    }
}
