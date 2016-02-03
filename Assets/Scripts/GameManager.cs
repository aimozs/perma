using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public int gardenSize;
	public GameObject tile;

	public List<GameObject> garden = new List<GameObject>();

	public Plant[] instancesPlant;

	public GameObject camera;
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


	// Use this for initialization
	void Start () {
		Load();
		GenerateGarden();
		SetCamera(currentParcel);
		GetInstancesPlant();
	}

//	void OnEnable(){
//		OnClicked += GrowThat;
//	}
//
//	void OnDisable(){
//		OnClicked -= GrowThat;
//		Save();
//	}

	void SetCamera(int parcel){
		camera.GetComponent<UnityStandardAssets.Cameras.AutoCam>().SetTarget(garden[parcel].transform);
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

		_scroll = Input.GetAxis("Mouse ScrollWheel");
		if(_scroll > 0.01f) {
			SelectNextPlant(true);
		} else if(_scroll < -0.01f) {
			SelectNextPlant(false);
		}

		if(Input.GetKeyDown(KeyCode.A)){
			Plant[] instancesPlant = GameObject.FindObjectsOfType<Plant>();

				
			Debug.Log("There's a total of " + instancesPlant.Length + "plants");
		}
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
		SetCamera(currentParcel);

	}

	public void SelectNextPlant(bool next){
		if(next){
			UIManager.Instance.HighlightCurrentPlant(false);
			if(currentPlant < GardenManager.numberOfPlants-1)
				currentPlant++;
			UIManager.Instance.HighlightCurrentPlant(true);
		} else {
			UIManager.Instance.HighlightCurrentPlant(false);
			if(currentPlant > 0)
				currentPlant--;
			UIManager.Instance.HighlightCurrentPlant(true);
		}

	}

//	public void GrowThat(){
//		if(GardenManager.Instance.GetCurrentPlant(currentPlant) != null)
//			Grow(garden[currentParcel], GardenManager.Instance.GetCurrentPlant(currentPlant));
//	}

	public void WaterThat(){
		if(garden[currentParcel].GetComponentInChildren<Plant>() != null)
			GardenManager.Instance.WaterThis(garden[currentParcel].GetComponentInChildren<Plant>().plantType.ToString());

		GetInstancesPlant();

		Debug.Log((int)(instancesPlant.Length/3) + "sprouts vs garden size " + (gardenSize-2));

		if((int)(instancesPlant.Length/3) > (gardenSize-2)){
			currentLevel++;
			currentLevel = Mathf.Clamp(currentLevel, 0, GardenManager.numberOfPlants);

			if(GardenManager.Instance.GetCurrentPlant(currentLevel) != null)
				GardenManager.Instance.IncreaseSeedNumber(GardenManager.Instance.GetCurrentPlant(currentLevel).plantType.ToString(), true);

			gardenSize++;
			CreateParcel(gardenSize);
		}
	}

	public void Grow(GameObject parcel, Plant plant){
		if(GardenManager.Instance.debugGarden)
			Debug.Log("growing " + plant.plantType + "on parcel number " + parcel.name);
		
		parcel.AddComponent<Plant>();
		parcel.GetComponent<Plant>().SetPlant(parcel, plant);


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


	void Load(){}

	void Save(){}
}
