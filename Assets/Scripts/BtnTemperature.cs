using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BtnTemperature : MonoBehaviour {

	public int temperature = 15;

	private static BtnTemperature instance;
	public static BtnTemperature Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<BtnTemperature>();
			}
			return instance;
		}
	}

	void OnEnable(){
		ClimateManager.OnTriggerClimate += UpdateTemperature;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= UpdateTemperature;
	}

	void UpdateTemperature(Climate climate){
		switch(climate.climateType){
		case Climate.ClimateType.sunny:
			temperature++;
			temperature++;
			break;
		case Climate.ClimateType.rainy:
			if(temperature > 10)
				temperature--;
			break;
		case Climate.ClimateType.storm:
			if(temperature > 8)
				temperature--;
			break;
		case Climate.ClimateType.cloudy:
				temperature++;
			break;
		case Climate.ClimateType.snowy:
			if(temperature > 1){
				temperature--;
				temperature--;
			}
			break;
		default:
			break;
		}
		GetComponentInChildren<Text>().text = temperature.ToString() + "˚C";
	}
}
