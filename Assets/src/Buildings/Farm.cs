using UnityEngine;
using System.Collections;

public class Farm : Building
{

    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = (energy * -1).ToString("0");
        string popText = population.ToString("0");
        string pollutionText = pollution.ToString("0");
        string stats = "     Cost: " + costText + "\n";
        stats +=       "     Energy Consumption: " + energyText + " units\n";
        stats +=       "     Polllution: " + pollutionText + "\n";
        stats +=       "     Additional Population: " + popText;
        return stats;
    }




}
