using UnityEngine;
using System.Collections;

public class Windmill : Building
{

    bool randomizeEnergy = false;

    const float interfereDistance = 70f;
    const float stormDistance = 30f;
    const float stormMultiplier = 4f;

    protected override void Awake()
    {
        base.Awake();
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    

    public override void AffectState()
    {

        if (!placed ||!isEnabled)
            return;

        //static Collider[] OverlapSphere(Vector3 position, float radius, int layerMask = AllLayers);
        Collider[] colliders = Physics.OverlapSphere(transform.position, interfereDistance);

        

        float energyProduction = state.windFactor * energy;


        foreach (Collider c in colliders)
        {
            if (  (c.gameObject.GetComponent<WindFarm>() != null  || c.gameObject.GetComponent<Windmill>() != null)
                  && c.gameObject != gameObject)
            {
                Building b = c.gameObject.GetComponent<Building>();
                if (b.placed)
                {
                    float distance = Vector3.Distance(transform.position, b.transform.position);
                    float energySteal = Mathf.Clamp(Mathf.Pow(.5f, distance / 15f), 0f, .5f);
                    energyProduction *= (1-energySteal);


                }
            }           
        }

        for (int i = 0; i < state.planet.windZones.Length; i++)
        {
            Vector3 point = state.planet.windZones[i].transform.position;
            float distance = Vector3.Distance(transform.position,point);
            if (distance <= stormDistance)
            {
                energyProduction *= stormMultiplier;
            }

        }

        
        if (randomizeEnergy)
        {
            energyProduction = Random.Range(0, energyProduction * 2.0f);
        }
        lastEnergy = energyProduction;
        state.AddEnergy(energyProduction);
    }
}
