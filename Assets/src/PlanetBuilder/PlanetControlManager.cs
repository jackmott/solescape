using UnityEngine;
using System.Collections;
using System.Threading;

public class PlanetControlManager : MonoBehaviour
{

		public dfControl ColorPanel;
		public dfButton ColorDialogButton;
    
		static PlanetControlManager instance;

		int width = 2048;
		int height = 1024;

		private bool legit = true;

		private PlanetGenerator pg;
		private Texture2D planetTex;

		string[] skyboxes;
		string[] planetNormals;
		string[] userPlanets;

		Planet mp;
		GameObject planet;
    
		// Use this for initialization

		public static PlanetControlManager Instance {
				get {
						while (instance == null) {
								System.Threading.Thread.Sleep (1000);
								print ("PlanetControl instance");
						}
						return instance;
				}
		}


		void Awake ()
		{
				print ("AWAKE");
				instance = this;
				planetTex = new Texture2D (width, height, TextureFormat.ARGB32, false);
				pg = new PlanetGenerator (width, height, planetTex);
		}

		void Start ()
		{
        
				if (Utility.IsWeb ()) {
						dfButton saveButton = GameObject.Find ("SaveButton").GetComponent<dfButton> ();
						saveButton.Hide ();
						dfTextbox saveText = GameObject.Find ("NameTextbox").GetComponent<dfTextbox> ();
						saveText.Hide ();
				}
       
				planet = GameObject.Find ("Planet");       
				mp = planet.GetComponent<Planet> ();

				skyboxes = Utility.GetSkyboxes ();
				dfDropdown skyboxDropdown = GameObject.Find ("SkyBoxDropdown").GetComponent<dfDropdown> ();
				skyboxDropdown.Items = skyboxes;
				skyboxDropdown.SelectedIndex = 0;

				planetNormals = Utility.GetPlanetNormals ();
				dfDropdown normalDropdown = GameObject.Find ("NormalDropdown").GetComponent<dfDropdown> ();
				normalDropdown.Items = planetNormals;
				normalDropdown.SelectedIndex = 0;

				dfDropdown loadDropdown = GameObject.Find ("LoadDropdown").GetComponent<dfDropdown> ();
				if (!Utility.IsWeb ()) {    
						userPlanets = Utility.GetAllFilesInFolder ("config", "userplanet");
						loadDropdown.Items = userPlanets;
				} else {
						loadDropdown.Hide ();
				}

				RedrawPlanet ();
                
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (planet != null)
						planet.transform.RotateAround (planet.transform.position, Vector3.up, Time.deltaTime * 5);
				if (pg.IsReady ()) {
            
						mp.SetPlanet (width, height, pg.GetPlanetColors (), pg.GetPlanetInfo ());
						float size = (float)mp.planetInfo.planetSize * .016f;
						mp.transform.localScale = new Vector3 (size, size, size);
						pg.Finished ();
				}
		}

		public void OnPlay ()
		{
				Persistence p = GameObject.Find ("Persistence").GetComponent<Persistence> ();
				p.pi = mp.planetInfo;
				Application.LoadLevel ("Planet");
		}

		public void OnSave ()
		{
				dfTextbox saveText = GameObject.Find ("NameTextbox").GetComponent<dfTextbox> ();
				if (saveText.Text == "") {
						print ("no planet name!");
				} else {
						mp.planetInfo.planetName = saveText.Text;
						print ("about to save");
						mp.Save ();
						print ("saved");
						Notification.Instance.SetNotification ("Planet Saved");
						print ("notification set");
				}
		}

		public void OnLoadChanged (dfControl control, int index)
		{
				mp.Load (userPlanets [index]);
				pg.planetInfo = mp.planetInfo;
				Thread thread = new Thread (new ThreadStart (pg.startPlanetInfo));
				thread.Start ();

		}
				
		public void OnSkyBoxChanged (dfControl control, int index)
		{
        
				Planet.SetSkyBox (skyboxes [index]);
				if (mp.planetInfo != null) {
						mp.planetInfo.skybox = skyboxes [index];
				}
        
		}

		public void OnNormalChanged (dfControl control, int index)
		{
				mp.SetNormals (planetNormals [index]);
				if (mp.planetInfo != null) {
						mp.planetInfo.normals = planetNormals [index];
				}
		}

	

		public void OnColorMouseUp (dfControl control, dfMouseEventArgs mouseEvent)
		{
				ColorPanel.Show ();
				ColorDialogButton buttonScript = ColorDialogButton.GetComponent<ColorDialogButton> ();
				buttonScript.colorToSet = (dfSlicedSprite)control.Find ("Color");
		}

