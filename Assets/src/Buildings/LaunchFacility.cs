using UnityEngine;
using System.Collections;

public class LaunchFacility : Building
{




    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = (energy * -1).ToString("0");        
        string pollutionText = pollution.ToString("0");
        string stats = "     Cost: " + costText + "\n";
        stats +=       "     Energy Consumption: " + energyText + " units\n";
        stats +=       "     Polllution: " + pollutionText + "\n";        
        return stats;
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
                            SpaceBuilding b = launchObject.GetComponent<SpaceBuilding>();
                            b.supplyFacility = this;
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
