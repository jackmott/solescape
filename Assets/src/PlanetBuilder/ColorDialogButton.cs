using UnityEngine;
using System.Collections;

public class ColorDialogButton : MonoBehaviour {

    public dfSlicedSprite colorToSet;
    public dfSlicedSprite sourceColor;
    public dfControl dialog;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClicked()
    {
        colorToSet.Color = sourceColor.Color;
        dialog.Hide();
        PlanetControlManager.Instance.RedrawPlanet();
    }
}
