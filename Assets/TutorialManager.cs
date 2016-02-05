using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour {

	public bool showTutorial;
	public int tipIndex = 0;
	public List<CanvasGroup> tips = new List<CanvasGroup>();

	// Use this for initialization
	void Start () {
		foreach(CanvasGroup canvas in tips){
			SetCanvasVisible(canvas, false);
		}

		if(showTutorial)
			ShowNextTip();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowNextTip(){
		SetCanvasVisible(tips[tipIndex], false);
		if(tipIndex < tips.Count-1){
			tipIndex++;
			SetCanvasVisible(tips[tipIndex], true);
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
}
