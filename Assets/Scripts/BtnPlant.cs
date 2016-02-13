using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class BtnPlant : MonoBehaviour {

	public Plant plant;

	void Start(){
//		RefreshUI();
		SetPrice();
	}

	public void PlantThat(){
		if(plant.seedNumber > 0){
			if(GameManager.Instance.currentParcelGO.GetComponent<Parcel>().ready){
				GameManager.Instance.GrowThatHere(plant);
				GardenManager.Instance.IncreaseSeedNumber(plant.plantType.ToString(), false);
				if(plant.seedNumber == 0)
					SetPrice();
			} else {
				UIManager.Notify("The parcel is not ready, use the shovel so you can plant something.");
			}
		} else {
			if(UIManager.Instance.currenPlant == plant)
				GameManager.Instance.BuyThat();
		}
	}

	public void RefreshUI(){
		if(UIManager.Instance.debugUI)
			Debug.Log("refreshing Btn");
		GetComponentInChildren<Text>().text = plant.seedNumber.ToString();
	}

	public void SetPlantUI(){
		if(UIManager.Instance.debugUI)
			Debug.Log("Adding sprite to button");
		transform.GetComponent<Image>().sprite = plant.plantIcon;
		RefreshUI();
	}



//	public void SetPlantShop(){
//		if(UIManager.Instance.debugUI)
//			Debug.Log("Adding sprite to button");
//		transform.GetComponent<Image>().sprite = plant.plantIcon;
//		SetPrice();
//	}

	void SetPrice(){
		GetComponentInChildren<Text>().text = plant.price.ToString() + "$";
	}

	public void SetPlantDetails(){
		UIManager.Instance.SetPlantDetails(plant);
	}
}
