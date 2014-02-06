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

    public void Generate3DPerlinMap(int width, int height)
    {
        Texture2D planetTex = new Texture2D(width, height, TextureFormat.ARGB32, false);


        // set the pixel values
        float[] colors = new float[planetTex.width * planetTex.height];

        Generate3DPerlinMap(planetTex, colors);
    }

    public void Generate3DPerlinMap(Texture2D planetTex, float[] colors)
    {


                
        Noise noise = new Noise();


        float pi = 3.14159265359f;
        float twopi = pi * 2.0f;

        float offsetx = (float)Random.Range(-200f, 200f);
        float offsety = (float)Random.Range(-200f, 200f);

        float min = 999;
        float max = -999;

        for (int y = 0; y < planetTex.height; y++)
        {
            int row = y * planetTex.width;
            for (int x = 0; x < planetTex.width; x++)
            {

                float theta = twopi * (x / (float)planetTex.width);
                float phi = pi * (y / (float)planetTex.height);

                float x3d = Mathf.Cos(theta) * Mathf.Sin(phi);
                float y3d = Mathf.Sin(theta) * Mathf.Sin(phi);
                float z3d = -Mathf.Cos(phi);



                float color = noise.fbm3(x3d * 2 + offsetx, y3d * 2 + offsety, z3d * 2, planetInfo.octaves, planetInfo.gain, planetInfo.lacunarity);

                if (color < min) min = color;
                if (color > max) max = color;
                colors[row + x] = color;
                

            }
        }
                        
        //GameObject.Find("Water").renderer.material.color = planetInfo.colorRamp.colors[0];
        planetTex.SetPixels(noise.rescaleAndColorArrayMenu(colors,min,max,planetInfo.colorRamp.colors));
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
