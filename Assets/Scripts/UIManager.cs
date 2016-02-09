using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public bool debugUI = true;
	public GameObject plantSelectPanel;
	public GameObject plantShopPanel;
	public GameObject coinBtn;
	public GameObject windBtn;
	public GameObject climatePanel;
	public GameObject infos;
	public GameObject notifPanel;
	public GameObject notifPrefab;

	public List<GameObject> btns = new List<GameObject>();

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
	}

	public static void Notify(string message){
		GameObject notif = Instantiate(Instance.notifPrefab);
		notif.GetComponentInChildren<Text>().text = message;
		notif.transform.SetParent(Instance.notifPanel.transform, false);
		Destroy(notif, 5f);
	}


	public void AddBtnPlant(Plant plant){
		GameObject btnPlant = Instantiate(GameModel.Instance.btnPlantPrefab);
		btnPlant.transform.SetParent(plantSelectPanel.transform, false);
		btnPlant.GetComponent<BtnPlant>().plant = plant;
		btnPlant.GetComponent<BtnPlant>().plant.plantBtn = btnPlant;
		btnPlant.GetComponent<BtnPlant>().SetPlantUI();
		btns.Add(btnPlant);

		if(debugUI)
			Debug.Log("Add a btn for " + plant.plantType.ToString());
	}

	public void AddBtnPlantToShop(Plant plant){
		GameObject btnPlant = Instantiate(GameModel.Instance.btnPlantShop);
		btnPlant.transform.SetParent(plantShopPanel.transform, false);
		btnPlant.GetComponent<BtnPlant>().plant = plant;
		btnPlant.GetComponent<BtnPlant>().plant.plantBtn = btnPlant;
		btnPlant.GetComponent<BtnPlant>().SetPlantShop();
		btns.Add(btnPlant);

		if(debugUI)
			Debug.Log("Add a btn for " + plant.plantType.ToString());
	}

	public void SetCoinText(string coin){
		coinBtn.GetComponentInChildren<Text>().text = coin + "$";
	}
	
	public void AddClimate(Climate climate){
		GameObject btnClimate = Instantiate(GameModel.Instance.btnClimatePrefab);
//		Debug.Log(btnClimate.name);
		btnClimate.transform.SetParent(climatePanel.transform, false);
		btnClimate.name = btnClimate.name + climatePanel.transform.childCount.ToString();
		btnClimate.GetComponent<BtnClimate>().climate = climate;
//		btnClimate.GetComponent<BtnClimate>().climate.climateBtn = btnClimate;
		btnClimate.GetComponent<BtnClimate>().SetClimateUI();
		//btns.Add(btnPlant);
	}

//	public void HighlightCurrentPlant(bool highlight){
//		if(highlight)
//			btns[GameManager.currentPlant].GetComponent<Image>().color = Color.green;
//		else
//			btns[GameManager.currentPlant].GetComponent<Image>().color = Color.white;
//	}

	public void DisplayScreenInfos(){
		CanvasGroup canvas = infos.GetComponent<CanvasGroup>();
		bool display = !canvas.interactable;
		if(debugUI)
			Debug.Log(display);
		if(canvas != null){
			if(display){
				canvas.alpha = 1f;
				canvas.blocksRaycasts = canvas.interactable = true;
			} else {
				canvas.alpha = 0f;
				canvas.blocksRaycasts = canvas.interactable = false;
				UnityAdsButton.Instance.DisplayAd();
			}
		}
	}

	public void DisplayScreen(GameObject screen, bool display){
		CanvasGroup canvas = screen.GetComponent<CanvasGroup>();
		if(canvas != null){
			if(display){
				canvas.alpha = 1f;
				canvas.blocksRaycasts = canvas.interactable = true;
			} else {
				canvas.alpha = 0f;
				canvas.blocksRaycasts = canvas.interactable = false;
			}
		}
	}

//	public void SetDuration(){
//		climatePanel.transform.GetChild(0).GetComponent<BtnClimate>().climate.duration = UnityEngine.Random.Range(10f, 60f);
//
//	}

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

	public void UpdateColor(Slider slider){
		if(BtnTemperature.Instance.temperature > 2){
			slider.fillRect.GetComponent<Image>().color = Color.cyan;
		} else {
			slider.fillRect.GetComponent<Image>().color = Color.white;
		}

		if(BtnTemperature.Instance.temperature > 10){
			slider.fillRect.GetComponent<Image>().color = Color.green;
		}
	}

	public void SetWindUI(int strength){
		windBtn.GetComponentInChildren<Text>().text = strength.ToString() + "Km/h";
	}

}
