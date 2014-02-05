using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiEventHandler : MonoBehaviour {

    public static GuiEventHandler instance;

    public dfScrollPanel mainButtonPanel;
    public dfScrollPanel buildButtonPanel;
    public dfScrollPanel researchButtonPanel;

    public dfButton prefabBuildingButton;
    public dfButton prefabResearchButton;
    public dfButton prefabHelpButton;

    public dfButton buildButton;
    public dfButton researchButton;

    public dfButton pauseResearchButton;
    public dfButton cancelResearchButton;

    public dfButton scanButton;

    public dfButton muteButton;

    public bool muted = false;

    public dfLabel energyBalanceLabel;
    public dfLabel countDownLabel;

    


    public GameState state;

    
    public dfProgressBar researchProgressBar;

    public static GuiEventHandler Instance
    {
        get
        {
            while (instance == null)
            {
                System.Threading.Thread.Sleep(1000);
                print("GuiEventHandler instance");
            }
            return instance;
        }
    }

    public string energy
    {
        get
        {
            
            if (state == null) return "";
            else return state.GetEnergy().ToString("0.0");
        }
        private set { }
    }

    public string coal
    {
        get
        {

            if (state == null) return "";
            else return state.coalReserves.ToString("0");
        }
        private set { }
    }

    public bool isResearchHappening
    {
        get
        {
            if (state == null) return false;
            if (state.pendingResearch != null) return true;
            else return false;
        }
        private set { }
    }

    public bool isPollutionLive
    {
        get
        {
          
            if (state == null) return false;
            if (state.attainedResearch["Climate Science"] != null) return true;
            else return false;
        }
        private set { }
    }

    public bool isCoalLive
    {
        get
        {

            if (state == null) return false;
            if (state.attainedResearch["Geology"] != null) return true;
            else return false;
        }
        private set { }
    }

    public string pollution
    {
        get
        {

            if (state == null) return "";
            else return (state.pollution / state.pollutionDeathAmount).ToString("p");
        }
        private set { }
    }

    public string iq
    {
        get
        {

            if (state == null) return "";
            else return (state.iq * 100).ToString("0");
        }
        private set { }
    }

    public string population
    {
        get
        {
            
            if (state == null) return "";
            else return state.population.ToString();
        }
        private set { }
    }

    public string energyBalance
    {
        get
        {
            if (state == null) return "";
            else
            {
                float balance = state.GetEnergy() - state.previousEnergy;
                if (balance >= 5) energyBalanceLabel.Color = Color.green;
                else if (balance < 0) energyBalanceLabel.Color = Color.red;
                else energyBalanceLabel.Color = Color.white;
                return balance.ToString("0.0");
            }
        }
        private set { }
    }

    public string yearsLeft
    {
        get
        {
            if (state != null)
            {
                return state.yearsLeft.ToString();
            }
            else
            {
                return "";
            }
        }
        private set {  }
    }

    public float researchProgress
    {
        
        get {
            
            if (state == null) return 0;

            if (state.pendingResearch != null)
            {

                return state.pendingResearch.completion / state.pendingResearch.iqCost;

            }
            else
            {
                return 0;
            }
           
        }
        private set { }
    }

    public string researchStatus
    {
        
        get
        {
            
            if (state == null) {
            
                return "Research Idle";
            }
            else if (state.pendingResearch == null) {
            
                return "Research Idle";
            }
            else if (state.pendingResearch.energyCost > state.energy)
            {
                return "Energy Low!";
            }
            else {   
                return state.pendingResearch.guiLabel;
            }

        }
        private set { }
    
    }

    void Awake()
    {
        print("GUI AWAKE");        
        instance = this;
    }
    
    void Start()
    {

        state = GameState.Instance;                       
    }

    public void WeatherScanButtonClick()
    {


        GameObject radar = (GameObject)Instantiate(Resources.Load("prefabs/RadarPlane"),Vector3.zero, Quaternion.identity);
        radar.transform.parent = Camera.main.transform;

        GameObject radarBG = (GameObject)Instantiate(Resources.Load("prefabs/RadarBackgroundPlane"), Vector3.zero, Quaternion.identity);
        radarBG.transform.parent = Camera.main.transform;


        foreach (GameObject o in state.planet.windZones)
        {
            Tornado t = o.GetComponent<Tornado>();
            t.Show();
        }

        
       
    }

    public void ScanButtonClick()
    {
        if (state.oilScan)
        {
            scanButton.Text = "Seismic Scan";
            state.oilScan = false;        
        }
        else
        {
            state.oilScan = true;
            GameState.Instance.planet.PlaceMode("OilProducer");
            researchButtonPanel.Hide();
            buildButtonPanel.Hide();
            mainButtonPanel.Show();                   
            scanButton.Text = "End Scan";
            Notification.Instance.SetNotification("Right Click To Scan\nLeft Click To Place Oil Well");

        }
       
    }

    public void MuteButtonClick()
    {
        if (muted)
        {
            muteButton.Text = "Mute";
            muted = false;
        }
        else
        {
            muteButton.Text = "UnMute";
            muted = true;
        }

        
    }

    public void BuildingBackClick()
    {
        state.planet.CancelPlace();
    }


    public void AddBuilding(BuildingInfo bi)
    {
        
        if (bi.type == 0)
        {
            dfButton button = (dfButton)Instantiate(prefabBuildingButton);
            dfButton helpButton = (dfButton)Instantiate(prefabHelpButton);

            button.Text = bi.ButtonText();

            BuildingInfoContainer container = button.GetComponent<BuildingInfoContainer>();
            container.bi = bi;
                        
            buildButtonPanel.AddControl(button);            
            button.AddControl(helpButton);

            HelpButton helpScript = helpButton.GetComponent<HelpButton>();
            helpScript.bi = bi;

            helpButton.RelativePosition = new Vector3(button.Width - helpButton.Width - 8, button.Height - helpButton.Height - 2, 0);
            helpButton.BringToFront();
            
        }
    }

    public void AddResearch(Research r)
    {

        if (state == null) state = GameState.Instance;
        dfButton button = (dfButton)Instantiate(prefabResearchButton);
        //button.Text = r.ButtonText(state.population,state.iq);
        ResearchContainer container = button.GetComponent<ResearchContainer>();
        container.r = r;
        container.state = state;
        button.Text = r.ButtonText(state.population,state.iq);

      

        researchButtonPanel.AddControl(button);
        
    }

    public void RemoveResearch(Research r)
    {
        print("Remove Research");
        foreach (dfButton button in researchButtonPanel.GetComponentsInChildren<dfButton>())
        {
            if (button.Text == r.ButtonText(state.population,state.iq))
            {
                researchButtonPanel.RemoveControl(button);
                Destroy(button.gameObject);
                break;
            }
        }
    }

    public void CancelResearch()
    {
        state.CancelResearch();
    }

    public void PauseResearch()
    {
        if (state.pendingResearch.paused)
        {
            state.pendingResearch.paused = false;
            pauseResearchButton.Text = "Pause";
        }
        else
        {
            state.pendingResearch.paused = true;
            pauseResearchButton.Text = "Resume";
        }
        
    }


    public void BuildClick()
    {
        mainButtonPanel.Hide();
        buildButtonPanel.Show();
        scanButton.Text = "Seismic Scan";
        state.oilScan = false;
        state.planet.CancelPlace();                        
    }
    public void ResearchClick()
    {
        mainButtonPanel.Hide();
        researchButtonPanel.Show();
        scanButton.Text = "Seismic Scan";
        state.oilScan = false;
        state.planet.CancelPlace();        

    }

    void Update()
    {
        if (!researchButton.IsVisible && Time.time > 1)
        {
            researchButton.Show();
        }

        if (muted && !AudioListener.pause)
        {
            AudioListener.pause = true;            
        }
        else if (!muted && AudioListener.pause)
        {
            AudioListener.pause = false;            
        }

      
    }

    void OnGUI()
    {
     

        if (Time.timeScale == 1)
        {


            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("KEYDOWN GUI");
                Dialog.Instance.PauseMenu("PAUSED", "Think hard, plan for the future, and you might just make it out of here alive", "Continue", true, false);
            }
        }
    }


}
