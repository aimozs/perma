using UnityEngine;
using System.Collections;

[RequireComponent(typeof (BoxCollider))]
public class PickUp : MonoBehaviour {

	public PlantPrefab plantPrefab;
	public Plant plant;

	void Start(){
		plantPrefab = GetComponentInParent<PlantPrefab>();
		plant = plantPrefab.plant;
	}

	public void Harvest(){
		if(plantPrefab.plantStage == Plant.stageEnum.pollination && plantPrefab.pollinationPrefab != null){
			GardenManager.Instance.IncreaseSeedNumber(plant, true);
//			UIManager.Notify("+1 seed for " + plant.plantName.ToString());
			Destroy(plantPrefab.pollinationPrefab);
			plantPrefab.pollinationPrefab = null;
		} else if (plantPrefab.plantStage == Plant.stageEnum.product && plantPrefab.productPrefab != null) {
//			Debug.Log("click received");
			GardenManager.HarvestProduct(plantPrefab);
//			GameManager.Instance.AddCoin(plant.price);
//			UIManager.Notify("Sold " + plant.plantName.ToString() + " for " + plant.price + "$");
//			Destroy(plantPrefab.productPrefab);
//			plantPrefab.productPrefab = null;
		}
	}
}
