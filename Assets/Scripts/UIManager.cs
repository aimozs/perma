using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public enum SaveState {open, saved, loaded, error}

	public bool debugUI = true;
	public GameObject shelf;
	public GameObject plantShopPanel;
	public GameObject toolPanel;
	public GameObject coinBtn;
	public GameObject windBtn;
	public GameObject climatePanel;
	public GameObject infos;
	public GameObject notifPanel;
	public GameObject notifPrefab;

	public Image fcImage;
	public Image toolsExplain;
	public Text fcDetails;
	public Text fcDescription;
	public Text fcSource;
	public Text fcBuy;

	[Header ("SaveState")]
	public GameObject currentState;
	public Sprite SSopen;
	public Sprite SSsaved;
	public Sprite SSloaded;
	public Sprite SSerror;

//	public GameObject wellGO;
//	public GameObject shovelGO;
//	public GameObject wateringGO;
	public GameObject cancelGO;

	public GameObject menu;

	public Light thunder;

	public Plant currenPlant;

	private bool _linkedToolsToSeed = false;

//	public List<GameObject> btns = new List<GameObject>();

	private static UIManager instance;
	public static UIManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<UIManager>();
			}
			return instance;
		}
	}

	void Start(){
		DisplayScreen(infos, false);
		DisplayMenu(false);
		SetPlantDetails(GardenManager.Instance.transform.GetChild(0).GetComponent<Plant>());
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.A)){
			EventSystem.current.SetSelectedGameObject(cancelGO);
			Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		}
	}

	public static void Notify(string message){
		GameObject notif = Instantiate(Instance.notifPrefab);
		notif.GetComponentInChildren<Text>().text = message;
		notif.transform.SetParent(Instance.notifPanel.transform, false);
		Destroy(notif, 5f);
	}

	public void DisplayMenu(bool on){
//		if(EventSystem.current.currentSelectedGameObject != null)
//			Debug.Log("1" + EventSystem.current.currentSelectedGameObject.name);
		DisplayScreen(menu, on);

		if(on){
			ActivateGardenSelectable(false);
//			if(EventSystem.current.currentSelectedGameObject != null)
//				Debug.Log("2" + EventSystem.current.currentSelectedGameObject.name);
//			EventSystem.current.SetSelectedGameObject(cancelGO);
//			if(EventSystem.current.currentSelectedGameObject != null)
//				Debug.Log("3" + EventSystem.current.currentSelectedGameObject.name);
		} else {
			ActivateGardenSelectable(true);
			EventSystem.current.SetSelectedGameObject(GameManager.Instance.currentParcelGO);
		}
//		StartCoroutine(SetButtonInteractable(on));
			
	}


	IEnumerator SetButtonInteractable(bool on){
		yield return new WaitForSeconds(.1f);
		cancelGO.GetComponent<Button>().interactable = on;
	}

	void ActivateGardenSelectable(bool on){
		foreach(GameObject parcel in GameManager.Instance.garden){
			if(parcel.GetComponent<Selectable>() != null)
				parcel.GetComponent<Selectable>().enabled = on;
		}
	}



	public static void DisplaySaveState(SaveState state){
		Instance.ShowCanvas(Instance.currentState.GetComponent<CanvasGroup>(), true);
		switch(state){
		case SaveState.open:
			Instance.currentState.GetComponent<Image>().sprite = instance.SSopen;
			break;
		case SaveState.saved:
			Instance.currentState.GetComponent<Image>().sprite = instance.SSsaved;
			break;
		case SaveState.loaded:
			Instance.currentState.GetComponent<Image>().sprite = instance.SSloaded;
			break;
		}

	}

	IEnumerator HideSaveState(){
		yield return new WaitForSeconds(3f);
		Instance.ShowCanvas(Instance.currentState.GetComponent<CanvasGroup>(), false);
	}
		

	public void AddBtnPlant(Plant plant/*, Vector3 position*/){
		GameObject btnPlant = (GameObject)Instantiate(GameModel.Instance.btnPlantPrefab/*, position, Quaternion.identity*/);
		btnPlant.transform.SetParent(shelf.transform, false);
		btnPlant.GetComponent<BtnPlant>().plant = plant;
		btnPlant.GetComponent<BtnPlant>().plant.plantBtn = btnPlant;
		btnPlant.GetComponent<BtnPlant>().SetPlantUI();

		if(debugUI)
			Debug.Log("linkToSeed " + _linkedToolsToSeed + " will link to " + btnPlant.name);
		
//		if(!_linkedToolsToSeed)
//			LinkToolsToSeeds(btnPlant);

		if(debugUI)
			Debug.Log("Add a btn for " + plant.plantType.ToString());
	}

