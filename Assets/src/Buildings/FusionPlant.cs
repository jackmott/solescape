using UnityEngine;
using System.Collections;

public class FusionPlant : Building
{


    public override string StatsText()
    {

        string energyText = (energy * 100f).ToString("0.00");
        string costText = cost.ToString("0");
        string stats = "     Initial Energy Cost: " + costText + "\n";
        stats +=       "     Energy Production: " + energyText + "% of planetary coal reserves\n";        
        return stats;
    }
}
