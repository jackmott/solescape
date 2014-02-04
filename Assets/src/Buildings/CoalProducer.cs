using UnityEngine;
using System.Collections;

public class CoalProducer : Building
{


  

    public override string StatsText()
    {
        
        string energyText = (energy*100f).ToString("0.00");
        string costText = cost.ToString("0");        
        string stats = "     Initial Energy Cost: "+costText+"\n";
        stats +=       "     Energy Production: " + energyText + "% of planetary coal reserves\n";
        string pollutionText = (pollution*100f).ToString("0");
        stats +=       "     Pollution: " + pollutionText + "% of energy production";
        return stats;        
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

