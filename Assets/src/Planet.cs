using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
public class Planet : MonoBehaviour
{

   
    public GameState state;        
    public bool placeMode = false;
    private GameObject placeObject;
    public List<Building> placedBuildings;
    public Vector3 rotateVector = new Vector3(.1f, 1f, 0f);
    public float buildingRotation = 0;    
    public GameObject[] windZones;
    public PlanetInfo planetInfo;

    Thread thread;
    PlanetGenerator pg;
    PlanetGenerator randomPG;

        
    // Use this for initialization
    protected void Start()
    {
                
        state = GameState.Instance;
        
    }

    public void BeginGame()
    {
        
        state.energy = planetInfo.startEnergy;
        state.pollution = planetInfo.startPollution;
        state.gameLength = planetInfo.gameLength;
        state.yearsLeft = planetInfo.gameLength;
        state.iq = planetInfo.iq;
        state.population = planetInfo.population;
        state.pollutionDeathAmount = planetInfo.maxPollution;
        state.pollutionClearance = planetInfo.pollutionClearance;
        state.oilFactor = planetInfo.oilFactor;
        state.windFactor = planetInfo.windFactor;
        state.sunFactor = planetInfo.sunFactor;
        state.coalReserves = planetInfo.coalReserves;
    
        placedBuildings = new List<Building>();
        GenerateWindZones();
    }
    

    public void GeneratePlanet(int width, int height, PlanetInfo planetInfo, bool setSkybox = true)
    {
        this.planetInfo = planetInfo;
        GeneratePlanet(width,height);
        if (setSkybox) SetSkyBox(planetInfo.skybox);
        SetNormals(planetInfo.normals);
    }

    public void SetPlanet(int width, int height, Color[] colors, PlanetInfo planetInfo)
    {
        Texture2D planetTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        this.planetInfo = planetInfo;
        planetTex.SetPixels(colors);
        planetTex.Apply();
        renderer.material.mainTexture = planetTex;        
        SetWater(planetInfo.colorRamp.gradient[0]);
        
    }

    public static void SetSkyBox(string skybox)
    {        
        Material box = (Material)Resources.Load("SkyBox/"+skybox);
        RenderSettings.skybox = box;
    }

    public void SetNormals(string normals)
    {
        Texture2D normalMap = (Texture2D)Resources.Load("PlanetNormals/" + normals);
        this.renderer.material.SetTexture("_BumpMap", normalMap);
    }
    

    void GenerateWindZones()
    {
        float radius = this.transform.localScale.x / 2.0f + 1;
        windZones = new GameObject[planetInfo.windZones];

        for (int i = 0; i < windZones.Length; i++)
        {
            float theta = Random.Range(0f, Mathf.PI * 2f);
            float phi = Random.Range(0f, Mathf.PI);

            float x3d = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
            float y3d = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
            float z3d = radius * -Mathf.Cos(phi);

            GameObject tornado = (GameObject)Instantiate(Resources.Load("Prefabs/TornadoParticle"), new Vector3(x3d,y3d,z3d), Quaternion.identity);

            Ray planetRay = new Ray(tornado.transform.position, Vector3.zero - tornado.transform.position);

            RaycastHit[] hits = Physics.RaycastAll(planetRay);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject == transform.gameObject)
                {
                    Quaternion q = Quaternion.LookRotation(hit.normal);
                    tornado.transform.localRotation = q;
                }
            }

