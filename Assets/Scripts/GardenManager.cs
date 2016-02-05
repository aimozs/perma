using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GardenManager : MonoBehaviour {

	public bool debugGarden;
	public Dictionary<string, Plant> AllPlants = new Dictionary<string, Plant>();
	public static int numberOfPlants;

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


//		AllPlants[Plant.plantEnum.Tomato.ToString()].seedNumber = 1;

		foreach(KeyValuePair<string, Plant> plant in AllPlants){
			UIManager.Instance.AddBtnPlant(plant.Value);
		}

		if(GetCurrentPlant(GameManager.Instance.currentLevel) != null)
			IncreaseSeedNumber(GetCurrentPlant(GameManager.Instance.currentLevel).plantType.ToString(), true);
	}

	// Use this for initialization
	public void GetAllPlants () {
		Plant[] plants = GameObject.FindObjectsOfType<Plant>();
		numberOfPlants = plants.Length;

		foreach(Plant plant in plants){
			if(debugGarden)
				Debug.Log("Adding to all plants " + plant.plantType.ToString());
			AllPlants.Add(plant.plantType.ToString(), plant);

			if(debugGarden)
				Debug.Log(AllPlants[plant.plantType.ToString()].plantIcon.name);
		}

		if(debugGarden)
			Debug.Log("Found " + AllPlants.Count + " plants");
	}

	public Plant GetCurrentPlant(int index){
		if(debugGarden)
			Debug.Log("index asked: " + index);
		int i = 0;

		foreach(KeyValuePair<string, Plant> plant in AllPlants){
			if(i == index){
				if(debugGarden)
					Debug.Log(i + " equal " + index + " for : " + plant.Value.plantPrefab.name);
				
				return plant.Value;
			} else{
				i++;
				if(debugGarden)
					Debug.Log("Not equal " + i);
			}
		}
		return null;
	}

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

	public void WaterThis(string plantType){

			
		if(debugGarden)
			Debug.Log("Watering plant  " + plantType.ToString());
		
		Plant plant = AllPlants[plantType.ToString()];

		if(debugGarden)
			Debug.Log("on GO " + plant.name);


		IncreaseSeedNumber(plantType.ToString(), true);
		return;
	}


}
