using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public Plant.plantEnum plantType = Plant.plantEnum.Tomato;

	void OnMouseDown(){
		Harvest();
	}

	public void Harvest(){
		GameManager.Instance.AddCoin(1);
		Destroy(gameObject);
	}
}
