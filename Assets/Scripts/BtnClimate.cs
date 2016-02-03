using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BtnClimate : MonoBehaviour {
  
  public Climate climate;
  
	public void RefreshUI(){
		GetComponentInChildren<Text>().text = climate.duration.ToString();
	}

	public void SetClimateUI(){
		transform.GetComponent<Image>().sprite = climate.climateIcon;
		RefreshUI();
	}
}
