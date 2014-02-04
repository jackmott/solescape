using UnityEngine;
using System.Collections;

public class OTree : Building
{


    public override string StatsText()
    {
       
        string costText = cost.ToString("0");
        string stats = "     Initial Energy Cost: " + costText + "\n";               
        stats +=       "     Pollution Cleanup: Minimal";        
        return stats;
    }
}
