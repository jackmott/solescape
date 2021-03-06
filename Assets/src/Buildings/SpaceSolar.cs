﻿using UnityEngine;
using System.Collections;

public class SpaceSolar : SpaceBuilding
{


  

    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = energy.ToString("0");
        string stats = "           Cost: " + costText + "\n";
        stats +=       "           Energy Production: " + energyText + " units\n";
        return stats;
    }

    

    public override void AffectState()
    {
        base.AffectState();

        if (!placed || !isEnabled)
            return;
        
        //check if we are in sunlight        
        float energyProduction = state.sunFactor * energy;
        lastEnergy = energyProduction;
        state.AddEnergy(energyProduction);
        
    }

    protected override void FixedUpdate()
    {
        
        float distance = (transform.position - state.planet.transform.position).magnitude;
        if (distance < 375 && transform.rigidbody != null)
        {
            transform.rigidbody.AddForce(transform.up * 50);
            transform.rigidbody.AddForce(transform.forward * 50);
            //transform.rotation = Quaternion.LookRotation (rigidbody.velocity);
        }
        else
        {
            if (transform.rigidbody != null)
            {
                print("destoy rigidbody");
                Vector3 movingDirection = transform.rigidbody.GetPointVelocity(transform.parent.position);
                rotateAngle = Vector3.Cross(movingDirection, transform.forward);
                Destroy(transform.rigidbody);
                transform.collider.enabled = true;
                transform.LookAt(transform.parent.transform.position);
            }

            transform.RotateAround(transform.parent.position, rotateAngle, Time.deltaTime * 10);

        }


    }

   
   
}
