﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Plant : MonoBehaviour {

	public enum plantEnum {Tomato, Potato, Basil, Corn}
	public enum stageEnum {seedling, plant}

	public int seedNumber = 0;
	public int productNumber = 0;

	public plantEnum plantType = plantEnum.Tomato;
	public stageEnum plantStage = stageEnum.seedling;
	public Sprite plantIcon;
	public GameObject plantPrefab;
	public GameObject plantBtn;


	public void SetPlant(GameObject parcel, Plant plant){
		plantType = plant.plantType;
		plantPrefab = plant.plantPrefab;
		if(plantPrefab != null){
			GameObject newPlant = Instantiate(plantPrefab) as GameObject;
			newPlant.transform.SetParent(parcel.transform, false);
		}
	}

//	public void Water(){
//		Debug.Log("Watering plant " + plantType.ToString());
//		GardenManager.Instance.WaterThis(plantType.ToString());
//	}



}
