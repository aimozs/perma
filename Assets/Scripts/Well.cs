using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Well : MonoBehaviour {

	public Slider levelUI;
	public float level = 0.75f;
	public float incrementValue = .1f;


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

		SetLevel(level);
	}

	void OnEnable(){
		ClimateManager.OnTriggerClimate += UpdateWell;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= UpdateWell;
	}

	void UpdateWell(Climate climate){
		if(climate.climateType == Climate.ClimateType.sunny)
			UpdateLevel(false);

		if(climate.climateType == Climate.ClimateType.rainy)
			UpdateLevel(true);

	}

	public void UpdateLevel(bool up){
		if(up)
			levelUI.value = levelUI.value + incrementValue;
		else
			levelUI.value = levelUI.value - incrementValue;
	}

	void SetLevel(float level){
		levelUI.value = level;
	}

}
