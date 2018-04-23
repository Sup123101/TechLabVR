using UnityEngine;
using System.Collections;

public class GateTrigger : MonoBehaviour {
	public GateControl gate;
	
	void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            gate.Open();
        }
	}
	void OnTriggerExit(Collider other) {
       
            gate.Close();
        
        		
	}
	
}
