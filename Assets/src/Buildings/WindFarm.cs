using UnityEngine;
using System.Collections;

public class WindFarm : Windmill
{


    private float originalCost;
    private float originalEnergy;

    protected override void Awake()
    {
        base.Awake();
        originalCost = cost;
        originalEnergy = energy;
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

    public override void CheckColor(Color c)
    {
        if (c.g < c.b)
        {
            cost = 2f * originalCost;
            energy = 2f * originalEnergy;
        }
        else
        {
            cost = originalCost;
            energy = originalEnergy;
        }

    }

  
}
