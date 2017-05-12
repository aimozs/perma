using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClimateManager : MonoBehaviour {

	public bool debugClimate = true;
	public float climateInterval = 24f;
	public int days_forecast = 3;
	public Climate.ClimateType previousWeather = Climate.ClimateType.sunny;
	public Dictionary<string, Climate> climatesDict = new Dictionary<string, Climate>();
	public Light sunLight;
	public Flare sunFlare;

	public List<Climate> forecast = new List<Climate>();

	private static float _windDir = 0f;
	private int _windStr = 5;
	public WindZone windZone;


	public ParticleSystem cloud;
	public ParticleSystem rain;
	public ParticleSystem snowFlake;
	public Light thunder;

	private static bool _displayParticles = true;
  
	public delegate void EmitClimate(Climate climate);
	public static event EmitClimate OnTriggerClimate;
	private static bool _initialized = false;

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
		_initialized = true;

		InvokeRepeating("UpdateClimate", 2f, climateInterval);

		UIManager.Instance.StartTimerForecast();

		UpdateTemperature();
	}

	#region 1.Statics

	public static float currentTemp {
		get { return BtnTemperature.currentTemp; }
	}

	public static float windDirection{
		get { return _windDir; }
	}

	public static float windStrength{
		get { return Instance._windStr; }
	}

	public static bool displayParticles{
		get { 
//			Debug.Log(_displayParticles);
			return _displayParticles; }
		set {
			_displayParticles = value;
			Instance.UpdateTemperature();
		}
	}
	#endregion


	void DisableAllParticles(){
		cloud.Stop();
		snowFlake.Stop();
		rain.Stop();
	}

	void AddClimateToForecast(Climate climate = null){
		
		if(climate == null)
			climate = GetRandomClimate();

		if(forecast.Count > 0){
			while(climate == forecast[forecast.Count-1]){
				climate = GetRandomClimate();
			}
		}
		
		forecast.Add(climate);
		
		UIManager.Instance.AddClimate(climate);

//		Debug.Log(_initialized);

		if(_initialized)
			UpdateTemperature();


	}

	public void RenewCLimate(){
		Destroy(UIManager.Instance.climatePanel.transform.GetChild(0).gameObject);
		previousWeather = forecast[0].climateType;
		forecast.RemoveAt(0);

		AddClimateToForecast();

	}

	void UpdateClimate(){

		if(OnTriggerClimate != null)
			OnTriggerClimate(forecast[0]);

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

//		if(debugClimate)
//			Debug.Log(climatesDict.Count);

	}

	public void UpdateTemperature(){

//		direction.transform.rotation = Quaternion.Euler(0f, 0f, _windDir);
		DisableAllParticles();

		switch(forecast[0].climateType){
		case Climate.ClimateType.storm:
			_windStr = UnityEngine.Random.Range(20, 50);
			if(_displayParticles){
				cloud.Play();
				rain.Play();
				snowFlake.Stop();
			}
			StartCoroutine(SetSun(.4f));
			SoundManager.PlayStorm();
			SoundManager.PlayRain();
			StartCoroutine(FlashThunder());
			break;
		case Climate.ClimateType.rainy:
			_windStr = UnityEngine.Random.Range(10, 40);
			if(_displayParticles){
				cloud.Play();
				rain.Play();
				snowFlake.Stop();
			}
			StartCoroutine(SetSun(.6f));
			SoundManager.PlayRain();
			break;
		case Climate.ClimateType.cloudy:
			_windStr = UnityEngine.Random.Range(5, 30);
			if(_displayParticles){
				cloud.Play();
				snowFlake.Stop();
				rain.Stop();
			}
			StartCoroutine(SetSun(.8f));
			SoundManager.PlayBirds();
			break;
		case Climate.ClimateType.snowy:
			_windStr = UnityEngine.Random.Range(5, 40);
			if(_displayParticles){
				snowFlake.Play();
				cloud.Play();
				rain.Stop();
			}
			StartCoroutine(SetSun(.4f));
			break;
		default:
			DisableAllParticles();
			StartCoroutine(SetSun(.9f));
			_windStr = UnityEngine.Random.Range(1, 20);
			if(BtnTemperature.Instance.temperature > 22)
				SoundManager.PlayCicadas();
			else if(BtnTemperature.Instance.temperature > 10)
				SoundManager.PlayBirds();
			break;
		}

		//Set wind direction
		_windDir = UnityEngine.Random.Range(0f, 359f);
		Debug.Log("wind orientation " + _windDir);
		windZone.transform.rotation = Quaternion.Euler(0f, _windDir, 0f);
		UIManager.Instance.SetWindDir(_windDir);

		//Set wind strength
		StartCoroutine(SetWindStrength(_windStr / 50f));
		StartCoroutine(SoundManager.Instance.TransitionToVolume(_windStr /50f));
		UIManager.Instance.SetWindStrength(_windStr);

	}

	public IEnumerator FlashThunder(){

		thunder.intensity = 8f;
		yield return new WaitForSeconds(.1f);
		thunder.intensity = 0f;
		yield return new WaitForSeconds(.1f);
		thunder.intensity = 8f;
		yield return new WaitForSeconds(.1f);
		thunder.intensity = 0f;
		yield return new WaitForSeconds(.1f);

	}

	IEnumerator SetWindStrength(float newStrength){
//		Debug.Log(newStrength);
//		float windStrength = windZone.GetComponentInChildren<WindZone>().windMain;

		float elapsedTime = 0;

		while (elapsedTime < 3f) {
			windZone.windMain = Mathf.Lerp(windZone.windMain, newStrength, elapsedTime / 3f);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator SetSun(float newIntensity = .9f){
		float elapsedTime = 0;


		while (elapsedTime < 3f)
		{
			sunLight.intensity = Mathf.Lerp(sunLight.intensity, newIntensity, elapsedTime / 3f);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		

//		if(on)
//			do{
//				sunLight.intensity = Mathf.Lerp(0f, 1f, .1f);
//			} while (sunLight.intensity < 1f);
//		else
//			do{
//				sunLight.intensity = Mathf.Lerp(1f, 0f, .1f);
//			} while (sunLight.intensity > 0f);
//		
//		yield return new WaitForSeconds(2f);
		
	}
}
