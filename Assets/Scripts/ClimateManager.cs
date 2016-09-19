using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClimateManager : MonoBehaviour {

	public bool debugClimate = true;
	public float climateInterval = 10f;
	public int days_forecast = 3;
	public Climate.ClimateType previousWeather = Climate.ClimateType.sunny;
	public Dictionary<string, Climate> climatesDict = new Dictionary<string, Climate>();
	public Light sunLight;
	public Flare sunFlare;

	public List<Climate> forecast = new List<Climate>();

	public GameObject direction;
	public float dir = 0f;
	public int strength = 5;
	public GameObject windZone;

	public GameObject snowFlake;
	public GameObject cloud;
	public GameObject rain;
	public Light thunder;


  
	public delegate void EmitClimate(Climate climate);
	public static event EmitClimate OnTriggerClimate;
	private bool _initialized = false;

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
	}


	void DisableAllParticles(){
		cloud.GetComponent<ParticleSystem>().Stop();
		snowFlake.GetComponent<ParticleSystem>().Stop();
		rain.GetComponent<ParticleSystem>().Stop();
	}

	void AddClimateToForecast(Climate climate = null){
		
		if(climate == null)
			climate = GetRandomClimate();

		forecast.Add(climate);
		
		UIManager.Instance.AddClimate(climate);

//		Debug.Log(_initialized);

		if(_initialized)
			UpdateTemperature(forecast[0]);
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

	public void UpdateTemperature(Climate climate){
//		Debug.Log(climate.climateType.ToString());
		dir = UnityEngine.Random.Range(0f, 359f);
		direction.transform.rotation = Quaternion.Euler(0f, 0f, dir);
//		windZone.transform.rotation = Quaternion.Euler(0f, -dir, 0f);


		switch(climate.climateType){
		case Climate.ClimateType.storm:
			strength = UnityEngine.Random.Range(20, 50);
//			cloud.GetComponent<ParticleSystem>().Play();
//			rain.GetComponent<ParticleSystem>().Play();
//			snowFlake.GetComponent<ParticleSystem>().Stop();
			StartCoroutine(SetSun(false));
			SoundManager.PlayStorm();
			SoundManager.PlayRain();
			StartCoroutine(FlashThunder());
			break;
		case Climate.ClimateType.rainy:
			strength = UnityEngine.Random.Range(20, 50);
//			cloud.GetComponent<ParticleSystem>().Play();
//			rain.GetComponent<ParticleSystem>().Play();
//			snowFlake.GetComponent<ParticleSystem>().Stop();
			StartCoroutine(SetSun(false));
			SoundManager.PlayRain();
			break;
		case Climate.ClimateType.cloudy:
			strength = UnityEngine.Random.Range(5, 20);
//			cloud.GetComponent<ParticleSystem>().Play();
//			snowFlake.GetComponent<ParticleSystem>().Stop();
//			rain.GetComponent<ParticleSystem>().Stop();
			StartCoroutine(SetSun(true));
			SoundManager.PlayBirds();
			break;
		case Climate.ClimateType.snowy:
			strength = UnityEngine.Random.Range(5, 30);
//			snowFlake.GetComponent<ParticleSystem>().Play();
//			cloud.GetComponent<ParticleSystem>().Play();
//			rain.GetComponent<ParticleSystem>().Stop();
			StartCoroutine(SetSun(false));
			break;
		default:
			//DisableAllParticles();
			StartCoroutine(SetSun(true));
			strength = UnityEngine.Random.Range(1, 20);
			if(BtnTemperature.Instance.temperature > 22)
				SoundManager.PlayCicadas();
			else if(BtnTemperature.Instance.temperature > 10)
				SoundManager.PlayBirds();
			break;
		}
//		windZone.GetComponentInChildren<WindZone>().windMain = strength;
		StartCoroutine(SetWindStrength(strength * .9f));
		StartCoroutine(SoundManager.Instance.TransitionToVolume(strength /50f));

		UIManager.Instance.SetWindUI(strength);

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
//			windZone.GetComponentInChildren<WindZone>().windMain = Mathf.Lerp(windZone.GetComponentInChildren<WindZone>().windMain, newStrength, elapsedTime / 3f);
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator SetSun(bool on){
		float elapsedTime = 0;

		if(on){
			while (elapsedTime < 3f)
			{
				sunLight.intensity = Mathf.Lerp(sunLight.intensity, 1f, elapsedTime / 3f);
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		} else {
			while (elapsedTime < 3f)
			{
				sunLight.intensity = Mathf.Lerp(sunLight.intensity, .7f, elapsedTime / 3f);
				elapsedTime += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
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
