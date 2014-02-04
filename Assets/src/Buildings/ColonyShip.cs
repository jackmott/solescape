using UnityEngine;
using System.Collections;

public class ColonyShip : Building
{




    public override string StatsText()
    {
        string costText = cost.ToString("0");
       
        string stats = "     Cost: " + costText + "\n";
       
        return stats;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();        
        placed = true;        

    }

    protected override void FixedUpdate()
    {
        float distance = (transform.position - state.planet.transform.position).magnitude;
        if (distance < 2000)
        {
            transform.rigidbody.AddForce(transform.forward * -100);


        }
        else
        {
            state.population = Mathf.Max(0, state.population - 100);
            state.colonyShipsLaunched++;
            Destroy(gameObject);
        }


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void AffectState()
    {
        //

    }
}
