using UnityEngine;
using System.Collections;


public class PlanetControlManager : MonoBehaviour {

    public dfControl ColorPanel;
    public dfButton ColorDialogButton;

    public GameObject menuPlanetPrefab;

    static PlanetControlManager instance;

    int width = 2048;
    int height = 1024;

    
	// Use this for initialization

    public static PlanetControlManager Instance
    {
        get
        {
            while (instance == null)
            {
                System.Threading.Thread.Sleep(1000);
                print("PlanetControl instance");
            }
            return instance;
        }
    }


    void Awake()
    {
        instance = this;
    }
	void Start () {
        RedrawPlanet();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnColorMouseUp(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ColorPanel.Show();
        ColorDialogButton buttonScript = ColorDialogButton.GetComponent<ColorDialogButton>();
        buttonScript.colorToSet = (dfSlicedSprite)control.Find("Color");
    }

    public void OnSliderChanged(dfControl control, float value)
    {
        dfSlider[] sliders = new dfSlider[5];
        int currIndex = 0;
        int sum = 0;

        //get the sum
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i] = GameObject.Find("Slider"+(i+1)).GetComponent<dfSlider>();
            if (sliders[i] == control) currIndex = i;
            sum += (int)sliders[i].Value;
        }

        //check if sum adds up to one hunnered
        if (sum < 100)
        {
            int adjustIndex = (currIndex+1)%sliders.Length;
            sliders[adjustIndex].IsEnabled = false;
            sliders[adjustIndex].Value = 100 - sum;
            sliders[adjustIndex].IsEnabled = true;
        }
        else if (sum > 100)
        {
            int adjustIndex = (currIndex + 1) % sliders.Length;
            do
            {
                int adjustmentNeeded = sum - 100;
                sum = (int)(sum - sliders[adjustIndex].Value);
                int adjustmentAvailable = (int)sliders[adjustIndex].Value;
                sliders[adjustIndex].IsEnabled = false;
                sliders[adjustIndex].Value -= Mathf.Min(adjustmentNeeded, adjustmentAvailable);
                sliders[adjustIndex].IsEnabled = true;
                sum = (int)(sum + sliders[adjustIndex].Value);
                adjustIndex = (adjustIndex + 1) % sliders.Length;
            } while (sum > 100);
        }        
    }

   
    public void RedrawPlanet()
    {
        GameObject planet = GameObject.Find("PLANET");
        if (planet != null) Destroy(planet);
        planet = (GameObject)Instantiate(menuPlanetPrefab, new Vector3(0, 1.35f, -7), Quaternion.identity);
        planet.transform.localScale = new Vector3(2, 2, 2);
        planet.name = "PLANET";
        planet.AddComponent("MenuPlanet");
        MenuPlanet mp = planet.GetComponent<MenuPlanet>();        
        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        PlanetGenerator pg = new PlanetGenerator(width, height, tex);
        pg.generatePlanet(CreateInfo());
        pg.LoadPlanet(mp);
        print("planet added");
        
    }

    public PlanetInfo CreateInfo()
    {
        PlanetInfo planetInfo = new PlanetInfo();
        planetInfo.coalReserves = 10000;
        planetInfo.oilFactor = 1;
        planetInfo.windFactor = 1;
        planetInfo.sunFactor = 1;
        planetInfo.pollutionClearance = 20;
        planetInfo.startPollution = 0;
        planetInfo.maxPollution = 5000;
        planetInfo.startEnergy = 20;
        planetInfo.population = 100;
        planetInfo.iq = 1;
        planetInfo.gameLength = 1000;
        planetInfo.planetSize = 500;
        planetInfo.rotationSpeed = 1;
        planetInfo.windZones = 3;
        planetInfo.octaves = 3;
        planetInfo.gain = 2;
        planetInfo.lacunarity = 2;

        Color[] colors = new Color[6];
        float[] ranges = new float[5];


        colors[0] = GameObject.Find("ColorDisplay1").GetComponentInChildren<dfSlicedSprite>().Color;
        ranges[0] = .2f;
        colors[1] = GameObject.Find("ColorDisplay2").GetComponentInChildren<dfSlicedSprite>().Color;
        ranges[1] = .2f;
        colors[2] = GameObject.Find("ColorDisplay3").GetComponentInChildren<dfSlicedSprite>().Color;
        ranges[2] = .2f;
        colors[3] = GameObject.Find("ColorDisplay4").GetComponentInChildren<dfSlicedSprite>().Color;
        ranges[3] = .2f;
        colors[4] = GameObject.Find("ColorDisplay5").GetComponentInChildren<dfSlicedSprite>().Color;
        ranges[4] = .2f;
        colors[5] = GameObject.Find("ColorDisplay6").GetComponentInChildren<dfSlicedSprite>().Color;

        

        ColorRamp r = new ColorRamp(colors, ranges);
        planetInfo.colorRamp = r;

        return planetInfo;
    }
}
