using UnityEngine;
using System.Collections;

public class CoalProducer : Building
{


    protected override void Awake()
    {
        base.Awake();
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


    public override void AffectState()
    {
        if (!placed || !isEnabled)
            return;
        
        
        float energyProduction = state.coalReserves * energy;
        lastEnergy = energyProduction;
        state.AddPollution(pollution*energyProduction);
        lastPollution = pollution * energyProduction;
        state.AddEnergy(energyProduction);
        state.coalReserves -= energyProduction;
    }

}

