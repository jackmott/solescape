using UnityEngine;
using System.Collections;

public class ResearchContainer : MonoBehaviour {

    public Research r;
    public GameState state;

	

    public string ButtonText
    {
        get {
            if (r != null) return r.ButtonText(state.population, state.iq);
            else return "";
        }
        private set {}
    }

    public void Research()
    {        
        if (GameState.Instance.pendingResearch == null)
            GameState.Instance.BeginResearch(r);
    }
}
