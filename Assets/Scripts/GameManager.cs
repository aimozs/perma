using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public bool debugGame;
	public int gardenSize;
	public GameObject tile;

	public int coins = 3;

	public List<GameObject> garden = new List<GameObject>();

	public Plant[] instancesPlant;

	public GameObject custCamera;
	public int currentParcel = 0;
	public int currentLevel = 0;
	public static int currentPlant = 0;
	public Color colorHighlight;
	public Color colorBasic;

	public float sensitivity = .01f;

	private float _horizontal;
	private float _scroll;

//	public delegate void GrowThatPlant();
//	public static event GrowThatPlant OnClicked;

	private static GameManager instance;
	public static GameManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GameManager>();
			}
			return instance;
		}
	}

	void Awake(){
		Load();
	}
	// Use this for initialization
	void Start () {
		
		GenerateGarden();

		GetInstancesPlant();

		SetCamera(garden[currentParcel]);

		UIManager.Instance.SetCoinText(coins.ToString());
	}

//	void OnEnable(){
//		OnClicked += GrowThat;
//	}
//
	void OnDisable(){
		Save();
	}

	void SetCamera(GameObject parcel){
		custCamera.GetComponent<UnityStandardAssets.Cameras.AutoCam>().SetTarget(parcel.transform);
	}

	// Update is called once per frame
	void Update () {
		GetInput();
	}

	void GetInstancesPlant(){
		instancesPlant = GameObject.FindObjectsOfType<Plant>();
	}


	void GetInput(){
		_horizontal = Input.GetAxis("Horizontal");
		_horizontal = Mathf.Clamp(_horizontal, _horizontal - sensitivity, _horizontal + sensitivity);
		if(_horizontal > .1f){
			SelectNextParcel(true);
		} else if(_horizontal < -.1f){
			SelectNextParcel(false);
		}

//		_scroll = Input.GetAxis("Mouse ScrollWheel");
//		if(_scroll > 0.01f) {
//			SelectNextPlant(true);
//		} else if(_scroll < -0.01f) {
//			SelectNextPlant(false);
//		}

//		if(Input.GetKeyDown(KeyCode.A)){
//			Plant[] instancesPlant = GameObject.FindObjectsOfType<Plant>();

				
//			Debug.Log("There's a total of " + instancesPlant.Length + "plants");
//		}
	}


	public void SelectNextParcel(bool next){
		HighlightCurrentParcel(garden[currentParcel], false);

		if(next){
			currentParcel++;
		} else {
			currentParcel--;
		}

		currentParcel = Mathf.Clamp(currentParcel, 0, garden.Count-1);
		HighlightCurrentParcel(garden[currentParcel], true);
		SetCamera(garden[currentParcel]);

	}

//	public void SelectNextPlant(bool next){
//		if(next){
//			UIManager.Instance.HighlightCurrentPlant(false);
//			if(currentPlant < GardenManager.numberOfPlants-1)
//				currentPlant++;
//			UIManager.Instance.HighlightCurrentPlant(true);
//		} else {
//			UIManager.Instance.HighlightCurrentPlant(false);
//			if(currentPlant > 0)
//				currentPlant--;
//			UIManager.Instance.HighlightCurrentPlant(true);
//		}
//
//	}

//	public void GrowThat(){
//		if(GardenManager.Instance.GetCurrentPlant(currentPlant) != null)
//			Grow(garden[currentParcel], GardenManager.Instance.GetCurrentPlant(currentPlant));
//	}

	public void WaterThat(){
		if(Well.Instance.levelUI.value > .1f){
			Well.Instance.UpdateLevel(false);

			if(garden[currentParcel].GetComponent<Parcel>() != null){
				garden[currentParcel].GetComponent<Parcel>().ReceivesWater();
			}

//			if(garden[currentParcel].GetComponentInChildren<Plant>() != null)
//				GardenManager.Instance.WaterThis(garden[currentParcel].GetComponentInChildren<Plant>().plantType.ToString());

//			GetInstancesPlant();
//
//			if(debugGame)
//				Debug.Log((int)(instancesPlant.Length/3) + "sprouts vs garden size " + (gardenSize-2));
//
//			if((int)(instancesPlant.Length/3) > (gardenSize-2)){
//				currentLevel++;
//				currentLevel = Mathf.Clamp(currentLevel, 0, GardenManager.numberOfPlants);
//
				
//
//				gardenSize++;
//				CreateParcel(gardenSize);
//			}
		}

	}



	public void GetParcelReady(){
		garden[currentParcel].GetComponent<Parcel>().ready = true;
	}

//	public void Grow(GameObject parcel, Plant plant){
//		if(GardenManager.Instance.debugGarden)
//			Debug.Log("growing " + plant.plantType + "on parcel number " + parcel.name);
//		
//		parcel.AddComponent<Plant>();
//		parcel.GetComponent<Plant>().SetPlant(parcel, plant);
//
//
//	}

	public void GrowThatHere(Plant plant){
		garden[currentParcel].GetComponent<Parcel>().SetPlant(plant);
	}

	void HighlightCurrentParcel(GameObject parcel, bool highlight){
		if(highlight){
			SetColor(parcel, colorHighlight);
		} else {
			SetColor(parcel, colorBasic);
		}
	}

	void SetColor(GameObject parcel, Color color){
		parcel.GetComponentInChildren<MeshRenderer>().material.color = color;
	}

	void GenerateGarden(){
		for(int g = 1; g <= gardenSize; g++){
			CreateParcel(g);
		}

		GardenManager.Instance.InitPlants();
	}

	void CreateParcel(int arrayPos){
		Vector3 position = new Vector3(arrayPos, 0, 0);
		GameObject parcel = Instantiate(tile, position, Quaternion.identity) as GameObject;
		parcel.name = parcel.name + arrayPos;
		garden.Add(parcel);
	}


	void Load(){
//		if(PlayerPrefs.GetInt("tuto") != null){
//			if(PlayerPrefs.GetInt("tuto") == 1)
//				TutorialManager.Instance.showTutorial = false;
//		}
			
	}

	void Save(){
//		if(TutorialManager.Instance.tipIndex > 1){
//			PlayerPrefs.SetInt("tuto", 1);
//		}
//
//		PlayerPrefs.Save();
	}

	public void DeletePlayerPrefs(){
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("PlayerPrefs DELETED");
	}
}
