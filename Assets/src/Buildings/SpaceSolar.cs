using UnityEngine;
using System.Collections;

public class SpaceSolar : Building
{


    public Vector3 rotateAngle;


    protected override void Awake()
    {
        base.Awake();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        placed = true;

    }

    public override void AffectState()
    {
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

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

   
}
