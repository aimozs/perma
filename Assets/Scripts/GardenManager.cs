using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GardenManager : MonoBehaviour {

	public bool debugGarden;
	public Dictionary<string, Plant> AllPlants = new Dictionary<string, Plant>();

	private static GardenManager instance;
	public static GardenManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GardenManager>();
			}
			return instance;
		}
	}

	public void InitPlants(){
		GetAllPlants();

		foreach(KeyValuePair<string, Plant> plant in AllPlants){
				UIManager.Instance.AddBtnPlant(plant.Value);
		}

	}

	// Use this for initialization
	public void GetAllPlants () {
		Plant[] plants = GetComponentsInChildren<Plant>();

		foreach(Plant plant in plants){
			if(debugGarden)
				Debug.Log("Adding to all plants " + plant.plantType.ToString());
			AllPlants.Add(plant.plantType.ToString(), plant);

		}

		if(debugGarden)
			Debug.Log("Found " + AllPlants.Count + " plants");
	}

//	public Plant GetCurrentPlant(int index){
//		if(debugGarden)
//			Debug.Log("index asked: " + index);
//		int i = 0;
//
//		foreach(KeyValuePair<string, Plant> plant in AllPlants){
//			if(i == index){
//				
//				return plant.Value;
//			} else{
//				i++;
//				if(debugGarden)
//					Debug.Log("Not equal " + i);
//			}
//		}
//		return null;
//	}

	public void IncreaseSeedNumber(string plantType, bool inc){
		if(debugGarden)
			Debug.Log("Increase plant seed number for " + plantType.ToString());
		
		Plant plant = AllPlants[plantType.ToString()];

		if(debugGarden)
			Debug.Log("on GO " + plant.name);
		
		if(inc)
			plant.seedNumber++;
		else
			plant.seedNumber--;
		plant.plantBtn.GetComponent<BtnPlant>().RefreshUI();
	}

	public Plant PlantFromString(string plantType){
		Plant plant = AllPlants[plantType.ToString()];

		if(debugGarden)
			Debug.Log("Returning " + plant.name);
		return plant;
	}

	public void ResetCycle(Parcel thisParcel, Plant plant){
		GameManager.Instance.ResetParcel(thisParcel);
		GameManager.Instance.ResetParcel(thisParcel);
		GameManager.Instance.currentParcelGO = thisParcel.gameObject;
		GameManager.Instance.GrowThatHere(plant);
	}

//	public void WaterThis(string plantType){
//
//			
//		if(debugGarden)
//			Debug.Log("Watering plant  " + plantType.ToString());
//		
//		Plant plant = AllPlants[plantType.ToString()];
//
//		if(debugGarden)
//			Debug.Log("on GO " + plant.name);
//
//
//		IncreaseSeedNumber(plantType.ToString(), true);
//		return;
//	}



}
