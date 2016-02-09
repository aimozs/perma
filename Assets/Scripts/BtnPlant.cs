using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BtnPlant : MonoBehaviour {

	public Plant plant;

	public void PlantThat(){
		if(plant.seedNumber > 0 && GameManager.Instance.garden[GameManager.Instance.currentParcel].GetComponent<Parcel>().ready){
			GameManager.Instance.GrowThatHere(plant);
//			GameManager.Instance.Grow(GameManager.Instance.garden[GameManager.Instance.currentParcel], plant);
			GardenManager.Instance.IncreaseSeedNumber(plant.plantType.ToString(), false);
		} else {
			UIManager.Notify("The parcel is not ready, use the shovel for that");
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

	public void BuyThat(){
		Debug.Log("Buying plant " + plant.price + "$ with " + GameManager.Instance.coins);
		if(plant.price <= GameManager.Instance.coins){
			plant.seedNumber++;
			UIManager.Instance.AddBtnPlant(plant);
			GameManager.Instance.coins = GameManager.Instance.coins - plant.price;
			UIManager.Instance.SetCoinText(GameManager.Instance.coins.ToString());
			Destroy(gameObject);
		}
			
	}

	public void SetPlantShop(){
		if(UIManager.Instance.debugUI)
			Debug.Log("Adding sprite to button");
		transform.GetComponent<Image>().sprite = plant.plantIcon;
		GetComponentInChildren<Text>().text = plant.price.ToString() + "$";

	}
}
