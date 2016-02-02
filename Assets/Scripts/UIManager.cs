using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	public GameObject plantSelectPanel;

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


	public void AddBtnPlant(Plant plant){
		GameObject btnPlant = Instantiate(GameModel.Instance.btnPlantPrefab);
		btnPlant.transform.SetParent(plantSelectPanel.transform, false);
		btnPlant.GetComponent<BtnPlant>().plant = plant;
		btnPlant.GetComponent<BtnPlant>().plant.plantBtn = btnPlant;
		btnPlant.GetComponent<BtnPlant>().SetPlantUI();
		btns.Add(btnPlant);
		Debug.Log("Add a btn for " + plant.plantType.ToString());
	}

	public void HighlightCurrentPlant(bool highlight){
		if(highlight)
			btns[GameManager.currentPlant].GetComponent<Image>().color = Color.green;
		else
			btns[GameManager.currentPlant].GetComponent<Image>().color = Color.white;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
