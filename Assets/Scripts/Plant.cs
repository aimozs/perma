using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Plant : MonoBehaviour {

	public enum plantEnum {Tomato, Potato, Basil, Corn, Pumpkin, Beetroot, Raspeberries, Rosemary, Onion, Mushroom, Banana}
	public enum plantTypeEnum {herb, vegie, fruit}
	public enum stageEnum {seedling, germination, pollination, product, growth}

	public plantEnum plantName = plantEnum.Tomato;
	public plantTypeEnum plantType = plantTypeEnum.herb;
	public ConditionTrigger.Condition condition;
	public Color color;

	public int seedNumber = 0;
	public int productNumber = 0;

	public int price = 0;
	public float calories = .1f;
	public float pHAve = 7f;
	public float germinationSize = 3;
	public float growthSize = 10; 
	public float pollinationSize = 15; // halfway between growth and product
	public float productSize = 20; //time to product / 3
	public float maxSize = 20; // product size + 10%
	public int tempMin = 10;
	public int tempMax = 25;

	public string description;
	public string source;



	
	public Sprite plantIcon;
//	public GameObject plantPrefab;
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
