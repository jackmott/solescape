using UnityEngine;
using System.Collections;

public class HelpButton : MonoBehaviour {

    public BuildingInfo bi;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void HelpClicked(dfControl control, dfMouseEventArgs args)
    {
        args.Use();
        TextAsset description = Resources.Load<TextAsset>("descriptions/" + bi.className);        
        GameObject prefab = (GameObject)Resources.Load("prefabs/Buildings/" + bi.className);
        Dialog.Instance.SetDialog(bi.buildingName, description.text, "Continue", true, false,prefab);
    }
}
