using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public enum SaveState {open, saved, loaded, error, savedOnline, loadOnline}


	[Header ("ShoppingShelf")]
	public GameObject seedBox;
	public GameObject herbsPanel;
	public GameObject vegiesPanel;
	public GameObject fruitsPanel;
	public Button confirmBtn;

	[Header ("Conditions")]
	public Slider indexTemp;
	public Slider indexpH;
	public Slider indexWater;

	[Header ("Player")]
	public GameObject coinBtn;
	public Slider healthSlider;

	[Header ("Climate")]
	public GameObject windBtn;
	public GameObject windDir;
	public GameObject climatePanel;

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
	public Image recipeImage;
	public Button createRecipeBtn;
	public Button visitLinkBtn;
	public Button consumeBtn;
	public Button sellBtn;
	public GameObject harvestedPlantPrefab;

	[Header ("Settings")]
	public GameObject currentState;
	public GameObject settingPanel;
	public Sprite SSopen;
	public Sprite SSsaved;
	public Sprite SSloaded;
	public Sprite SSonline;
	public Sprite SSerror;
	public Image signedInBtn;

	public GameObject seedMenu;

	private static Plant _currenPlant;

	private static Color _transparent = new Color(1f, 1f, 1f, 0f);
	private static Recipe _currentRecipe;
	const float _priceToCaloriesRatio = 50f;

	private CanvasGroup _menuCanvas = null;

	public delegate void RefreshRecipe();
	public static event RefreshRecipe OnRefreshRecipePanel;

//	public delegate void DisplayCondition(ConditionTrigger.Condition condition);
//	public static event DisplayCondition OnDisplayCondition;

	private static UIManager instance;
	public static UIManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<UIManager>();
			}
			return instance;
		}
	}

	public static Plant currentPlant {
		get { return _currenPlant; }
		set { 
			_currenPlant = value;
			if(Instance.confirmBtn != null){
				Instance.confirmBtn.interactable = _currenPlant != null ? true : false;
				RefreshConditions();
				if(_currenPlant == null)
					GardenCursor.DisplayPreviewPlant();
			}
		}
	}

	#region 0.Basics
		
//	void Start(){}

	#endregion

//	void Update(){}

//	void FixedUpdate(){}



	void OnEnable(){
		ClimateManager.OnTriggerClimate += AdjustHunger;
	}

	void OnDisable(){
		ClimateManager.OnTriggerClimate -= AdjustHunger;
	}

	public void Init(){
		_menuCanvas = seedMenu.GetComponent<CanvasGroup>();
		SetCoinText(GameManager.Instance.coins.ToString());
		DisplayMenu(false);
		kitchenPanel.SetActive(false);
		RefreshConditions();
	}

	#region 1.Statics

	public void SowCurrentPlant(){
		if(currentPlant != null){
			if(currentPlant.seedNumber > 0){
				GardenManager.GrowAtPosition(currentPlant, GardenCursor.cursorPosition);
				GardenManager.IncreaseSeedNumber(currentPlant, false);
				currentPlant = null;
			} else {
				GameManager.Instance.BuyPlantSeed(currentPlant);
			}
		}
	}

	public static bool isOverUI{
		get { return EventSystem.current.IsPointerOverGameObject(); }
	}
	#endregion

	#region Tools actions
	public void UseWateringCan(){
//		GardenManager.Instance.UseWateringCan();
	}

	public void UseSecator(){
//		GardenManager.Instance.UseSecator();
	}

