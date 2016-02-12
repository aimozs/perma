using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public Plant plant;
//	public Plant.plantEnum plantType = Plant.plantEnum.Tomato;

	void Start(){
		plant = GetComponentInParent<PlantPrefab>().plant;
	}

	void OnMouseDown(){
		Harvest();
	}

	public void Harvest(){
		GameManager.Instance.AddCoin(plant.price);
		UIManager.Notify("Sold " + plant.plantType.ToString() + " for " + plant.price + "$");
		Destroy(gameObject);

	}
}
