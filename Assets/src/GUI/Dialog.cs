using UnityEngine;
using System.Collections;



public class Dialog : MonoBehaviour
{
    //root dialog object
    public dfPanel  dialogPanel;

    //children
    public dfLabel  dialogTitle;
    public dfLabel  dialogText;
    public dfButton dialogButton;
    public dfTextureSprite  dialogImage;
    public dfButton exitButton;
    public dfButton restartButton;

    

    public GameObject guiCameraPrefab;
    public GameObject guiCameraLightPrefab;

    private GameObject guiCamera = null;
    private GameObject guiCameraLight = null;
    private GameObject renderObject = null;

    
    

    public System.DateTime timestamp;
    
    //Start game over on ButtonClick?
    private bool endGame;
    
    //Singleton
    private static Dialog instance;
    

    void Awake()
    {
        instance = this;
    }


    public static Dialog Instance
    {
        get
        {            
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {
        exitButton.Hide();
        restartButton.Hide();

    }

    public void PauseMenu(string title, string text, string buttonText, bool pause, bool endGame, GameObject prefab = null)
    {
        exitButton.Show();
        restartButton.Show();
        SetDialog(title, text, buttonText, pause, endGame, prefab);
        
    }


    public void SetDialog(string title, string text, string buttonText, bool pause, bool endGame, GameObject prefab = null)
    {
        

        
        

        if (prefab != null)
        {
            guiCamera = (GameObject)Instantiate(guiCameraPrefab, Vector3.zero, Quaternion.identity);
            guiCameraLight = (GameObject)Instantiate(guiCameraLightPrefab, Vector3.zero, Quaternion.identity);
            renderObject = (GameObject)Instantiate(prefab, guiCamera.transform.position + guiCamera.transform.forward * 32, Quaternion.identity);
            renderObject.transform.Translate(guiCamera.transform.up * -9);
            Building b = renderObject.GetComponent<Building>();
            if (b != null)
            {
                text = text + "\n\n" + b.StatsText();
            }
            //renderObject.transform.Rotate(100, 0, 0);
            timestamp = System.DateTime.Now;
            dialogImage.Parent.Show();
            dialogImage.Show();

        }
        else
        {
            dialogImage.Parent.Hide();
            dialogImage.Hide();
        }

        dialogTitle.Text = title;
        dialogText.Text = text;
        dialogButton.Text = buttonText;
        

        dialogButton.Focus();
        //show the dialog and pause time if desired
        dialogPanel.Show();            
        if (pause)
        {
            Time.timeScale = 0;
         }                      
        this.endGame = endGame;
     }

    public void CloseDialog()
    {

        
        if (guiCamera != null)
        {
            Destroy(guiCamera);
            guiCamera = null;
        }

        if (guiCameraLight != null)
        {
            Destroy(guiCameraLight);
            guiCameraLight = null;
        }

        if (renderObject != null)
        {
            Destroy(renderObject);
            renderObject = null;
        }

        dialogPanel.Hide();
        
        exitButton.Hide();
        restartButton.Hide();

        Time.timeScale = 1;

        if (endGame)
        {
            ShowStatsDialog();
        }
       
    }

    public void ShowStatsDialog()
    {
        
        
        
        dialogImage.Parent.Hide();
        dialogImage.Hide();
        

        dialogTitle.Text = "Planet Stats";
        dialogText.Text = "";
        dialogButton.Text = "Try Again";


        dialogButton.Hide();
        restartButton.Show();
        exitButton.Show();

        GameStats[] stats = GameState.Instance.gameStats;
        float[] energy = new float[stats.Length];
        float[] pollution = new float[stats.Length];
        float[] population = new float[stats.Length];
        float[] iq = new float[stats.Length];
        float[] buildings = new float[stats.Length];
        for (int i = 0; i < stats.Length; i++)
        {
            energy[i] = stats[i].energy;
            pollution[i] = stats[i].pollution;
            iq[i] = stats[i].iq;
            population[i] = stats[i].population;
            buildings[i] = 0;
            foreach (System.Collections.Generic.KeyValuePair<string,int> pair in stats[i].buildings)
            {
                print(pair.Key + " - " + pair.Value);
                buildings[i] += pair.Value;
            }

            
        }

        float x = Screen.width * .2f;
        float y = Screen.height * .2f;
        float width = Screen.width * .6f;
        float height = Screen.height * .6f;
        Rect r = new Rect(x, y, width, height);

        Graph.graph(energy, Color.red, r);
        Graph.graph(pollution, Color.yellow,r);
        Graph.graph(population, Color.blue, r);
        Graph.graph(iq, Color.green, r);
        Graph.graph(buildings, Color.white, r);

        
        Time.timeScale = 0;
        
        
        
    }

    public void OnGUI()
    {
        if (dialogTitle.Text == "Planet Stats")
        {
            int startX = 100;
            int startY = Screen.height/2+100;
            int yPos = 0;
            int yInc = 16;

            GuiEventHandler.Instance.countDownLabel.Text = "Game Statistics";

            GUIStyle style = new GUIStyle();
            style.fontSize = 16;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.red;
            GUI.Label(new Rect(startX, startY+yPos, 200, 15), "Energy", style);
            
            style.normal.textColor = Color.yellow;
            yPos += yInc;
            GUI.Label(new Rect(startX, startY + yPos, 200, 15), "Pollution", style);

            style.normal.textColor = Color.blue;
            yPos += yInc;
            GUI.Label(new Rect(startX, startY + yPos, 200, 15), "Population", style);

            style.normal.textColor = Color.green;
            yPos += yInc;
            GUI.Label(new Rect(startX, startY + yPos, 200, 15), "IQ", style);

            style.normal.textColor = Color.white;
            yPos += yInc;
            GUI.Label(new Rect(startX, startY + yPos, 200, 15), "Buildings", style);

        }
    }

    public void Restart()
    {
        Application.LoadLevel("Planet");
    }

    public void Exit()
    {
        Application.LoadLevel("MainMenu");
    }

    

    void Update()
    {
       

        if (renderObject != null)
        {
            System.TimeSpan timeSpan = System.DateTime.Now - timestamp;
            renderObject.transform.Rotate(new Vector3(0, 1, 0), 25f * (float)timeSpan.TotalSeconds);
            timestamp = System.DateTime.Now;
        }

       
    }
    

    


    //When the "ok" button is pressed
    public void ButtonPressed()
    {
        CloseDialog();
    }                     
          
}
