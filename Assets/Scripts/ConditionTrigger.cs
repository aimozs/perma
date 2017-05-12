using UnityEngine;
using System.Collections;

public class ConditionTrigger : MonoBehaviour {

	public enum Condition { temp, dry, water, pH }
	public Condition condition;

	[Range(0,10)]
	public float val;

//	private Collider _col;
//	private MeshRenderer _meshRend;

	// Use this for initialization
//	void Start () {
//		_col = GetComponent<Collider>();
//		_col.isTrigger = true;
//		_meshRend = GetComponent<MeshRenderer>();
//		_meshRend.enabled = false;
//	}
//
//	void OnEnable(){
//		UIManager.OnDisplayCondition += DisplayCond;
//	}
//
//	void OnDisable(){
//		UIManager.OnDisplayCondition -= DisplayCond;
//	}
//	
//	void DisplayCond(Condition _condition){
//		if(_condition == condition)
//			_meshRend.enabled = !_meshRend.enabled;
//	}

	public float phVal {
		get { return val - 5f; }
	}

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player"))
			GardenManager.currentCondition = condition;
	}

	void OnTriggerExit(Collider other){
		if(other.CompareTag("Player"))
			GardenManager.currentCondition = Condition.dry;
	}
}
