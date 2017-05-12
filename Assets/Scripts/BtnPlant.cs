using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class BtnPlant : MonoBehaviour {

	public Plant plant;

	void Start(){
		RefreshInventory();
	}

	public void PlantThat(){
//		if(UIManager.IsCooking){
//			Debug.Log("selll");
//			GameManager.Instance.SellCurrentPlantProduct(plant);
//		} else {
//			if(GardenCursor.currentPlantTrigger.GetComponentInChildren<PlantPrefab>() == null){
		if(plant.seedNumber > 0){
			GardenManager.GrowAtPosition(plant, GardenCursor.cursorPosition);
			GardenManager.IncreaseSeedNumber(plant, false);

		} else {
			GameManager.Instance.BuyPlantSeed(plant);
		}
//			}
//		}
	}

	public void RefreshInventory(){
		if(UIManager.isCooking)
			GetComponentInChildren<Text>().text = plant.productNumber.ToString();
		else {
			if(plant.seedNumber <= 0){
				GetComponentInChildren<Text>().text = plant.price.ToString() + "$";
			} else {
				GetComponentInChildren<Text>().text = plant.seedNumber.ToString();
			}
		}
	}

	public void SetPlantUI(){
		transform.GetComponent<Image>().sprite = plant.plantIcon;
		RefreshInventory();
	}


	void SetPrice(){
		GetComponentInChildren<Text>().text = plant.price.ToString() + "$";
	}

	public void SelectDisplayDetail(){
		EventSystem.current.SetSelectedGameObject(this.gameObject);
		if(plant.seedNumber > 0){
			UIManager.currentPlant = plant;
			GardenCursor.DisplayPreviewPlant(plant);
		} else {
			GameManager.Instance.BuyPlantSeed(plant);
		}
	}
}
