using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BtnWind : MonoBehaviour {

	public GameObject direction;
	public float dir = 0f;
	public int strength = 5;
	public GameObject windZone;

	public GameObject snowFlake;
	public GameObject cloud;
	public GameObject rain;

	void OnEnable(){
		ClimateManager.OnTriggerClimate += UpdateTemperature;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= UpdateTemperature;
	}

	void UpdateTemperature(Climate climate){
		dir = UnityEngine.Random.Range(0f, 359f);
		direction.transform.rotation = Quaternion.Euler(0f, 0f, dir);
		windZone.transform.rotation = Quaternion.Euler(0f, -dir, 0f);


		switch(climate.climateType){
		case Climate.ClimateType.storm:
			strength = UnityEngine.Random.Range(20, 50);
			cloud.GetComponent<ParticleSystem>().Play();
			rain.GetComponent<ParticleSystem>().Play();
			snowFlake.GetComponent<ParticleSystem>().Stop();
			break;
		case Climate.ClimateType.rainy:
			strength = UnityEngine.Random.Range(20, 50);
			cloud.GetComponent<ParticleSystem>().Play();
			rain.GetComponent<ParticleSystem>().Play();
			snowFlake.GetComponent<ParticleSystem>().Stop();
			break;
		case Climate.ClimateType.cloudy:
			strength = UnityEngine.Random.Range(5, 20);
			cloud.GetComponent<ParticleSystem>().Play();
			snowFlake.GetComponent<ParticleSystem>().Stop();
			rain.GetComponent<ParticleSystem>().Stop();
			break;
		case Climate.ClimateType.snowy:
			strength = UnityEngine.Random.Range(5, 30);
			snowFlake.GetComponent<ParticleSystem>().Play();
			cloud.GetComponent<ParticleSystem>().Play();
			rain.GetComponent<ParticleSystem>().Stop();
			break;
		default:
			DisableAllParticles();
			strength = UnityEngine.Random.Range(1, 20);
			break;
		}
		windZone.GetComponentInChildren<WindZone>().windMain = strength;
		UIManager.Instance.SetWindUI(strength);

	}

	void DisableAllParticles(){
		cloud.GetComponent<ParticleSystem>().Stop();
		snowFlake.GetComponent<ParticleSystem>().Stop();
		rain.GetComponent<ParticleSystem>().Stop();
	}
}
