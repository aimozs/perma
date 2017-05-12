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
		transform.GetComponent<Image>().sprite = climate.climateIcon;
		timer = UnityEngine.Random.Range(15, 60);
		RefreshUI();
	}

}
