using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class ConditionTrigger : MonoBehaviour {

	public enum Condition { cool, dry, moist }
	public Condition condition;

	private Collider collider;
	// Use this for initialization
	void Start () {
		collider = GetComponent<Collider>();
		collider.isTrigger = true;
	}
	
	// Update is called once per frame
//	void Update () {}

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player"))
			GardenManager.currentCondition = condition;
	}

	void OnTriggerExit(Collider other){
		if(other.CompareTag("Player"))
			GardenManager.currentCondition = Condition.dry;
	}
}
