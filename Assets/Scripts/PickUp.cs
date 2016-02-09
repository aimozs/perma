using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public Plant.plantEnum plantType = Plant.plantEnum.Tomato;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){
		GameManager.Instance.AddCoin(1);
		Destroy(gameObject);
	}
}
