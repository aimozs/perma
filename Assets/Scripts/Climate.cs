using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Climate : MonoBehaviour {
  
	public enum ClimateType {sunny, rainy, storm, snowy, cloudy}
	public ClimateType climateType;
	public int tempMod = 0;
	public int tempTarget = 20;
	  
	public Sprite climateIcon;
	public GameObject climateBtn;
//	public float duration = 0f;


//	public int GetDuration(){
//		return (int)duration;
//	}
//  
//	void OnEnable(){
//		ClimateManager.OnTriggerClimate += TriggerClimate;
//	}
//	
//	void OnDisable(){
//		ClimateManager.OnTriggerClimate -= TriggerClimate;
//	}
//	
//	void TriggerClimate(Climate climate){
//	  StartCoroutine(EmitClimate());
//	}
//	
//	IEnumerator EmitClimate(){
//		yield return new WaitForSeconds(duration);
//		if(ClimateManager.Instance.debugClimate)
//			Debug.Log("Climate has finished");
//	}

}
