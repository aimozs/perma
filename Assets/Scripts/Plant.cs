﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Plant : MonoBehaviour {

	public enum plantEnum {Tomato, Potato, Basil, Corn}
	public enum stageEnum {seedling, germination, growth, product}

	public int seedNumber = 0;
	public int productNumber = 0;

	public plantEnum plantType = plantEnum.Tomato;

	
	public Sprite plantIcon;
	public GameObject plantPrefab;
	public GameObject plantBtn;

	public GameObject growth;
	public GameObject product;
		
	public List<plantEnum> friends = new List<plantEnum>();
	public List<plantEnum> foes = new List<plantEnum>();
	



//	public void Water(){
//		Debug.Log("Watering plant " + plantType.ToString());
//		GardenManager.Instance.WaterThis(plantType.ToString());
//	}



}
