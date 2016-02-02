using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BtnPlant : MonoBehaviour {

	public Plant plant;

	public void PlantThat(){
		if(plant.seedNumber > 0){
			GameManager.Instance.Grow(GameManager.Instance.garden[GameManager.Instance.currentParcel], plant);
			GardenManager.Instance.IncreaseSeedNumber(plant.plantType.ToString(), false);
		}


	}

	public void RefreshUI(){
		Debug.Log("refreshing UI");
		GetComponentInChildren<Text>().text = plant.seedNumber.ToString();
	}

	public void SetPlantUI(){
		Debug.Log("Adding sprite to button");
		transform.GetComponent<Image>().sprite = plant.plantIcon;
		RefreshUI();
	}
}
