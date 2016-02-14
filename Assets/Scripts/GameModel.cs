using UnityEngine;
using System.Collections;

public class GameModel : MonoBehaviour {

	public GameObject btnPlantPrefab;
	public GameObject btnPlantShop;
	public GameObject btnClimatePrefab;

	public GameObject plantPrefab;
	public GameObject germination;
	public GameObject parcelReady;
	public GameObject waste;

	public Color frozen;
	public Color onHold;
	public Color growing;
	public Color tooWarm;
	public Color gold;
	public Color bronze;

	private static GameModel instance;
	public static GameModel Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GameModel>();
			}
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
