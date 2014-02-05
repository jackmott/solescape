using UnityEngine;
using System.Collections;

public class SpaceBuilding : Building {

	public LaunchFacility supplyFacility;
    public Vector3 rotateAngle;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        placed = true;

    }

    public override void AffectState()
    {

        if (supplyFacility == null)
        {
            print("Supply facilitiy destroyed!");
            Destroy(this.gameObject);
        }
        if (isEnabled && !supplyFacility.isEnabled)
        {
            Disable();
        }
        else if (!isEnabled && supplyFacility.isEnabled)
        {
            Enable();
        }
       

    }
}
