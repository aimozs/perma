using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tool : MonoBehaviour {

	public Sprite toolIcon;
	public string toolDescription;
	public Sprite graphic;


	public void SetToolDescription(){
		UIManager.Instance.SetToolDescription(this);
	}
}
