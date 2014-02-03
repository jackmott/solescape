using UnityEngine;
using System.Collections;

public class LaunchFacility : Building
{



    protected override void Awake()
    {
        base.Awake();
        //DO SOME STUFF		
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();

    }



    protected override void OnGUI()
    {
        base.OnGUI();
        if (showGui)
        {
            int count = 1;
            foreach (DictionaryEntry e in state.availableBuildings)
            {
                BuildingInfo bi = (BuildingInfo)e.Value;
                if (bi.type == 1)
                {  //if this is a launch facility buidling
                    if (GUI.Button(new Rect(menuX, menuY - count * buttonHeight*2, buttonWidth, buttonHeight*2), bi.ButtonText()))
                    {
                        if (state.HasEnoughEnergyFor(bi.cost))
                        {
                            state.AddEnergy(-bi.cost);
                            Vector3 launchVector = transform.position;
                            GameObject launchObject = (GameObject)Instantiate(Resources.Load("prefabs/buildings/" + bi.className), launchVector, transform.rotation);
                            launchObject.transform.parent = state.planet.transform;
                            Building b = launchObject.GetComponent<Building>();
                            state.planet.placedBuildings.Add(b);
                            showGui = false;
                        }
                    }
                    count++;
                }
            }
        }
    }



}
