using UnityEngine;
using System.Collections;


public class SETITelescope : Building
{
    private float chance = 1f / 100f;

    public override string StatsText()
    {
        string costText = cost.ToString("0");
        string energyText = (energy * -1).ToString("0");
        string pollutionText = pollution.ToString("0");
        string stats = "     Cost: " + costText + "\n";
        stats += "     Energy Consumption: " + energyText + " units\n";
        stats += "     Polllution: " + pollutionText + "\n";
        return stats;
    }   

    protected override void Place(Vector3 position)
    {     
        base.Place(position);
        state.setiChance += chance;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (placed && isEnabled)
        {
            state.setiChance -= chance;
        }
    }

    protected override void Enable()
    {
        base.Enable();
        state.setiChance += chance;
    }

    protected override void Disable()
    {
        base.Disable();
        state.setiChance -= chance;
    }


}
