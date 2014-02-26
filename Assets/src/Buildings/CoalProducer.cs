using UnityEngine;
using System.Collections;


public class CoalProducer : Building
{

    Color orange = Color.Lerp(Color.red, Color.yellow, .5f);
    Color baseColor;
    Color goalColor;
    Color color = Color.white;
    float colorTime = 2;
    float colorMult = 1;
    System.DateTime lastTime;

    protected override void Start()
    {        
        base.Start();
        lastTime = System.DateTime.Now;
    }
  

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


    protected override void Update()
    {        
        base.Update();

            
        if (colorTime >= 1)
        {
            colorTime = 0;
            baseColor = color;
            if (goalColor == orange) goalColor = Color.white;
            else goalColor = orange;                                    
            colorMult = Random.Range(1f,4.0f);
        }

        System.DateTime now = System.DateTime.Now;
        float timeDelta = (float)(now - lastTime).TotalSeconds;
        lastTime = now;
        colorTime += timeDelta * colorMult; ;
        color = Color.Lerp(baseColor, goalColor, Mathf.SmoothStep(0,1,colorTime));   
        
        this.renderer.material.SetColor("_IllumColor", color);
    }

}

