using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class BtnPlant : MonoBehaviour {

	public Plant plant;

	void Start(){
		SetPlantShop();
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
			BuyThat();
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

	void BuyThat(){
		Debug.Log("Buying plant " + plant.price + "$ with " + GameManager.Instance.coins);
		if(plant.price <= GameManager.Instance.coins){
			plant.seedNumber++;
			GameManager.Instance.coins = GameManager.Instance.coins - plant.price;
			UIManager.Instance.SetCoinText(GameManager.Instance.coins.ToString());
			RefreshUI();
		}
	}

	public void SetPlantShop(){
		if(UIManager.Instance.debugUI)
			Debug.Log("Adding sprite to button");
		transform.GetComponent<Image>().sprite = plant.plantIcon;
		SetPrice();
	}

	void SetPrice(){
		GetComponentInChildren<Text>().text = plant.price.ToString() + "$";
	}

	public void SetPlantDetails(){
		UIManager.Instance.SetPlantDetails(plant);
	}
}
