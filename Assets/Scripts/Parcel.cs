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
	  
	void Start(){
		SetpH();
	}
  
	void SetpH(){
		pH = UnityEngine.Random.Range(5f, 9f);
		pHUI.text = "pH: " + pH.ToString("0.0");
		waterUI.value = 0.1f;
  	}

	public void SetPlant(Plant plant){
		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		newPlant.GetComponent<PlantPrefab>().plant = plant;
		newPlant.transform.SetParent(transform, false);
	}

	public void ReceivesWater(){
		if(GetComponentInChildren<PlantPrefab>() != null){
			
			GardenManager.Instance.IncreaseSeedNumber(GetComponentInChildren<PlantPrefab>().plant.plantType.ToString(), true);

		}
		UpdateLevel(true);
	}

	void OnEnable(){
		ClimateManager.OnTriggerClimate += UpdateLevel;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= UpdateLevel;
	}

	public void UpdateLevel(Climate climate){
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
				waterUI.value = waterUI.value - incrementValue;
				break;
			}
			break;

		default:
			break;
		}

		if(GetComponentInChildren<PlantPrefab>() != null){
			PlantPrefab plantPrefab = GetComponentInChildren<PlantPrefab>();
			if(plantPrefab.plant.pHAve > pH - 1f && plantPrefab.plant.pHAve < pH + 1f && waterUI.value >= 0.1f && BtnTemperature.Instance.temperature > 10){
				plantPrefab.IncreaseSize(true);
			}
		}
		UpdateColor();
	}

	void UpdateColor(){
		if(BtnTemperature.Instance.temperature > 2){
			waterUI.fillRect.GetComponent<Image>().color = Color.cyan;
		} else {
			waterUI.fillRect.GetComponent<Image>().color = Color.white;
		}

		if(BtnTemperature.Instance.temperature > 10){
			waterUI.fillRect.GetComponent<Image>().color = Color.green;
		}
	}

	public void UpdateLevel(bool up){
		if(up) {
			waterUI.value = waterUI.value + incrementValue;
			if(GetComponentInChildren<PlantPrefab>() != null){
				PlantPrefab plantPrefab = GetComponentInChildren<PlantPrefab>();
				Debug.Log("Watering to " + waterUI.value.ToString("0.0") + " with a of " + plantPrefab.plant.pHAve);
				if(plantPrefab.plant.pHAve > pH - 1f && plantPrefab.plant.pHAve < pH + 1f && waterUI.value > 0.6f){
					Debug.Log("Increasing size of " + plantPrefab.plant.plantType.ToString());
					plantPrefab.IncreaseSize(true);
				}
			}
		} else {
			waterUI.value = waterUI.value - incrementValue;
			if(GetComponentInChildren<PlantPrefab>() != null){
				PlantPrefab plantPrefab = GetComponentInChildren<PlantPrefab>();
				if(waterUI.value < 0.4f){
					plantPrefab.IncreaseSize(false);
				}
			}
		}

		UpdateColor();
			
	}

	void SetLevel(float level){
		waterUI.value = level;
	}
}