//	public void UseShovel(){
//		ToggleSeedMenu();
////		GardenManager.Instance.UseShovel();
//	}

	public void SwitchKitchenGarden(){
		bool on = !kitchenPanel.activeInHierarchy;
		kitchenPanel.SetActive(on);
		DisplayScreen(seedMenu, on);

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

	public static void RefreshConditions(){

		Instance.indexWater.value = GardenManager.GetWaterIndex();
		Instance.indexWater.fillRect.GetComponent<Image>().color = GetConditionColor(ConditionTrigger.Condition.water, Instance.indexWater.value);

		Instance.indexTemp.value = GardenManager.GetTempIndex();
		Instance.indexTemp.fillRect.GetComponent<Image>().color = GetConditionColor(ConditionTrigger.Condition.temp, Instance.indexTemp.value);

		Instance.indexpH.value = GardenManager.GetpHIndex();
		Instance.indexpH.fillRect.GetComponent<Image>().color = GetConditionColor(ConditionTrigger.Condition.pH, Instance.indexpH.value);
	}

	static Color GetConditionColor(ConditionTrigger.Condition condi, float value){
		Color color = Color.white;
		switch (condi){
		case ConditionTrigger.Condition.temp:
			color = Color.Lerp(GameModel.Instance.cold, GameModel.Instance.hot, value);
			if(_currenPlant != null){
				if(_currenPlant.tempMin < value * 50f && value * 50f < _currenPlant.tempMax){
					color = Color.green;
				}
			}
			break;

		case ConditionTrigger.Condition.pH:
			color = Color.Lerp(GameModel.Instance.acid, GameModel.Instance.basic, value);
			if(_currenPlant != null){
//				Debug.Log(_currenPlant.pHAve + " vs " + value * 14f);
				if(_currenPlant.pHAve - 1 < value * 14f && value * 14f < _currenPlant.pHAve + 1){
					color = Color.green;
				}
			}
			break;

		case ConditionTrigger.Condition.water:
			color = Color.Lerp(GameModel.Instance.dry, GameModel.Instance.wet, value);
			if(_currenPlant != null){
//				Debug.Log(_currenPlant.pHAve + " vs " + value * 14f);
				switch(_currenPlant.waterNeeds){
				case Plant.waterEnum.generous:
					if(.75 < value)
						color = Color.green;
					break;
				case Plant.waterEnum.little:
					if(.5 < value && value < .75)
						color = Color.green;
					break;
				case Plant.waterEnum.moist:
					if(.25 < value && value < .5)
						color = Color.green;
					break;
				default:
					if(value < .25)
						color = Color.green;
					break;
				}
			}
			break;
		}

		return color;
	}

	public void ResetGarden(){
		Parcel[] parcels = GameObject.FindObjectsOfType<Parcel>();

		foreach(Parcel p in parcels){
			Destroy(p.gameObject);
		}

		SaveManager.DeleteSaveFile();
	}

	public void ToggleParticles(){
		ClimateManager.displayParticles = !ClimateManager.displayParticles;
	}


//	public void EnableTempVision(){
//		if(OnDisplayCondition != null)
//			OnDisplayCondition(ConditionTrigger.Condition.cool);
////		GardenCursor.SetWaterVision();
//	}
//
//	public void EnablepHVision(){
//		if(OnDisplayCondition != null){
//			OnDisplayCondition(ConditionTrigger.Condition.pH);
//		}
////		GardenCursor.SetpHVision();
//	}
//
//	public void EnableWaterVision(){
//		if(OnDisplayCondition != null)
//			OnDisplayCondition(ConditionTrigger.Condition.moist);
////		GardenCursor.SetTempVision();
//	}

	public void DisplayMenu(bool on){
		DisplayScreen(seedMenu, on);

		if(!on){
			currentPlant = null;
		}
	}

	public void ToggleSettings(){
		settingPanel.SetActive(!settingPanel.activeInHierarchy);
	}

	public void ToggleSeedMenu(){
		bool on = _menuCanvas.alpha == 0 ? true : false;
		if(_menuCanvas != null)
			ShowCanvas(_menuCanvas, on);

		if(!on)
			currentPlant = null;
	}

	public void SelectDisplayDetail(GameObject go){
		EventSystem.current.SetSelectedGameObject(go);
	}

	public static void DisplayHarvestedProduct(PlantPrefab pp){
//		Vector3 position = Camera.main.WorldToScreenPoint(pp.transform.position);
//		Debug.Log("displayed at " + position);
		GameObject _harvestedPlant = Instantiate(Instance.harvestedPlantPrefab);
		_harvestedPlant.GetComponent<HarvestedPlantPrefab>().SetDestination(Instance.healthSlider.transform.position);
		_harvestedPlant.transform.SetParent(Instance.notifPanel.transform, false);
		_harvestedPlant.GetComponentInChildren<Image>().sprite = pp.plant.plantIcon;
	}

	public static void DisplayHarvestedSeed(PlantPrefab pp){
//		Vector3 position = Camera.main.WorldToScreenPoint(pp.transform.position);
		//		Debug.Log("displayed at " + position);
		GameObject _harvestedPlant = Instantiate(Instance.harvestedPlantPrefab);
		_harvestedPlant.GetComponent<HarvestedPlantPrefab>().SetDestination(Instance.seedBox.transform.position);
		_harvestedPlant.transform.SetParent(Instance.notifPanel.transform, false);
		_harvestedPlant.GetComponentInChildren<Image>().sprite = pp.plant.plantIcon;
	}

	public void InitPanelsSize(){
		StartCoroutine(InitPanelSize(Instance.herbsPanel));
		StartCoroutine(InitPanelSize(Instance.vegiesPanel));
		StartCoroutine(InitPanelSize(Instance.fruitsPanel));

	}

	IEnumerator InitPanelSize(GameObject panel){

		yield return new WaitForSeconds(.3f);

		ScrollRect scrollRect = panel.GetComponent<ScrollRect>();

		if(scrollRect != null){
			int plantCount = scrollRect.content.childCount;

			RectTransform panelRect = scrollRect.content.GetComponent<RectTransform>();

			if(panelRect != null){
//				Vector2 anchors = panelRect.anchoredPosition;
//				Vector2 size = panelRect.sizeDelta;

				float newSize = plantCount * 150f;
//				Debug.Log(panelRect.name + panel.activeInHierarchy + " of size " + panelRect.sizeDelta.x + " will be changed to " + newSize + " / " + plantCount);
				Vector2 newSizeDelta = new Vector2(newSize, panelRect.sizeDelta.y);
				panelRect.sizeDelta = newSizeDelta;
//				Debug.Log(panelRect.sizeDelta.x);
				Vector2 newAnchors = new Vector2(newSize /2f, panelRect.anchoredPosition.y);
				panelRect.anchoredPosition = newAnchors;

			}
		}
		
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

	public static bool signedIn{
		set { Instance.signedInBtn.color = value ? Color.green : Color.red; }
	}

	public void Save(){
		SaveManager.SaveGame();
	}

	public void Load(){
		SaveManager.LoadGame();
	}

	public void ShowSavedGames(){
		#if (UNITY_ANDROID)
		SaveManager.ShowSelectUI();
		#endif
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

//	public void SetCurrentPlant(Plant plant){
////		tempText.text = plant.tempMin + "C - " + plant.tempMax + "C";
////		pHText.text = "pH " + plant.pHAve.ToString();
//		_currenPlant = plant;
//	}


	#region Climate UI
	public void AddClimate(Climate climate){
		GameObject btnClimate = Instantiate(GameModel.Instance.btnClimatePrefab);
		btnClimate.transform.SetParent(climatePanel.transform, false);
		btnClimate.name = btnClimate.name + climatePanel.transform.childCount.ToString();
		btnClimate.GetComponent<BtnClimate>().climate = climate;
		btnClimate.GetComponent<BtnClimate>().SetClimateUI();
	}

	public void SetWindStrength(int strength){
		windBtn.GetComponentInChildren<Text>().text = strength.ToString() + "Km/h";
	}

	public void SetWindDir(float direction){
		Debug.Log("icon set at orientation: " + direction);
		windDir.transform.rotation = Quaternion.Euler(0f, 0f, direction - 90f);
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


	public void DisplayScreen(GameObject screen, bool display){
		CanvasGroup canvas = screen.GetComponent<CanvasGroup>();
		if(canvas != null){
			ShowCanvas(canvas, display);
		}
	}

//	public void DisplayCanvas(CanvasGroup canvas, bool display){
//		ShowCanvas(canvas, display);
//	}

	void ShowCanvas(CanvasGroup canvas, bool on){
		canvas.alpha = on ? 1f : 0f;
		canvas.blocksRaycasts = canvas.interactable = on;
	}



	public void UpdateFriendColor(Slider slider, Plant plant, PlantPrefab.FriendStatus friendStatus){

		if(BtnTemperature.Instance.temperature >= plant.tempMin - 3 && BtnTemperature.Instance.temperature <= plant.tempMax + 3){
			switch (friendStatus){
			case PlantPrefab.FriendStatus.friend:
				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.hot;
				break;
			default:
				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.acid;
				break;
			}
		} else {
			slider.fillRect.GetComponent<Image>().color = GameModel.Instance.cold;
		}
	}

	#region Cooking related

	public static bool isCooking{
		get{ return Instance.kitchenPanel.activeInHierarchy; }
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
//			Instance.recipeDescription.text = _currentRecipe.linkRecipe;

			if(_currentRecipe.linkPhoto != null && _currentRecipe.linkPhoto != ""){
				Instance.StartCoroutine("FetchRecipeImage");
//				www.LoadImageIntoTexture(tex);
//				GetComponent<Renderer>().material.mainTexture = tex;
			} else {
				Instance.recipeImage.sprite = GameModel.Instance.icon;
			}
				
			Instance.visitLinkBtn.interactable = _currentRecipe.linkRecipe != "" ? true : false;

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

	IEnumerator FetchRecipeImage(){
		Texture2D tex;
		tex = new Texture2D(512, 512, TextureFormat.DXT1, false);
		WWW www = new WWW(_currentRecipe.linkPhoto);
		yield return www;
//		www.LoadImageIntoTexture(tex);
		Instance.recipeImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
	}



	static void RefreshIngredientsList(List<Plant> ingredients){
		for(int i = Instance.recipeIngredients.childCount; i > 0; i--){
			Destroy(Instance.recipeIngredients.GetChild(i-1).gameObject);
		}

		foreach(Plant ingredient in ingredients){
			GameObject ingredientBtn = Instantiate(Instance.ingredientBtnPrefab);
			ingredientBtn.GetComponent<IngredientBtn>().SetPlant(ingredient);
			ingredientBtn.transform.SetParent(Instance.recipeIngredients, false);
		}
		Instance.RefreshRecipeBtn();
	}

	public void DoRecipe(){
		KitchenManager.DoRecipe(_currentRecipe);
		RefreshRecipeBtn();
	}

	public void OpenLink(){
		if(_currentRecipe.linkRecipe != "")
			Application.OpenURL(_currentRecipe.linkRecipe);
	}



	#endregion
}