            windZones[i] = tornado;

        }
        
    }

    private void SetWater(Color c)
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            if (t.gameObject.name == "Water")
            {
                t.gameObject.renderer.material.color = new Color(c.r, c.g, c.b, 1);
            }
        }

    }

    private void GeneratePlanet(int width, int height)
    {
        
        Texture2D planetTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Texture2D cloudsTex = new Texture2D(width, height, TextureFormat.ARGB32, false);

        if (pg == null)
        {
            pg = new PlanetGenerator(width, height, true);
        }
        pg.generatePlanet(planetInfo);        
        cloudsTex.SetPixels(pg.GetCloudColors());
        cloudsTex.Apply();
      
        
        GameObject clouds = GameObject.Find("Clouds");
        if (clouds != null) clouds.renderer.material.mainTexture = cloudsTex;

        SetWater(planetInfo.colorRamp.gradient[0]);
      
        
        planetTex.SetPixels(pg.GetPlanetColors());
        planetTex.Apply();
        renderer.material.mainTexture = planetTex;
        pg.Finished();
    }

    




    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            
            if (state.oilScan && state.HasEnoughEnergyFor(state.scanCost))
            {
            
                state.UpdateOilMap();
                state.scanning = true;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            state.scanning = false;
        }

    }

    void FixedUpdate()
    {
        //transform.RotateAround (Vector3.zero, rotateVector, Time.deltaTime * rotateSpeed);
        if (placeMode)
        {
            CheckMousePosition();
        }

    }

  

    void OnMouseDown()
    {

    }

    void OnMouseUp()
    {


    }

    void CheckMousePosition()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {

            bool hitPlanet = false;
            RaycastHit planetHit = new RaycastHit();

            for (int i = 0; i < hits.Length; i++)
            {

                if (hits[i].transform.gameObject == transform.gameObject)
                {
                    hitPlanet = true;
                    planetHit = hits[i];
                    break;
                }
            }


            if (hitPlanet )
            {
                if (!state.scanning)
                {
                    if (Input.GetMouseButton(1))
                    {
                        buildingRotation = (buildingRotation + 100 * Time.deltaTime) % 360;
                    }
                }
                Texture2D surfaceTexture = (Texture2D)planetHit.transform.renderer.material.mainTexture;
                Color c = surfaceTexture.GetPixel((int)(planetHit.textureCoord.x * surfaceTexture.width), (int)(planetHit.textureCoord.y * surfaceTexture.height));
                Building b = (Building)placeObject.GetComponent(typeof(Building));
                b.CheckColor(c);

                Vector3 normal = planetHit.normal;
                Quaternion q = Quaternion.LookRotation(normal);
                placeObject.transform.localRotation = q;
                placeObject.transform.Rotate(90, 0, 0);
                placeObject.transform.Rotate(0, buildingRotation, 0);

                float adjustUpwards = (placeObject.transform.lossyScale.z / 2.0f) * .9f;
                placeObject.transform.position = planetHit.point;
                placeObject.transform.Translate(Vector3.up * adjustUpwards);

                //placeObject.rigidbody.MovePosition(planetHit.point);                
            }
            else
            {
                placeObject.transform.position = Vector3.zero;
            }
        }
        else
        {
            placeObject.transform.position = Vector3.zero;
        }
    }

    public void PlaceBuilding(Building b, Vector3 position)
    {
        print("placebuilding");
        placeMode = false;
        placeObject.transform.position = position;
        placeObject.transform.parent = transform;        
        placedBuildings.Add(b);
        placeObject = null;


        //PlanetGUI.Instance.state = (int)PlanetGUI.GUI_STATE.BUILD_OPTIONS;
    }

    public void RemoveBuilding(Building b)
    {
        placedBuildings.Remove(b);
        Destroy(b.gameObject);
    }

    public void PlaceMode(string buildingType)
    {
        //Material[] mats = new Material[2];
        //mats[0] = originalMat; mats[1] = gridMat;
        //renderer.materials = mats;				
        print("PLACE MODE!:" + buildingType);
        if (placeMode)
        {
            Destroy(placeObject);
            print("DESTRYOEDDDDD");
        }
        placeMode = true;
        placeObject = (GameObject)Instantiate(Resources.Load("Prefabs/buildings/" + buildingType), Vector3.zero, Quaternion.identity);

        if (placeObject == null)
        {
            print("PLACE OBJECT WAS NULLLLLLLLLLL");
            print(buildingType);
        }
        print("END PLACE MODE!!!");
    }



    public void CancelPlace()
    {
        print("cancelplace");
        Destroy(placeObject);
        placeObject = null;
        placeMode = false;
    }



}
