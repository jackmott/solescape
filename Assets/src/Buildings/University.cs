using UnityEngine;
using System.Collections;

public class University : Building
{

    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = (energy*-1).ToString("0");
        string iqText = (iqFactor*100f).ToString("0")+"%";
        string pollutionText = pollution.ToString("0");
        string stats = "     Cost: " + costText + "\n";
        stats +=       "     Energy Consumption: " + energyText + " units\n";
        stats +=       "     Polllution: " + pollutionText + "\n";
        stats +=       "     IQ Factor: " + iqText;
        return stats;
    }
}