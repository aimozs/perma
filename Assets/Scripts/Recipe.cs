using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Recipe : MonoBehaviour {

	public enum RecipeType {salad, soup, meal, dessert}

	public string recipeName;
	public RecipeType recipeType;
	public Sprite icon;
	public string description;

	private float _calories = .1f;
	private int _price = 1;
	private int _quantity = 0;

	public List<Plant> ingredients = new List<Plant>();

	void Awake(){
		if(recipeName == null || recipeName == ""){
			recipeName = gameObject.name;
		}

		foreach(Plant ingredient in ingredients){
			_price += ingredient.price;
			_calories += ingredient.calories;
		}
	}

	public float calories{
		get { return _calories / 100f; }
	}

	public int price{
		get { return _price; }
	}

	public int quantity{
		get { return _quantity; }
		set { _quantity = value; }
	}
}
