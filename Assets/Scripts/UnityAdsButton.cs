using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if (UNITY_ANDROID)
using UnityEngine.Advertisements;
#endif

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
		#if (UNITY_ANDROID)
		Advertisement.Show ();
		#endif
    }
}
