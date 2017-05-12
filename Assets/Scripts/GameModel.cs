using UnityEngine;
using System.Collections;

public class GameModel : MonoBehaviour {

	public GameObject btnPlantPrefab;
	public GameObject btnClimatePrefab;

	public GameObject plantPrefab;
	public Mesh seedling;
	public GameObject germination;
	public GameObject parcelReady;
	public GameObject waste;

	public Sprite icon;

	public Color cold;
	public Color hot;

	public Color acid;
	public Color basic;

	public Color wet;
	public Color dry;

	public Color onHold;
	public Color growing;
	public Color gold;

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
