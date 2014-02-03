using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Research
{

    public string name;
    public float iqCost;
    public float energyCost;
    public string[] researchDependencies;
    public float completion = 0.0f;

    public bool paused = false;
    public bool insufficientEnergy = false;

    public string guiLabel;

    public Research(string name, float energyCost,float iqCost, string[] researchDependencies)
    {
        this.iqCost = iqCost;
        this.energyCost = energyCost;
        this.name = name;
        this.researchDependencies = researchDependencies;
        guiLabel = "Researching " + name;
    }

    public string ButtonText(int population, float iq)
    {        
        float seconds = iqCost / points(population, iq);
        return name + "\n" + seconds.ToString("0")+" sec";
    }


    public void incrementCompletion(int population, float iq)
    {
        completion += points(population, iq);
    }
    public float points(int population, float iq)
    {
        return (population * iq) / 100.0f; //points per second

    }

    public string ProgressText()
    {
        string progress = name + ": " + Mathf.RoundToInt(completion / iqCost * 100f) + "%";
        if (paused)
        {
            progress += "\nResearch On Hold";
        }
        else if (insufficientEnergy)
        {
            progress += "\nINSUFFICIENT ENERGY";
        }
        return progress;
    }

       
}
