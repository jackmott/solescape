using UnityEngine;
using System.Collections;


public class SETITelescope : Building
{
    private float chance = 1f / 100f;

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

    protected override void Place(Vector3 position)
    {
        print("SETI PLACE");
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
