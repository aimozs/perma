﻿using UnityEngine;
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

		AllPlants[Plant.plantEnum.Tomato.ToString()].seedNumber = 1;

		foreach(KeyValuePair<string, Plant> plant in AllPlants){
			UIManager.Instance.AddBtnPlant(plant.Value);
		}

	}

	// Use this for initialization
	public void GetAllPlants () {
		Plant[] plants = GameObject.FindObjectsOfType<Plant>();
		numberOfPlants = plants.Length;

		foreach(Plant plant in plants){
			if(debugGarden)
				Debug.Log("Adding to all plants " + plant.plantType.ToString());
			AllPlants.Add(plant.plantType.ToString(), plant);
			Debug.Log(AllPlants[plant.plantType.ToString()].plantIcon.name);
		}

		if(debugGarden)
			Debug.Log("Found " + AllPlants.Count + " plants");
	}

	public Plant GetCurrentPlant(int index){
		if(debugGarden)
			Debug.Log("index asked: " + index);
		int i = 0;
		Plant currentPlant = new Plant();
		foreach(KeyValuePair<string, Plant> plant in AllPlants){
			if(i == index){
				Debug.Log(i + " equal " + index + " for : " + plant.Value.plantPrefab.name);
				currentPlant = plant.Value;
				break;
			} else{
				i++;
				if(debugGarden)
					Debug.Log("Not equal " + i);
			}
				
		}
		if(debugGarden)
			Debug.Log("returning " + currentPlant.plantType.ToString());
		return currentPlant;
	}

	public void IncreaseSeedNumber(string plantType, bool inc){
		Debug.Log("Increase plant seed number for " + plantType.ToString());
		Plant plant = AllPlants[plantType.ToString()];
		Debug.Log("on GO " + plant.name);
		if(inc)
			plant.seedNumber++;
		else
			plant.seedNumber--;
		plant.plantBtn.GetComponent<BtnPlant>().RefreshUI();
	}

	public void WaterThis(string plantType){
		Debug.Log("Watering plant  " + plantType.ToString());
		Plant plant = AllPlants[plantType.ToString()];
		Debug.Log("on GO " + plant.name);

		switch(plant.plantStage){
		case Plant.stageEnum.seedling:
			plant.plantStage = Plant.stageEnum.plant;
			break;
		default:
			break;
		}
		Debug.Log("it's now " + plant.plantStage.ToString());
		IncreaseSeedNumber(plantType.ToString(), true);
		return;
	}

}
