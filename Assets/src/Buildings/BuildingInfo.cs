using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingInfo  {

    public float cost;
    public float energy;
    public float pollution;
    public int population;
    public float iqFactor;
    public string[] researchDependencies;
    public string buildingName;
    public string className;
    public int type;
    public string upgrade;
    public float upgradeCost;
    

    public BuildingInfo(string buildingName, string className,float cost, float energy, float pollution, int population, float iqFactor, int type, string upgrade, float upgradeCost,string[] researchDependencies)
    {
        this.buildingName = buildingName;
        this.className = className;        
        this.cost = cost;
        this.energy = energy;
        this.pollution = pollution;
        this.population = population;
        this.iqFactor = iqFactor;
        this.researchDependencies = researchDependencies;
        this.type = type;
        this.upgrade = upgrade;
        this.upgradeCost = upgradeCost;
    }

    public string ButtonText()
    {
        return buildingName + "\n"+cost;
    }

   
}
