using UnityEngine;
using System.Collections;

public class OilProducer : Building
{
    private float originalCost;        
    private int oilX, oilY;
    private int oilWidth, oilHeight;

    float energyPerPixel = 2.0f; //a value of 1 for the pixel = 500 energy
    static int wellSize = 33;
    static int factor = (wellSize - 1) / 2;



    protected override void Awake()
    {
        base.Awake();
        print("OIL PRODUCER!!!!");
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        originalCost = cost;
        oilWidth = state.oilTex.width;
        oilHeight = state.oilTex.height;              
    }


    protected override void Place(Vector3 position)
    {        
        base.Place(position);
       
        Ray ray = new Ray(transform.position, (Vector3.zero - transform.position).normalized);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        GameObject oilMap = GameObject.Find("Oil");
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.gameObject == oilMap)
                {
                    
                    oilX = Mathf.RoundToInt(hits[i].textureCoord.x * oilWidth);
                    oilY = Mathf.RoundToInt(hits[i].textureCoord.y * oilHeight);
                    break;
                }
            }
        }

        print("oil well texture coord:" + oilX + "," + oilY);
         
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void CheckColor(Color c)
    {
        if (c.g < c.b)
        {
            cost = 2f * originalCost;
            

        }
        else
        {
            cost = originalCost;            
        }

    }

    private float DistanceFromOrigin(int x, int y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    public override void AffectState()
    {
        
        if (!placed || !isEnabled)
            return;
        

        Color[] colors = state.oilColors;
     
        float totalEnergy = 0;
        int row, index, posX, posY;
        for (int y = -factor; y < factor; y++)
        {
            posY = (oilY + y) % oilHeight;
            if (posY < 0) posY = oilHeight + posY;
            row = posY * oilWidth;
            for (int x = -factor; x < factor; x++)
            {
                posX = (oilX + x) % oilWidth;
                if (posX < 0) posX = oilWidth + posX;
                index = row+posX;

                
                Color c = colors[index];
                                                      
                float energyFactor = (1-c.r) * energy * Mathf.Max(1f - DistanceFromOrigin(x,y) / factor,0);
                totalEnergy += energyPerPixel * energyFactor;
                float newColor = Mathf.Min(1,c.r + energyFactor);                              
                
                colors[index].r = newColor;
                
            }
        }
        
        lastEnergy = totalEnergy;
        state.AddEnergy(totalEnergy);
        state.AddPollution(pollution * totalEnergy);
        lastPollution = pollution * totalEnergy;
        
        
       

    }

}

