using UnityEngine;
using System.Collections;

public class RecyclingCenter : Building
{

    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = (energy * -1).ToString("0");
        string stats = "     Cost: " + costText + "\n";
        stats +=       "     Energy Consumption: " + energyText + "\n";
        stats +=       "     Pollution Removal: Moderate";
        return stats;
    }
}
