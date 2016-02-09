using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Advertisements;

public class UnityAdsButton : MonoBehaviour {

	private static UnityAdsButton instance;
	public static UnityAdsButton Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<UnityAdsButton>();
			}
			return instance;
		}
	}

	public void DisplayAd ()
    {
		Advertisement.Show ();
    }
}
