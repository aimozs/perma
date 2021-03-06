﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Well : MonoBehaviour {

	public Slider levelUI;
	public float level = 0.75f;
	public float incrementValue = .05f;


	private static Well instance;
	public static Well Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<Well>();
			}
			return instance;
		}
	}

	void Start(){

//		SetLevel(level);
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
			levelUI.value = levelUI.value + incrementValue;
			break;

		case Climate.ClimateType.storm:
			levelUI.value = levelUI.value + (incrementValue * 2);
			break;

		case Climate.ClimateType.sunny:

			switch(ClimateManager.Instance.previousWeather){
			case Climate.ClimateType.snowy:
				levelUI.value = levelUI.value + incrementValue;
				break;
			default:
				levelUI.value = levelUI.value - incrementValue;
				break;
			}
			break;

		default:
			break;
		}
	}
		

	public void UpdateLevel(bool up){
		if(up)
			levelUI.value = levelUI.value + incrementValue;
		else
			levelUI.value = levelUI.value - incrementValue;
	}

	void SetLevel(float level){
		if(levelUI == null)
			levelUI = GetComponentInChildren<Slider>();

//		if(level != null)
		levelUI.value = level;
	}

}
