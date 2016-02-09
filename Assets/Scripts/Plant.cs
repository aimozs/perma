using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Plant : MonoBehaviour {

	public enum plantEnum {Tomato, Potato, Basil, Corn}
	public enum stageEnum {seedling, germination, pollination, product, growth}

	public int seedNumber = 0;
	public int productNumber = 0;
	public int price = 0;
	public float pHAve = 7f;
	public float germinationSize = 3;
	public float growthSize = 10;
	public float pollinationSize = 15;
	public float productSize = 20;

	public plantEnum plantType = plantEnum.Tomato;

	
	public Sprite plantIcon;
	public GameObject plantPrefab;
	public GameObject plantBtn;

	public GameObject growth;
	public GameObject pollination;
	public GameObject product;

		
	public List<plantEnum> friends = new List<plantEnum>();
	public List<plantEnum> foes = new List<plantEnum>();
	



//	public void Water(){
//		Debug.Log("Watering plant " + plantType.ToString());
//		GardenManager.Instance.WaterThis(plantType.ToString());
//	}



}
