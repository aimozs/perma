using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	public PlantPrefab plantPrefab;
	public Plant plant;
//	public Plant.plantEnum plantType = Plant.plantEnum.Tomato;

	void Start(){
		plantPrefab = GetComponentInParent<PlantPrefab>();
		plant = plantPrefab.plant;
	}

	void OnMouseDown(){
		Harvest();
	}

	public void Harvest(){
		if(plantPrefab.plantStage == Plant.stageEnum.pollination && plantPrefab.pollinationPrefab != null){
			GardenManager.Instance.IncreaseSeedNumber(plant.plantType.ToString(), true);
			UIManager.Notify("+1 seed for " + plant.plantType.ToString());
			Destroy(plantPrefab.pollinationPrefab);
			plantPrefab.pollinationPrefab = null;
		} else if (plantPrefab.plantStage == Plant.stageEnum.product && plantPrefab.productPrefab != null) {
			GameManager.Instance.AddCoin(plant.price);
			UIManager.Notify("Sold " + plant.plantType.ToString() + " for " + plant.price + "$");
			Destroy(plantPrefab.productPrefab);
			plantPrefab.productPrefab = null;
		}
	}
}
