using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public enum SaveState {open, saved, loaded, error, savedOnline, loadOnline}

	public bool debugUI = true;

	[Header ("ShoppingShelf")]
	public GameObject herbsPanel;
	public GameObject vegiesPanel;
	public GameObject fruitsPanel;

	[Header ("Details")]
	public Text tempText;
	public Text pHText;

	[Header ("Player")]
	public GameObject coinBtn;
	public Slider healthSlider;

	[Header ("Climate")]
	public GameObject windBtn;
	public GameObject climatePanel;
	public GameObject infos;

	[Header ("Notifs")]
	public GameObject notifPanel;
	public GameObject notifPrefab;

	[Header ("Kitchen")]
	public GameObject kitchenPanel;
	public Text recipeName;
	public Image recipeIcon;
	public Transform recipeIngredients;
	public GameObject ingredientBtnPrefab;
	public Transform recipeItemsList;
	public Text recipeDescription;
	public Button createRecipeBtn;
	public Button consumeBtn;
	public Button sellBtn;
	public GameObject harvestedPlantPrefab;

	[Header ("SaveState")]
	public GameObject splashScreen;
	public GameObject currentState;
	public Sprite SSopen;
	public Sprite SSsaved;
	public Sprite SSloaded;
	public Sprite SSonline;
	public Sprite SSerror;
	public Image signedInBtn;

	public GameObject cancelGO;
	public GameObject menu;

	public Plant currenPlant;

	private static Color _transparent = new Color(1f, 1f, 1f, 0f);
	private static Recipe _currentRecipe;
	const float _priceToCaloriesRatio = 50f;

	public delegate void RefreshRecipe();
	public static event RefreshRecipe OnRefreshRecipePanel;



	private static UIManager instance;
	public static UIManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<UIManager>();
			}
			return instance;
		}
	}

	void OnEnable(){
		ClimateManager.OnTriggerClimate += AdjustHunger;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= AdjustHunger;
	}



	public void Init(){
		SetCoinText(GameManager.Instance.coins.ToString());
		DisplayScreen(infos, false);
		DisplayMenu(false);
		kitchenPanel.SetActive(false);
	}

	#region Tools actions
	public void UseWateringCan(){
		GardenManager.Instance.UseWateringCan();
	}

	public void UseSecator(){
		GardenManager.Instance.UseSecator();
	}

	public void UseShovel(){
		GardenManager.Instance.UseShovel();
	}

	public void SwitchKitchenGarden(){
		bool on = !kitchenPanel.activeInHierarchy;
		kitchenPanel.SetActive(on);
		DisplayScreen(menu, on);

//		if(on)
//			RefreshRecipeBtn();
	}

	#endregion
		

	#region InGame UI
	public static void Notify(string message){
		GameObject notif = Instantiate(Instance.notifPrefab);
		notif.GetComponentInChildren<Text>().text = message;
		notif.transform.SetParent(Instance.notifPanel.transform, false);
		Destroy(notif, 5f);
	}

	public void DisplayMenu(bool on){
		DisplayScreen(menu, on);
	}

	public void SelectDisplayDetail(GameObject go){
		EventSystem.current.SetSelectedGameObject(go);
	}

	public static void DisplayHarvestedPlant(PlantPrefab pp){
		Vector3 position = Camera.main.WorldToScreenPoint(pp.transform.position);
//		Debug.Log("displayed at " + position);
		GameObject _harvestedPlant = Instantiate(Instance.harvestedPlantPrefab);
		_harvestedPlant.transform.SetParent(Instance.notifPanel.transform, false);
		_harvestedPlant.GetComponentInChildren<Image>().sprite = pp.plant.plantIcon;
	}

	public void DisplaySelectedPlantType(string type){
		herbsPanel.SetActive(false);
		vegiesPanel.SetActive(false);
		fruitsPanel.SetActive(false);

		switch(type){
		case "fruits":
			fruitsPanel.SetActive(true);
			break;
		case "vegies":
			vegiesPanel.SetActive(true);
			break;
		default:
			herbsPanel.SetActive(true);
			break;
		}
	}



	#endregion

	#region SaveStatus

	public static void HideSplashScreen(){
		Instance.splashScreen.SetActive(false);
	}

	public static void DisplaySaveState(SaveState state){
		Instance.ShowCanvas(Instance.currentState.GetComponent<CanvasGroup>(), true);
		Image currentStateImage = Instance.currentState.GetComponent<Image>();
		currentStateImage.color = Color.white;
		switch(state){
		case SaveState.open:
			currentStateImage.sprite = instance.SSopen;
			break;
		case SaveState.saved:
			currentStateImage.sprite = instance.SSsaved;
			break;
		case SaveState.loaded:
			currentStateImage.sprite = instance.SSloaded;
			break;
		case SaveState.savedOnline:
			currentStateImage.sprite = instance.SSonline;
			break;
		case SaveState.loadOnline:
			currentStateImage.sprite = instance.SSonline;
			currentStateImage.color = Color.green;
			break;
		}
	}

	public void ToggleSignIn(){
		if(SaveManager.signedIn)
			SaveManager.SignOut();
		else
			SaveManager.SignIn();
	}

	public static bool SignedIn{
		set { Instance.signedInBtn.color = value ? Color.green : Color.red; }
	}

	public void Save(){
		SaveManager.SaveGame();
	}

	public void Load(){
		SaveManager.LoadGame();
	}

	public void ShowSavedGames(){
		SaveManager.ShowSelectUI();
	}

	#endregion
		

	//Add a btn for the given plant to the shop menu
	public void AddBtnPlant(Plant plant){
		GameObject newBtn = (GameObject)Instantiate(GameModel.Instance.btnPlantPrefab);

		switch(plant.plantType){
		case Plant.plantTypeEnum.fruit:
			newBtn.transform.SetParent(fruitsPanel.GetComponent<ScrollRect>().content, false);
			break;
		case Plant.plantTypeEnum.vegie:
			newBtn.transform.SetParent(vegiesPanel.GetComponent<ScrollRect>().content, false);
			break;
		default:
			newBtn.transform.SetParent(herbsPanel.GetComponent<ScrollRect>().content, false);
			break;
		}


		BtnPlant newBtnPlant = newBtn.GetComponent<BtnPlant>();
		newBtnPlant.plant = plant;
		newBtnPlant.plant.plantBtn = newBtn;
		newBtnPlant.SetPlantUI();
	}
		


	public void SetCoinText(string coin = null){
		if(coin != null)
			coinBtn.GetComponentInChildren<Text>().text = coin;
		else 
			coinBtn.GetComponentInChildren<Text>().text = GameManager.Instance.coins.ToString();
	}

	public void SetPlantDetails(Plant plant){
		tempText.text = plant.tempMin + "C - " + plant.tempMax + "C";
		pHText.text = "pH " + plant.pHAve.ToString();
		currenPlant = plant;
	}


	#region Climate UI
	public void AddClimate(Climate climate){
		GameObject btnClimate = Instantiate(GameModel.Instance.btnClimatePrefab);
		btnClimate.transform.SetParent(climatePanel.transform, false);
		btnClimate.name = btnClimate.name + climatePanel.transform.childCount.ToString();
		btnClimate.GetComponent<BtnClimate>().climate = climate;
		btnClimate.GetComponent<BtnClimate>().SetClimateUI();
	}

	public void SetWindUI(int strength){
		windBtn.GetComponentInChildren<Text>().text = strength.ToString() + "Km/h";
	}

	public void StartTimerForecast(){
		InvokeRepeating("UpdateTimer", 0f, 1f);
	}

	void UpdateTimer(){
		BtnClimate currentClimate = climatePanel.transform.GetChild(0).GetComponent<BtnClimate>();
		if(currentClimate.timer > 0){
			currentClimate.timer--;
			currentClimate.RefreshUI();

		} else {
			ClimateManager.Instance.RenewCLimate();
		}
	}

	#endregion

	public void DisplayScreenInfos(){
		
		CanvasGroup canvas = infos.GetComponent<CanvasGroup>();
		ShowCanvas(canvas, !canvas.interactable);
	}

	public void DisplayScreen(GameObject screen, bool display){
		CanvasGroup canvas = screen.GetComponent<CanvasGroup>();
		if(canvas != null){
			ShowCanvas(canvas, display);
		}
	}

	void ShowCanvas(CanvasGroup canvas, bool on){
		canvas.alpha = on ? 1f : 0f;
		canvas.blocksRaycasts = canvas.interactable = on;
	}



	public void UpdateTempColor(Slider slider){
		if(BtnTemperature.Instance.temperature > 35){
			slider.fillRect.GetComponent<Image>().color = GameModel.Instance.tooWarm;
		} else {
			if(BtnTemperature.Instance.temperature > 2){
				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.onHold;
			} else {
				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.frozen;
			}
		}

	}

	public void UpdateFriendColor(Slider slider, Plant plant, PlantPrefab.FriendStatus friendStatus){
		if(debugUI)
			Debug.Log("Trying to change slider color for " + slider.name + "with plant " + plant.plantName.ToString() + " as friend " + friendStatus.ToString());

		if(BtnTemperature.Instance.temperature >= plant.tempMin - 3 && BtnTemperature.Instance.temperature <= plant.tempMax + 3){
			switch (friendStatus){
			case PlantPrefab.FriendStatus.friend:
				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.gold;
				break;
			default:
				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.growing;
				break;
			}
		} else {
			slider.fillRect.GetComponent<Image>().color = GameModel.Instance.onHold;
		}
	}

	#region Cooking related

	public static bool IsCooking{
		get{return Instance.kitchenPanel.activeInHierarchy;}
	}

	public static float hungerLevel{
		get { return Instance.healthSlider.value; }
		set { Instance.healthSlider.value = value; }
	}

	public void DisplaySelectedCategory(string type){
		for(int i = 0; i < recipeItemsList.childCount; i++){
			recipeItemsList.GetChild(i).gameObject.SetActive(false);
		}

		Transform selectedCategory = recipeItemsList.FindChild(type);
		selectedCategory.gameObject.SetActive(true);
		ShowRecipeDetails(selectedCategory.GetChild(0).GetComponent<RecipeBtnPrefab>().recipe);
	}

	void AdjustHunger(Climate climate){
		healthSlider.value += -.01f;
		if(healthSlider.value < .02f){
			healthSlider.value = 1f;
			UnityAdsButton.Instance.DisplayAd();
		}
	}

	public void AdjustHunger(){
		healthSlider.value += (_currentRecipe.price/_priceToCaloriesRatio);
		_currentRecipe.quantity--;
		ShowRecipeDetails();
	}

	public void SellMeal(){
		GameManager.Instance.AddCoin(_currentRecipe.price);
		_currentRecipe.quantity--;
		ShowRecipeDetails();
	}

	void RefreshRecipePanel(){
		if(OnRefreshRecipePanel != null)
			OnRefreshRecipePanel();
	}

	void RefreshRecipeBtn(){
		if(_currentRecipe != null){
			bool doable = true;
			foreach(Plant ingredient in _currentRecipe.ingredients){
				if(ingredient.productNumber <= 0){
					doable = false;
				}
			}
			createRecipeBtn.interactable = doable;
		}
	}

	public static void ShowRecipeDetails(Recipe recipe = null){
		if(recipe != null)
			_currentRecipe = recipe;
		
		if(_currentRecipe != null){

			Instance.recipeName.text = _currentRecipe.recipeName + "\n (" + _currentRecipe.quantity + ")";
			Instance.recipeDescription.text = _currentRecipe.description;

			Instance.consumeBtn.GetComponentInChildren<Text>().text = "Consume" + "\n+" + (_currentRecipe.price/_priceToCaloriesRatio).ToString("F2");
			Instance.sellBtn.GetComponentInChildren<Text>().text = "Sell" + "\n+" + _currentRecipe.price + "$";

			Instance.consumeBtn.interactable = _currentRecipe.quantity > 0 ? true : false;
			Instance.sellBtn.interactable = _currentRecipe.quantity > 0 ? true : false;

			if(_currentRecipe.icon == null){
				Instance.recipeIcon.color = _transparent;
			} else {
				Instance.recipeIcon.color = Color.white;
				Instance.recipeIcon.sprite = _currentRecipe.icon;
			}

			RefreshIngredientsList(_currentRecipe.ingredients);

		} else {
			Instance.consumeBtn.GetComponentInChildren<Text>().text = "Consume";
			Instance.sellBtn.GetComponentInChildren<Text>().text = "Sell";
		}
	}

	static void RefreshIngredientsList(List<Plant> ingredients){
		for(int i = Instance.recipeIngredients.childCount; i > 0; i--){
			Destroy(Instance.recipeIngredients.GetChild(i-1).gameObject);
		}

		foreach(Plant ingredient in ingredients){
			GameObject ingredientBtn = Instantiate(Instance.ingredientBtnPrefab);
			ingredientBtn.GetComponent<IngredientBtn>().SetPlant(ingredient);
			ingredientBtn.transform.SetParent(Instance.recipeIngredients);
		}
		Instance.RefreshRecipeBtn();
	}

	public void DoRecipe(){
		KitchenManager.DoRecipe(_currentRecipe);
		RefreshRecipeBtn();
	}



	#endregion
}
