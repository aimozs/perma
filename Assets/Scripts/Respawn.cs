using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour {


	// Use this for initialization
//	void Start () {}
	
	// Update is called once per frame
//	void Update () {}

	void OnTriggerExit(Collider other){
		if(other.CompareTag("Player")){
			other.transform.position = Vector3.one;
			UIManager.Notify("Stay focused on your garden ;)");
		}
	}
}
