using UnityEngine;
using System.Collections;
using System.Threading;

public class PlanetControlManager : MonoBehaviour {

    public dfControl ColorPanel;
    public dfButton ColorDialogButton;

    public GameObject menuPlanetPrefab;

    static PlanetControlManager instance;

    int width = 1024;
    int height = 512;

    private bool legit = true;

    private PlanetGenerator pg;
    private Texture2D planetTex;

    MenuPlanet mp;
    GameObject planet;
    
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
        print("AWAKE");
        instance = this;
    }

	void Start () {
        print("START");
        planetTex = new Texture2D(width,height,TextureFormat.ARGB32,false);
        pg = new PlanetGenerator(width, height, planetTex);
        print("STARTB");
        planet = GameObject.Find("Planet");
        print("STARTC");                        
        mp = planet.GetComponent<MenuPlanet>();
        print("STARTD");
        RedrawPlanet();	
	}
	
	// Update is called once per frame
	void Update () {
        if (pg.IsReady())
        {
            print("loadplanet");
            mp.SetPlanet(width, height, pg.GetPlanetColors(), pg.GetPlanetInfo());
            pg.Finished();
        }
	}

    public void OnColorMouseUp(dfControl control, dfMouseEventArgs mouseEvent)
    {
        ColorPanel.Show();
        ColorDialogButton buttonScript = ColorDialogButton.GetComponent<ColorDialogButton>();
        buttonScript.colorToSet = (dfSlicedSprite)control.Find("Color");
    }

    public void OnSliderChanged(dfControl control, float value)
    {
        if (legit)
        {
            legit = false;
        }
        else
        {
            return;
        }
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
            sliders[adjustIndex].Value += 100 - sum;            
        }
        else if (sum > 100)
        {
            int adjustIndex = (currIndex + 1) % sliders.Length;
            do
            {
                print("sum:" + sum);
                int adjustmentNeeded = sum - 100;
                sum = (int)(sum - sliders[adjustIndex].Value);
                int adjustmentAvailable = (int)sliders[adjustIndex].Value;
                print("Slider preadjusted val:" + sliders[adjustIndex].Value);
                sliders[adjustIndex].Value -= Mathf.Min(adjustmentNeeded, adjustmentAvailable);
                print("Slider adjusted val:" + sliders[adjustIndex].Value);
                sum = (int)(sum + sliders[adjustIndex].Value);
                adjustIndex = (adjustIndex + 1) % sliders.Length;
            } while (sum > 100);
        }

        legit = true;
    }

   
    public void RedrawPlanet()
    {
        
        pg.planetInfo = CreateInfo();
        Thread thread = new Thread(new ThreadStart(pg.startPlanetInfo));
        thread.Start();
        
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
        planetInfo.octaves = (int)GameObject.Find("OctavesSlider").GetComponentInChildren<dfSlider>().Value;
        planetInfo.gain = GameObject.Find("GainSlider").GetComponentInChildren<dfSlider>().Value;
        planetInfo.lacunarity = GameObject.Find("LacunaritySlider").GetComponentInChildren<dfSlider>().Value;
        planetInfo.stretch = 2;

        Color[] colors = new Color[6];
        float[] ranges = new float[5];

        for (int i = 0; i < colors.Length;i++)
        {
            Color c = GameObject.Find("ColorDisplay" + (i + 1)).GetComponentInChildren<dfSlicedSprite>().Color;
            if (i == 0) c.a = 0;
            colors[i] = c;
        }

        for (int i = 0; i < ranges.Length; i++)
        {
            ranges[i] = GameObject.Find("Slider" + (i + 1)).GetComponentInChildren<dfSlider>().Value / 100f;
        }
        
                

        ColorRamp r = new ColorRamp(colors, ranges);
        planetInfo.colorRamp = r;

        return planetInfo;
    }
}
