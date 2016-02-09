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
//	public Color colorHighlight;
//	public Color colorBasic;

	public float sensitivity = .01f;

	private float _horizontal;
	private bool _scroll;

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



		UIManager.Instance.SetCoinText(coins.ToString());

		if(garden[currentParcel]!= null)
			SetCamera(garden[currentParcel]);
	}

//	void OnEnable(){
//		OnClicked += GrowThat;
//	}
//
	void OnDisable(){
		Save();
	}

	void SetCamera(GameObject parcel){
		if(debugGame)
			Debug.Log(parcel.name);
		custCamera.GetComponent<UnityStandardAssets.Cameras.AutoCam>().SetTarget(parcel.transform);
	}

	// Update is called once per frame
	void FixedUpdate () {
		GetInput();
	}

	void GetInstancesPlant(){
		instancesPlant = GameObject.FindObjectsOfType<Plant>();
	}


	void GetInput(){
		_horizontal = Input.GetAxis("Horizontal");
		_horizontal = Mathf.Clamp(_horizontal, _horizontal - sensitivity, _horizontal + sensitivity);
		if(_horizontal > .2f && !_scroll){
			_scroll = true;
			SelectNextParcel(true);
		} else if(_horizontal < -.2f && !_scroll){
			_scroll = true;
			SelectNextParcel(false);
		}

		if(_horizontal < .1f && _horizontal > -.1f)
			_scroll = false;

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
		if(next){
			currentParcel++;
		} else {
			currentParcel--;
		}

		currentParcel = Mathf.Clamp(currentParcel, 0, garden.Count-1);
		SetCamera(garden[currentParcel]);
	}

	public void WaterThat(){
		if(Well.Instance.levelUI.value > .1f){
			Well.Instance.UpdateLevel(false);

			if(garden[currentParcel].GetComponent<Parcel>() != null){
				garden[currentParcel].GetComponent<Parcel>().ReceivesWater();
			}
		}
	}

	public void GetParcelReady(){
		Parcel parcel = garden[currentParcel].GetComponent<Parcel>();
		if(parcel != null){
			if(debugGame)
				Debug.Log(parcel.ready);
			if(!parcel.ready){
				parcel.ready = true;

//				if(parcel.GetComponentInChildren<Waste>() != null){
//					GameObject waste = parcel.GetComponentInChildren<Waste>().gameObject;
//					Destroy(waste);
//				}
				GameObject ready = (GameObject)Instantiate(GameModel.Instance.parcelReady, transform.position,  Quaternion.Euler(-90, 0, 0));
				ready.transform.SetParent(garden[currentParcel].transform, false);
			} else  {
				parcel.ready = false;
//				Debug.Log(parcel.GetComponentInChildren<PlantPrefab>().name);
				if(parcel.GetComponentInChildren<PlantPrefab>() != null){
					
					GameObject pp = parcel.GetComponentInChildren<PlantPrefab>().gameObject;
//					Debug.Log(pp.name);
					Destroy(pp);

				}
				GameObject waste = (GameObject)Instantiate(GameModel.Instance.waste, transform.position,  Quaternion.Euler(-90, 0, 0));
				waste.transform.SetParent(garden[currentParcel].transform, false);
			}

		}

	}

	public void GrowThatHere(Plant plant){
		garden[currentParcel].GetComponent<Parcel>().SetPlant(plant);
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

	public void AddParcel(){
		if(coins > 5){
			gardenSize++;
			CreateParcel(gardenSize);
		} else {
			UIManager.Notify("You don't have enough coins, harvest some products and sell them to get more coins!");
		}

	}

	void CreateParcel(int arrayPos){
		Vector3 position = new Vector3(arrayPos, 0, 0);
		GameObject parcel = Instantiate(tile, position, Quaternion.identity) as GameObject;
		parcel.name = parcel.name + arrayPos;
		garden.Add(parcel);
	}

	public void AddCoin(int change){
		coins = coins + change;
		UIManager.Instance.SetCoinText(coins.ToString());
	}


	void Load(){
		if(PlayerPrefs.GetInt("tuto") != null){
			if(PlayerPrefs.GetInt("tuto") == 1)
				TutorialManager.Instance.showTutorial = false;
		}
			
	}

	void Save(){
		if(TutorialManager.Instance.tipIndex > 1){
			PlayerPrefs.SetInt("tuto", 1);
		}

		PlayerPrefs.Save();
	}

	public void DeletePlayerPrefs(){
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("PlayerPrefs DELETED");
	}
}
