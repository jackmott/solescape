using UnityEngine;
using System.Collections;

public class NaturePreserve : Building
{

    protected float pollutionBuffer = 0f;
    protected float maxPollution = 1000f;
    float bufferAmount; 
    float clearAmount;

    public override string StatsText()
    {
        string costText = cost.ToString("0");              
        string stats = "     Cost: " + costText + "\n";
        stats += "     Pollution Removal: Significant until saturated";
        return stats;
    }
   
    // Use this for initialization
    protected override void Start()
    {

        base.Start();
        bufferAmount = pollution * .8f;
        clearAmount = pollution * .2f;

    }

   

    public override void AffectState()
    {
        if (!placed || !isEnabled)        
            return;
        
        float amt = state.AddPollution(clearAmount);        
        if (-amt < -clearAmount)
        {
            
            float remaining = -clearAmount - -amt;
            pollutionBuffer = Mathf.Max(0, pollutionBuffer - remaining);
        }
        float bufferLeft = maxPollution - pollutionBuffer;  //1000
        float toAddToBuffer = Mathf.Min(bufferLeft, -bufferAmount); // 20
        float pollutionRemoved = -state.AddPollution(-toAddToBuffer);			//0		
        pollutionBuffer += pollutionRemoved;

        if (pollutionBuffer == maxPollution)
        {
            pollution = clearAmount;
        }
        else
        {
            pollution = clearAmount + bufferAmount;
        }

        lastPollution = pollution;

    }

    protected override void OnGUI()
    {
        base.OnGUI();
        if (showHoverGui)
        {
            GUI.Label(new Rect(guiPos.x + 30, Screen.height - guiPos.y + yOffset, 100, 20), "Pollution Buffer:" + Mathf.RoundToInt(pollutionBuffer), style);
            yOffset += yOffsetAmount;
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        state.AddPollution(pollutionBuffer);
    }



}
