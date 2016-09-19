using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KitchenManager : MonoBehaviour {

	public GameObject recipeBtnPrefab;
	public Transform saladPanel;
	public Transform soupPanel;
	public Transform mealPanel;
	public Transform dessertPanel;
	public Transform basicPanel;

	private List<Recipe> recipes = new List<Recipe>();


	// Use this for initialization
	void Start () {
		Recipe[] recipesArray = FindObjectsOfType<Recipe>();

		bool dirty = true;
		foreach(Recipe recipe in recipesArray){
			recipes.Add(recipe);
			PopulateKitchenPanel(recipe);

			if(dirty){
				UIManager.ShowRecipeDetails(recipe);
				dirty = false;
			}
		}

		soupPanel.gameObject.SetActive(false);
		mealPanel.gameObject.SetActive(false);
		dessertPanel.gameObject.SetActive(false);

//		UIManager.ShowRecipeDetails(saladPanel.GetChild(0).GetComponent<RecipeBtnPrefab>().recipe);
	}

	void PopulateKitchenPanel(Recipe recipe){
		GameObject newBtnPrefab = Instantiate(recipeBtnPrefab);

		switch(recipe.recipeType){
//		case Recipe.RecipeType.salad:
//			break;
		case Recipe.RecipeType.soup:
			newBtnPrefab.transform.SetParent(soupPanel, false);
			break;
		case Recipe.RecipeType.meal:
			newBtnPrefab.transform.SetParent(mealPanel, false);
			break;
		case Recipe.RecipeType.dessert:
			newBtnPrefab.transform.SetParent(dessertPanel, false);
			break;
		default:
			newBtnPrefab.transform.SetParent(saladPanel, false);

//			newBtnPrefab.transform.SetParent(basicPanel, false);
			break;
		}

		newBtnPrefab.GetComponent<RecipeBtnPrefab>().recipe = recipe;
		newBtnPrefab.GetComponentInChildren<Text>().text = recipe.recipeName;

	}

	public static void DoRecipe(Recipe recipe){
		foreach(Plant ingredient in recipe.ingredients){
			ingredient.productNumber--;
			ingredient.plantBtn.GetComponent<BtnPlant>().RefreshInventory();
		}

		recipe.quantity++;

		UIManager.ShowRecipeDetails();
//		UIManager.Instance.AdjustHunger(recipe.calories);
	}
}
