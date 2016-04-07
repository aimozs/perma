using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour {

	public bool showTutorial = true;
	public int tipIndex = 0;
	public List<CanvasGroup> tips = new List<CanvasGroup>();


	private static TutorialManager instance;
	public static TutorialManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<TutorialManager>();
			}
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
		HideAllTutoPanels();

		StartTutorial();
	}

	public void HideAllTutoPanels(){
		foreach(CanvasGroup canvas in tips){
			SetCanvasVisible(canvas, false);
		}
	}

	void StartTutorial(){
		SetCanvasVisible(tips[tipIndex], true);
	}


	public void ShowNextTip(){
		SetCanvasVisible(tips[tipIndex], false);
		if(tipIndex < tips.Count-1 && showTutorial){
			if(tipIndex == 2)
				UIManager.Instance.DisplayMenu(true);
			
			tipIndex++;
			SetCanvasVisible(tips[tipIndex], true);
//			Debug.Log("Adding " + tips[tipIndex].GetComponentInChildren<Button>().gameObject.name + " from " + tips[tipIndex].name);
			EventSystem.current.SetSelectedGameObject(tips[tipIndex].GetComponentInChildren<Button>().gameObject);
		} else {
			FinishTutorial();
		}
	}

	void SetCanvasVisible(CanvasGroup canvas, bool visible){
		if(visible){
			canvas.alpha = 1f;
			canvas.interactable = canvas.blocksRaycasts = true;
		} else {
			canvas.alpha = 0f;
			canvas.interactable = canvas.blocksRaycasts = false;
		}
			
	}

	public void FinishTutorial(){
		HideAllTutoPanels();
		UIManager.Instance.DisplayMenu(false);
		EventSystem.current.SetSelectedGameObject(GameManager.Instance.garden[0]);
		showTutorial = false;
		StartCoroutine(GameManager.Instance.Save());
	}
}
