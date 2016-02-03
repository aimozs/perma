using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public bool debugUI = true;
	public GameObject plantSelectPanel;
	public GameObject infos;

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

	public void HighlightCurrentPlant(bool highlight){
		if(highlight)
			btns[GameManager.currentPlant].GetComponent<Image>().color = Color.green;
		else
			btns[GameManager.currentPlant].GetComponent<Image>().color = Color.white;
	}

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
}
