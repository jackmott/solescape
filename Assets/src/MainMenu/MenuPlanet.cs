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
        Texture2D planetTex = new Texture2D(256, 256, TextureFormat.ARGB32, false);

        // set the pixel values
        Color[] colors = planetTex.GetPixels();

        Noise noise = new Noise();
        float pi = Mathf.PI;
        float twopi = pi * 2.0f;

        float offsetx = (float)Random.Range(-200, 200);
        float offsety = (float)Random.Range(-200, 200);
        int row;
        for (int y = 0; y < planetTex.height; y++)
        {
            row = planetTex.width * y;
            for (int x = 0; x < planetTex.width; x++)
            {

                float theta = twopi * (x/(float)planetTex.width);
                float phi = pi * (y/(float)planetTex.height);

                float x3d = Mathf.Cos(theta) * Mathf.Sin(phi);
                float y3d = Mathf.Sin(theta) * Mathf.Sin(phi);
                float z3d = -Mathf.Cos(phi);


                float color = noise.fbm3(x3d*2+offsetx, y3d*2+offsety, z3d*2,3,2,2);
               
                int index = (int)(color * (planetInfo.colorRamp.colors.Length - 1));
                colors[row + x] = planetInfo.colorRamp.colors[index];
                            
            }
        }

        
        // Apply all SetPixel calls
        planetTex.SetPixels(colors);
        planetTex.Apply();
        renderer.material.mainTexture = planetTex;

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
