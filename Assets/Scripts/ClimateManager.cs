using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClimateManager : MonoBehaviour {

	public bool debugClimate = true;
	public float climateInterval = 10f;
	public int days_forecast = 3;
	public Climate.ClimateType previousWeather = Climate.ClimateType.sunny;
	public Dictionary<string, Climate> climatesDict = new Dictionary<string, Climate>();

	public List<Climate> forecast = new List<Climate>();
  
	public delegate void EmitClimate(Climate climate);
	public static event EmitClimate OnTriggerClimate;
  
	private static ClimateManager instance;
	public static ClimateManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<ClimateManager>();
			}
			return instance;
		}
	}
	
	void Start(){
		InitClimate();
	

		for(int f = 0; f < days_forecast; f++){
			AddClimateToForecast();

//			if(f == 0)
//				UIManager.Instance.SetDuration();
//			else
//				forecast[f].climateBtn.GetComponent<BtnClimate>().RefreshUI("?");
		}

		StartCoroutine(UpdateClimate());

		UIManager.Instance.StartTimerForecast();
	}

	void AddClimateToForecast(){
		Climate climate = GetRandomClimate();
		if(climate != null)
			forecast.Add(climate);
		UIManager.Instance.AddClimate(climate);
	}

	public void RenewCLimate(){
		Destroy(UIManager.Instance.climatePanel.transform.GetChild(0).gameObject);
		previousWeather = forecast[0].climateType;
		forecast.RemoveAt(0);
		AddClimateToForecast();
	}

	IEnumerator UpdateClimate(){

		if(OnTriggerClimate != null)
			OnTriggerClimate(forecast[0]);

		yield return new WaitForSeconds(climateInterval);
		StartCoroutine(UpdateClimate());
	}

	public Climate GetRandomClimate(){
		int rc = UnityEngine.Random.Range(0, climatesDict.Count);

		int i = 0;

		foreach(KeyValuePair<string, Climate> climate in climatesDict){
			if(i == rc){
				if(debugClimate)
					Debug.Log(i + " equal " + rc + " for : " + climate.Value.climateType.ToString());

				return climate.Value;
			} else{
				i++;
				if(debugClimate)
					Debug.Log("Not equal " + i);
			}
		}
		return climatesDict["sunny"];
	}

	
	public void InitClimate () {
		Climate[] climates = GameObject.FindObjectsOfType<Climate>();

		foreach(Climate climate in climates){
			climatesDict.Add(climate.climateType.ToString(), climate);
		}

		if(debugClimate)
			Debug.Log(climatesDict.Count);

	}
}
