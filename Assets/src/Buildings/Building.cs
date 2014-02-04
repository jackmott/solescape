using System;
using UnityEngine;
using System.Collections;

public abstract class Building : MonoBehaviour
{


    public string buildingName;
    public float cost;
    public float energy;
    protected float lastEnergy;
    public float pollution;
    protected float lastPollution;
    public float iqFactor;
    public int population;
    public string upgrade;
    public float upgradeCost;
    public string[] researchDependencies;

    public int collisionCount = 0;


    private string upgradeText;
    BuildingInfo upgradeBi = null;

    protected int buttonWidth = 120;
    protected int buttonHeight = 20;

    private Color blinkSwapColor = Color.red;

    protected GUIStyle style;


    protected bool showGui = false;
    protected bool showHoverGui = false;

    protected float menuX;
    protected float menuY;


    protected GameState state;


    protected bool validSurface = true;

    public bool placed = false;

    protected bool isEnabled = true;


    protected int yOffset;
    protected int yOffsetAmount;
    protected Vector3 guiPos;

    public float startTime;

    private TickManager tickManager;

    protected virtual void Awake()
    {
        tickManager = (TickManager)FindObjectOfType(typeof(TickManager));
        tickManager.lateTick += new Action(LateFixedUpdate);
        startTime = Time.time;
        gameObject.AddComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>("sound/ground_impact");
        audio.maxDistance = 100000;
        audio.rolloffMode = AudioRolloffMode.Linear;
        audio.volume = 1f;
        state = GameState.Instance;
        style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 16;
        LoadInfo();
    }

    protected virtual void Start()
    {


    }

    protected virtual void Blink()
    {
        Color c = renderer.material.color;
        renderer.material.color = blinkSwapColor;
        blinkSwapColor = c;

    }


    protected virtual void Update()
    {

    }

    protected virtual bool IsAllValid()
    {

        if (collisionCount == 0 && validSurface && isEnabled)
            return true;
        else
            return false;
    }

    protected virtual void LateFixedUpdate()
    {
        
        if (Input.GetMouseButton(0) && !placed)
        {
            float firstTurnEnergy = 0;
            if (energy < 0)
            {
                firstTurnEnergy = -energy;
            }
            if (transform.position == Vector3.zero)
            {
                print("don't place, im in the middle of a planet");
            }
            else if (state.HasEnoughEnergyFor(cost+firstTurnEnergy) && IsAllValid())
            {
                Place(transform.position);
            }
            else if (collisionCount > 0)
            {
                //nothin
            }
            else if (!validSurface)
            {
                Notification.Instance.SetNotification("Invalid Placement");
            }
            else if (!state.HasEnoughEnergyFor(cost))
            {
                print("COST:" + cost + " stateenergy:" + state.energy);
                Notification.Instance.SetNotification("Not Enough Energy");
            }
           

        }
    }

    protected virtual void FixedUpdate()
    {



    }

    protected virtual void LateUpdate()
    {


    }

    private void LoadInfo()
    {
        string classname = this.GetType().Name;
        BuildingInfo bi = (BuildingInfo)state.buildings[classname];
        this.buildingName = bi.buildingName;
        this.cost = bi.cost;
        this.energy = bi.energy;
        this.pollution = bi.pollution;
        this.iqFactor = bi.iqFactor;
        this.population = bi.population;
        this.researchDependencies = bi.researchDependencies;
        this.upgrade = bi.upgrade;
        this.upgradeCost = bi.upgradeCost;
        if (upgrade != "")
        {
            upgradeBi = (BuildingInfo)state.buildings[upgrade];
            upgradeText = "Upgrade to\n" + upgradeBi.buildingName + "\n(" + upgradeCost.ToString("0") + ")";
        }

    }


    //only energy consumers should use this
    public virtual void AffectState()
    {

        if (!placed || !isEnabled)
            return;

        lastEnergy = energy;
        lastPollution = pollution;


        if (state.HasEnoughEnergyFor(-energy))
        {
            state.AddEnergy(energy);
            state.AddPollution(pollution);
        }
        else
        {
            Disable();

        }


    }

    public virtual void CheckColor(Color c)
    {
        if (c.g < c.b)
        {
            validSurface = false;
            SetRenderTransparent();
        }
        else
        {
            validSurface = true;
            SetRenderOpaque();
        }

    }

    protected virtual void OnMouseEnter()
    {

        if (placed && !state.oilScan)
        {
            showHoverGui = true;
        }
    }

    protected virtual void OnMouseExit()
    {
        if (placed)
        {
            showHoverGui = false;
        }
    }

    protected virtual void OnMouseDown()
    {

    }
    protected virtual void OnMouseUp()
    {

        if (!state.planet.placeMode && !state.oilScan && placed)
        {

            menuX = Input.mousePosition.x;
            menuY = Screen.height - Input.mousePosition.y;
            showGui = !showGui;
        }
    }

    protected virtual void Place(Vector3 position)
    {

        audio.Play();
        state.AddEnergy(-cost);
        state.population += population;
        state.iq += iqFactor;
        state.planet.PlaceBuilding(this, position);
        placed = true;
        //don't place another if scanning
        state.planet.PlaceMode(this.GetType().Name);

    }

