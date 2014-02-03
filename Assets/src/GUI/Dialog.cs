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

    //Set this to cover the whole screen
    public dfPanel dimmerPanel;

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
        dialogTitle.Text = title;
        dialogText.Text = text;
        dialogButton.Text = buttonText;
        

        //Set the dimmer panel to dark the background
        dimmerPanel.BackgroundColor = new Color(0, 0, 0, .7f);

        //Set interactive to prevent button clicks
        dimmerPanel.IsInteractive = true;
        dimmerPanel.Show();

        //bring the dimmer panel to the front, then the dialog on top of it
        dimmerPanel.BringToFront();
        dialogPanel.BringToFront();

        if (prefab != null)
        {
            guiCamera = (GameObject)Instantiate(guiCameraPrefab, Vector3.zero, Quaternion.identity);
            guiCameraLight = (GameObject)Instantiate(guiCameraLightPrefab, Vector3.zero, Quaternion.identity);
            renderObject = (GameObject)Instantiate(prefab, guiCamera.transform.position + guiCamera.transform.forward * 45, Quaternion.identity);
            //renderObject.transform.Rotate(100, 0, 0);
            timestamp = System.DateTime.Now;
            dialogImage.Show();

        }
        else
        {
            dialogImage.Hide();
        }
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

        print("CLOSE DIALOG");
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
        dimmerPanel.BackgroundColor = new Color(0, 0, 0, 0);
        dimmerPanel.IsInteractive = false;
        dimmerPanel.Hide();

        exitButton.Hide();
        restartButton.Hide();

        Time.timeScale = 1;

        if (endGame)
        {
            Application.LoadLevel(0);
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
