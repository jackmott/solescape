using UnityEngine;
using System.Collections;

public class SolarArray : Building
{

    private GameObject sun;

    protected override void Awake()
    {
        base.Awake();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        sun = GameObject.Find("Sun");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void AffectState()
    {
        if (!placed || !isEnabled)
            return;

        //check if we are in sunlight

        RaycastHit hit = new RaycastHit();
        
        if (!Physics.Raycast(transform.position, sun.transform.position - transform.position, out hit))
        {
        
            float energyProduction = state.sunFactor * energy * (1f - (state.GetPollution() / state.pollutionDeathAmount));            
            lastEnergy = energyProduction;
            state.AddEnergy(energyProduction);
        }
        else
        {        
            lastEnergy = 0;
        }

    }

    protected override void OnGUI()
    {
        base.OnGUI();
        if (showHoverGui && !showGui)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            if (lastEnergy == 0)
            {
                GUI.Label(new Rect(pos.x + 30, Screen.height - pos.y + 10, 100, 20), "Night", style);
            }
            else
            {
                GUI.Label(new Rect(pos.x + 30, Screen.height - pos.y + 10, 100, 20), "Day", style);
            }
        }

    }

}