//	void LinkToolsToSeeds(GameObject firstSeed){
//		if(debugUI)
//			Debug.Log("Linking tools " + toolPanel.transform.GetChild(toolPanel.transform.childCount-1).name + "to seed " + firstSeed.name);
//
//		_linkedToolsToSeed = true;
//
//		Navigation custNav = new Navigation();
//		custNav.mode = Navigation.Mode.Explicit;
//		custNav.selectOnUp = wellGO.GetComponent<Selectable>();
//		custNav.selectOnLeft = (Selectable)wateringGO.GetComponent<Button>();
//		custNav.selectOnRight = (Selectable)firstSeed.GetComponent<Button>();
//		
//		shovelGO.GetComponent<Button>().navigation = custNav;
//	}

	public void SetCoinText(string coin){
		if(debugUI)
			Debug.Log("Updating coin UI");
		coinBtn.GetComponentInChildren<Text>().text = coin;
	}
	
	public void AddClimate(Climate climate){
		GameObject btnClimate = Instantiate(GameModel.Instance.btnClimatePrefab);
		btnClimate.transform.SetParent(climatePanel.transform, false);
		btnClimate.name = btnClimate.name + climatePanel.transform.childCount.ToString();
		btnClimate.GetComponent<BtnClimate>().climate = climate;
		btnClimate.GetComponent<BtnClimate>().SetClimateUI();
	}

	public void DisplayScreenInfos(){
		
		CanvasGroup canvas = infos.GetComponent<CanvasGroup>();
		ShowCanvas(canvas, !canvas.interactable);
//		bool display = !canvas.interactable;
//		if(debugUI)
//			Debug.Log(display);
//		if(canvas != null){
//			if(display){
//				ShowCanvas(canvas, true);
//			} else {
//				ShowCanvas(canvas, false);
//				canvas.alpha = 0f;
//				canvas.blocksRaycasts = canvas.interactable = false;
//
//			}
//		}
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

	public void StartTimerForecast(){
		StartCoroutine(UpdateTimer());
	}

	IEnumerator UpdateTimer(){
		BtnClimate currentClimate = climatePanel.transform.GetChild(0).GetComponent<BtnClimate>();
		if(currentClimate.timer > 0f){
			currentClimate.timer--;
			currentClimate.RefreshUI();

		} else {
			ClimateManager.Instance.RenewCLimate();

		}
		yield return new WaitForSeconds(1f);
		StartCoroutine(UpdateTimer());
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
			Debug.Log("Trying to change slider color for " + slider.name + "with plant " + plant.plantType.ToString() + " as friend " + friendStatus.ToString());

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
			
//		if(friendStatus == PlantPrefab.FriendStatus.none){
//			if(BtnTemperature.Instance.temperature >= plant.tempMin - 3 && BtnTemperature.Instance.temperature <= plant.tempMax + 3)
//				slider.fillRect.GetComponent<Image>().color = GameModel.Instance.growing;
//			else{
//				UpdateTempColor(slider);
//			}
//		} else {
//			if(BtnTemperature.Instance.temperature >= plant.tempMin - 3 && BtnTemperature.Instance.temperature <= plant.tempMax + 3){
//				if(friendStatus == PlantPrefab.FriendStatus.friend){
//					slider.fillRect.GetComponent<Image>().color = GameModel.Instance.gold;
//				}
//
////				if(friendStatus == PlantPrefab.FriendStatus.foe){
////					slider.fillRect.GetComponent<Image>().color = GameModel.Instance.bronze;
////				}
//			} else {
//				UpdateTempColor(slider);
//			}
//
//		}
	}

	public void SetWindUI(int strength){
		windBtn.GetComponentInChildren<Text>().text = strength.ToString() + "Km/h";
	}

	public IEnumerator FlashThunder(){

		thunder.intensity = 8f;
		yield return new WaitForSeconds(.1f);
		thunder.intensity = 0f;
		yield return new WaitForSeconds(.1f);
		thunder.intensity = 8f;
		yield return new WaitForSeconds(.1f);
		thunder.intensity = 0f;
		yield return new WaitForSeconds(.1f);

	}

	public void SetPlantDetails(Plant plant){
		fcBuy.transform.parent.gameObject.SetActive(true);
		toolsExplain.color = new Color(1f,1f,1f,0f);
		fcImage.sprite = plant.plantIcon;
		fcDetails.text = "Ideal growth temperature range:\n" + plant.tempMin + "˚C - " + plant.tempMax + "˚C\n\npH average:\n" + plant.pHAve + "+/-1";
		fcDescription.text = plant.description;
		fcSource.text = plant.source;
		fcBuy.text = "Buy (" + plant.price + "$)";
		currenPlant = plant;
	}

	public void SetToolDescription(Tool tool){
		fcBuy.transform.parent.gameObject.SetActive(false);
		toolsExplain.sprite = tool.graphic;
		toolsExplain.color = new Color(1f,1f,1f,1f);
		fcImage.sprite = tool.toolIcon;
		fcDetails.text = "";
		fcDescription.text = tool.toolDescription;
		fcSource.text = "N/A";
		currenPlant = null;
	}
}
