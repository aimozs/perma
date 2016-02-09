using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BtnClimate : MonoBehaviour {
  
	public Climate climate;
	public int timer;

	public void RefreshUI(){
		GetComponentInChildren<Text>().text = timer.ToString();
	}

	public void RefreshUI(string text){
		GetComponentInChildren<Text>().text = text;
	}

	public void SetClimateUI(){
		if(UIManager.Instance.debugUI && ClimateManager.Instance.debugClimate)
			Debug.Log("Setting climate for " + gameObject.name);
		transform.GetComponent<Image>().sprite = climate.climateIcon;
		timer = UnityEngine.Random.Range(3, 10);
		RefreshUI();
	}

}
