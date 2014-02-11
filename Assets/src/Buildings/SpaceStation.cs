using UnityEngine;
using System.Collections;

public class SpaceStation : SpaceBuilding
{




    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = (energy * -1).ToString("0");        
        string stats = "     Cost: " + costText + "\n";
        stats += "     Energy Consumption: " + energyText + " units\n";        
        return stats;
    }
  
   
    protected override void FixedUpdate()
    {
        float distance = (transform.position - state.planet.transform.position).magnitude;
        if (distance < 500)
        {
            transform.rigidbody.AddForce(transform.forward * 50);
            transform.rigidbody.AddForce(transform.right * 50);
            //transform.rotation = Quaternion.LookRotation (rigidbody.velocity);
        }
        else
        {
            if (transform.rigidbody != null)
            {

                Vector3 movingDirection = transform.rigidbody.GetPointVelocity(transform.parent.position);
                rotateAngle = Vector3.Cross(movingDirection, transform.right);
                Destroy(transform.rigidbody);
                transform.collider.enabled = true;
                transform.LookAt(transform.parent.transform.position);

            }

            transform.RotateAround(transform.parent.position, rotateAngle, Time.deltaTime);

        }


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
                if (bi.type == 2)
                {  //if this is a launch facility buidling
                    if (GUI.Button(new Rect(menuX, menuY - count * buttonHeight*2, buttonWidth, buttonHeight*2), bi.ButtonText()))
                    {
                        if (state.HasEnoughEnergyFor(bi.cost))
                        {
                            state.AddEnergy(-bi.cost);
                            Vector3 launchVector = transform.position;
                            GameObject launchObject = (GameObject)Instantiate(Resources.Load("Prefabs/buildings/" + bi.className), launchVector, transform.rotation);
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

 

    public override void AffectState()
    {
        base.AffectState();

       
        if (!placed || !isEnabled)
            return;

        
        lastEnergy = energy;
        state.AddEnergy(energy);

    }
}
