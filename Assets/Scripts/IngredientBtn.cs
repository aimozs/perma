using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IngredientBtn : MonoBehaviour {

	public Image ingredientImage;
	public Text ingredientNumber;
	// Use this for initialization
//	void Start () {}
	
	// Update is called once per frame
//	void Update () {}

	public void SetPlant(Plant plant){
		ingredientImage.sprite = plant.plantIcon;
		ingredientNumber.text = plant.productNumber.ToString();
	}
}
