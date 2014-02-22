using UnityEngine;
using System.Collections;

public class MultiplayerComponentManager : MonoBehaviour {

    public MonoBehaviour[] behaviours;
    
    void Awake() { 
        if(networkView.isMine){
            foreach (MonoBehaviour m in behaviours)
            {
                m.enabled = false;
            }
        }
    }
}