		public void OnSliderChanged (dfControl control, float value)
		{
				if (legit) {
						legit = false;
				} else {
						return;
				}
				dfSlider[] sliders = new dfSlider[5];
				int currIndex = 0;
				int sum = 0;

				//get the sum
				for (int i = 0; i < sliders.Length; i++) {
						sliders [i] = GameObject.Find ("Slider" + (i + 1)).GetComponent<dfSlider> ();
						if (sliders [i] == control)
								currIndex = i;
						sum += (int)sliders [i].Value;
				}

				//check if sum adds up to one hunnered
				if (sum < 100) {
						int adjustIndex = (currIndex + 1) % sliders.Length;            
						sliders [adjustIndex].Value += 100 - sum;            
				} else if (sum > 100) {
						int adjustIndex = (currIndex + 1) % sliders.Length;
						do {
								print ("sum:" + sum);
								int adjustmentNeeded = sum - 100;
								sum = (int)(sum - sliders [adjustIndex].Value);
								int adjustmentAvailable = (int)sliders [adjustIndex].Value;
								print ("Slider preadjusted val:" + sliders [adjustIndex].Value);
								sliders [adjustIndex].Value -= Mathf.Min (adjustmentNeeded, adjustmentAvailable);
								print ("Slider adjusted val:" + sliders [adjustIndex].Value);
								sum = (int)(sum + sliders [adjustIndex].Value);
								adjustIndex = (adjustIndex + 1) % sliders.Length;
						} while (sum > 100);
				}

				legit = true;
		}

   
		public void RedrawPlanet ()
		{
        
				pg.planetInfo = CreateInfo ();
				Thread thread = new Thread (new ThreadStart (pg.startPlanetInfo));
				thread.Start ();
                        
		}

    

		public PlanetInfo CreateInfo ()
		{
				PlanetInfo planetInfo = new PlanetInfo ();
				planetInfo.skybox = skyboxes [GameObject.Find ("SkyBoxDropdown").GetComponent<dfDropdown> ().SelectedIndex];
				planetInfo.normals = planetNormals [GameObject.Find ("NormalDropdown").GetComponent<dfDropdown> ().SelectedIndex];
				planetInfo.coalReserves = 10000 * GameObject.Find ("CoalSlider").GetComponentInChildren<dfSlider> ().Value;
				planetInfo.oilFactor = GameObject.Find ("OilSlider").GetComponentInChildren<dfSlider> ().Value;
				;
				planetInfo.windFactor = GameObject.Find ("WindSlider").GetComponentInChildren<dfSlider> ().Value;
				;
				planetInfo.sunFactor = GameObject.Find ("SunSlider").GetComponentInChildren<dfSlider> ().Value;
				;
				planetInfo.pollutionClearance = 20;
				planetInfo.startPollution = 0;
				planetInfo.maxPollution = 5000;
				planetInfo.startEnergy = 20;
				planetInfo.population = 100;
				planetInfo.iq = 1;
				planetInfo.gameLength = 1000;
				planetInfo.planetSize = (int)GameObject.Find ("SizeSlider").GetComponentInChildren<dfSlider> ().Value;
				print (planetInfo.planetSize);
				planetInfo.rotationSpeed = 1;
				planetInfo.windZones = 3;
				planetInfo.octaves = (int)GameObject.Find ("OctavesSlider").GetComponentInChildren<dfSlider> ().Value;
				planetInfo.gain = GameObject.Find ("GainSlider").GetComponentInChildren<dfSlider> ().Value;
				planetInfo.lacunarity = GameObject.Find ("LacunaritySlider").GetComponentInChildren<dfSlider> ().Value;
				planetInfo.stretch = GameObject.Find ("StretchSlider").GetComponentInChildren<dfSlider> ().Value;

				Color[] colors = new Color[6];
				float[] ranges = new float[5];

				for (int i = 0; i < colors.Length; i++) {
						Color c = GameObject.Find ("ColorDisplay" + (i + 1)).GetComponentInChildren<dfSlicedSprite> ().Color;
						if (i == 0)
								c.a = 0;
						colors [i] = c;
				}

				for (int i = 0; i < ranges.Length; i++) {
						ranges [i] = GameObject.Find ("Slider" + (i + 1)).GetComponentInChildren<dfSlider> ().Value / 100f;
				}
        
                

				ColorRamp r = new ColorRamp (colors, ranges);
				planetInfo.colorRamp = r;

				return planetInfo;
		}
}
