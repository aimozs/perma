using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Parcel : MonoBehaviour {

	public float pH;
	public bool ready = false;
	public Text pHUI;
	public Slider waterUI;
	public float incrementValue = 0.1f;
	public static int price = 5;

	public GameObject plantPrefabGO;
	public GameObject parcelReadyGO;
	public GameObject waste;
	  
	void Start(){
		SetpH();
	}
  
	void SetpH(){
		pH = UnityEngine.Random.Range(5f, 9f);
		pHUI.text = "pH: " + pH.ToString("0.0");
		SetWaterLevel(0.1f);
  	}

	public void SetPlant(Plant plant){
		//plant the prefab
		plantPrefabGO = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		plantPrefabGO.GetComponent<PlantPrefab>().plant = plant;
		plantPrefabGO.transform.SetParent(transform, false);

		//clean any waste or parcelReady model if any
		parcelReadyGO = transform.FindChild("parcelReady(Clone)") != null ? transform.FindChild("parcelReady(Clone)").gameObject : null;
		waste = transform.FindChild("waste(Clone)") != null ? transform.FindChild("waste(Clone)").gameObject : null;

		if(waste != null)
			Destroy(waste);
		if(parcelReadyGO != null)
			Destroy(parcelReadyGO);
	}

	//
//	public void ReceivesWater(){
//		UpdateLevel(true);
//	}

	public void GetSeedOrProduct(){
		PlantPrefab pp = GetComponentInChildren<PlantPrefab>();
		if(pp != null){
			if(pp.plantStage == Plant.stageEnum.pollination && pp.pollinationPrefab != null){
				GardenManager.Instance.IncreaseSeedNumber(pp.plant.plantType.ToString(), true);
				UIManager.Notify("+1 seed for " + pp.plant.plantType.ToString());
				Destroy(pp.pollinationPrefab);
			} else if(GetComponentInChildren<PickUp>() != null){
				GetComponentInChildren<PickUp>().Harvest();
			}
		} else {
			UIManager.Notify("You try to look for results, but you havent started yet. Use the shovel and plant a seed first!");
		}
	}

	void OnEnable(){
		ClimateManager.OnTriggerClimate += ClimateConsequence;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= ClimateConsequence;
	}

	//called by the event at regular intervales
	public void ClimateConsequence(Climate climate){
		switch(climate.climateType){

		case Climate.ClimateType.rainy:
			waterUI.value = waterUI.value + incrementValue;
			break;

		case Climate.ClimateType.storm:
			waterUI.value = waterUI.value + (incrementValue * 2);
			break;

		case Climate.ClimateType.sunny:
			
			switch(ClimateManager.Instance.previousWeather){
			case Climate.ClimateType.snowy:
				waterUI.value = waterUI.value + incrementValue;
				break;
			default:
				if(GardenManager.Instance.debugGarden)
					Debug.Log("Reducing parcel's water level");
				waterUI.value = waterUI.value - incrementValue;
				break;
			}
			break;

		default:
			break;
		}

		if(GetComponentInChildren<PlantPrefab>() != null){
			PlantPrefab plantPrefab = GetComponentInChildren<PlantPrefab>();
			if(BtnTemperature.Instance.temperature >= plantPrefab.plant.tempMin && BtnTemperature.Instance.temperature <= plantPrefab.plant.tempMax){
				if(waterUI.value > 0.1f){
					if(plantPrefab.plant.pHAve > pH - 1f && plantPrefab.plant.pHAve < pH + 1f){
						plantPrefab.IncreaseSize(true);
					} else {
						UIManager.Notify(plantPrefab.plant.plantType + " will not grow on a pH not close to "+ plantPrefab.plant.pHAve.ToString()+ ", use the shovel to start fresh!");
					}
				} else {
					UIManager.Notify("Some parcelles are a bit too dry, think about watering your plants on sunny days!");
				}
			}
		}
		UIManager.Instance.UpdateColor(waterUI);
	}



	public void UpdateLevel(bool up){
//		if(GetComponentInChildren<PlantPrefab>() != null){
//			PlantPrefab plantPrefab = GetComponentInChildren<PlantPrefab>();
				
			if(up) {
				if(GardenManager.Instance.debugGarden)
					Debug.Log("Watering to " + waterUI.value.ToString("0.0")/* + " with a of " + plantPrefab.plant.pHAve*/);

				SetWaterLevel(waterUI.value + incrementValue);
				
//				if(plantPrefab.plant.pHAve > pH - 1f && plantPrefab.plant.pHAve < pH + 1f && waterUI.value > 0.6f){
//					if(GardenManager.Instance.debugGarden)
//						Debug.Log("Increasing size of " + plantPrefab.plant.plantType.ToString());
					
//					plantPrefab.IncreaseSize(true);
//					UpdateLevel(false);
				}

//			} else {
//				
//				waterUI.value = waterUI.value - incrementValue;
//					if(waterUI.value < 0.3f){
//						plantPrefab.IncreaseSize(false);
//					}
//			}
//		}

		UIManager.Instance.UpdateColor(waterUI);
			
	}

	void SetWaterLevel(float level){
		waterUI.value = level;
	}

	void OnMouseDown(){
		SetParcelSelected();
		UIManager.Instance.DisplayMenu(true);
	}

	public void SetParcelSelected(){
		GameManager.Instance.SetCamera(gameObject);
	}
}
