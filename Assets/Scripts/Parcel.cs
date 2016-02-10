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

	public GameObject plantPrefab;
	public GameObject parcelReady;
	public GameObject waste;
	  
	void Start(){
		SetpH();
	}
  
	void SetpH(){
		pH = UnityEngine.Random.Range(5f, 9f);
		pHUI.text = "pH: " + pH.ToString("0.0");
		waterUI.value = 0.1f;
  	}

	public void SetPlant(Plant plant){
		parcelReady = transform.FindChild("parcelReady(Clone)") != null ? transform.FindChild("parcelReady(Clone)").gameObject : null;
		waste = transform.FindChild("waste(Clone)") != null ? transform.FindChild("waste(Clone)").gameObject : null;
		plantPrefab = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		plantPrefab.GetComponent<PlantPrefab>().plant = plant;
		plantPrefab.transform.SetParent(transform, false);

		if(waste != null)
			Destroy(waste);
		if(parcelReady != null)
			Destroy(parcelReady);
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
			if(BtnTemperature.Instance.temperature > 10 && waterUI.value >= 0.1f){
				if(plantPrefab.plant.pHAve > pH - 1f && plantPrefab.plant.pHAve < pH + 1f){
					plantPrefab.IncreaseSize(true);
				} else {
					UIManager.Notify(plantPrefab.plant.plantType + " will not grow on a pH not close to "+ plantPrefab.plant.pHAve.ToString()+ ", use the shovel to start fresh!");
				}
			}
		}
		UIManager.Instance.UpdateColor(waterUI);
	}



	public void UpdateLevel(bool up){
		if(up) {
			waterUI.value = waterUI.value + incrementValue;
			if(GetComponentInChildren<PlantPrefab>() != null){
				PlantPrefab plantPrefab = GetComponentInChildren<PlantPrefab>();
//				Debug.Log("Watering to " + waterUI.value.ToString("0.0") + " with a of " + plantPrefab.plant.pHAve);
				if(plantPrefab.plant.pHAve > pH - 1f && plantPrefab.plant.pHAve < pH + 1f && waterUI.value > 0.6f){
//					Debug.Log("Increasing size of " + plantPrefab.plant.plantType.ToString());
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

		UIManager.Instance.UpdateColor(waterUI);
			
	}

	void SetLevel(float level){
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