    protected virtual void OnGUI()
    {
        if (!placed && state.planet.placeMode && collisionCount == 0)
        {
            if (transform.position == Vector3.zero) return;
            guiPos = Camera.main.WorldToScreenPoint(transform.position);
            yOffset = -20;
            yOffsetAmount = 15;

            GUI.Box(new Rect(guiPos.x + 22, Screen.height - guiPos.y + yOffset - 5, 125, 50), "");
            GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), buildingName, style);
            if (validSurface)
            {
                GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset + yOffsetAmount, 100, 20), "Cost:" + cost, style);
            }
            else
            {
                GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset + yOffsetAmount, 100, 20), "Can't Place Here", style);
            }
        }
        else if (showGui)
        {
            //if this building can be upgraded and the upgrade is available
            if (upgradeBi != null && state.availableBuildings[upgradeBi.buildingName] != null)
            {
                //then show the upgrade button
                if (GUI.Button(new Rect(menuX, menuY - 50, buttonWidth, 50), upgradeText))
                {
                    Vector3 position = this.transform.position;
                    Quaternion rotation = this.transform.rotation;
                    state.planet.placedBuildings.Remove(this);
                    Destroy(this.gameObject);
                    GameObject upgradeObject = (GameObject)Instantiate(Resources.Load("prefabs/buildings/" + upgradeBi.className), position, rotation);
                    upgradeObject.audio.Play();
                    upgradeObject.transform.parent = state.planet.transform;
                    upgradeObject.rigidbody.mass = 999999;
                    Building b = upgradeObject.GetComponent<Building>();
                    b.placed = true;
                    state.planet.placedBuildings.Add(b);
                    state.energy -= upgradeCost;
                    print("pop:" + population + " bpop:" + b.population);
                    state.population += b.population;
                    state.iq += b.iqFactor;

                }
            }

            if (GUI.Button(new Rect(menuX, menuY, buttonWidth, buttonHeight), "Cancel"))
            {
                showGui = false;
            }
            if (GUI.Button(new Rect(menuX, menuY + buttonHeight, buttonWidth, buttonHeight), "Destroy"))
            {
                state.planet.RemoveBuilding(this);
            }

            if (!isEnabled)
            {
                if (GUI.Button(new Rect(menuX, menuY + buttonHeight * 2, buttonWidth, buttonHeight), "Enable"))
                {
                    Enable();
                    showGui = false;
                }
            }
            else
            {
                if (GUI.Button(new Rect(menuX, menuY + buttonHeight * 2, buttonWidth, buttonHeight), "Disable"))
                {
                    Disable();
                    showGui = false;
                }

            }

        }
        else if (showHoverGui)
        {

            guiPos = Camera.main.WorldToScreenPoint(transform.position);


            yOffset = -20;
            yOffsetAmount = 15;

            GUI.Box(new Rect(guiPos.x + 22, Screen.height - guiPos.y + yOffset - 5, 200, 60), "");


            GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), buildingName, style);
            yOffset += yOffsetAmount;
            if (this.isEnabled)
            {
                if (energy > 0)
                {
                    GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), "Energy Production:" + lastEnergy.ToString("0.00"), style);
                    yOffset += yOffsetAmount;
                }
                else if (energy < 0)
                {
                    GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), "Energy Consumption:" + (-energy).ToString("0.00"), style);
                    yOffset += yOffsetAmount;
                }
                if (pollution > 0)
                {
                    GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), "Pollution Output:" + lastPollution.ToString("0.00"), style);
                    yOffset += yOffsetAmount;
                }
                else if (pollution < 0)
                {
                    GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), "Pollution Absorbption:" + (-lastPollution).ToString("0.00"), style);
                    yOffset += yOffsetAmount;
                }
            }
            else if (energy < 0)
            {
                GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), "Out of Energy", style);
                yOffset += yOffsetAmount;
            }


        }
    }

    protected virtual void OnDestroy()
    {
        if (placed && isEnabled)
        {
            state.iq -= iqFactor;
            state.population -= population;
            state.populationKilled += population;
        }
        tickManager.lateTick -= new Action(LateFixedUpdate);

    }

    void OnCollisionEnter(Collision collision)
    {

        if (placed)
            return;
        //print ("enter collision");
        if (collision.collider.gameObject.GetComponent(typeof(Building)) != null)
        {
            collisionCount = collisionCount + 1;
            SetRenderTransparent();

        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (placed)
            return;
        //print ("exit collision");
        if (collision.collider.gameObject.GetComponent(typeof(Building)) != null)
        {
            collisionCount = collisionCount - 1;
            if (collisionCount == 0)
                SetRenderOpaque();
        }
    }

    void OnCollisionStay(Collision collision)
    {

        if (placed)
            return;
        //print ("enter collision");
        if (collision.collider.gameObject.GetComponent(typeof(Building)) != null)
        {
            SetRenderTransparent();
        }

    }

    protected virtual void Enable()
    {
        if (state.HasEnoughEnergyFor(-energy))
        {
            isEnabled = true;
            state.iq += iqFactor;
            state.population += population;
            CancelInvoke("Blink");
            if (renderer.material.color == Color.red)
            {
                renderer.material.color = blinkSwapColor;
                blinkSwapColor = Color.red;
            }
        }

    }

    protected virtual void Disable()
    {
        isEnabled = false;
        InvokeRepeating("Blink", 0f, .5f);
        state.iq -= iqFactor;
        state.population -= population;
    }

    protected void SetRenderOpaque()
    {
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
        {
            r.enabled = true;
        }




    }
    protected void SetRenderTransparent()
    {
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
        {
            r.enabled = false;
        }

    }


}
