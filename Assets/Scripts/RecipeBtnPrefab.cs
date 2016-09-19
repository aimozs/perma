using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class RecipeBtnPrefab : MonoBehaviour {

	public Recipe recipe;

	// Use this for initialization
	void Start () {
		GetComponentInChildren<Text>().text = recipe.recipeName;
	}

//	void OnEnable(){
//		UIManager.OnRefreshRecipePanel += RefreshRecipeBtn;
//	}
//
//	void OnDisable(){
//		UIManager.OnRefreshRecipePanel -= RefreshRecipeBtn;
//	}
	


	public void DisplayRecipe(){
		UIManager.ShowRecipeDetails(recipe);
	}


}
