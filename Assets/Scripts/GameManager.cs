using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public bool debugGame;
	public int gardenSize;
	public GameObject tile;

	public int coins = 5;

	public List<GameObject> garden = new List<GameObject>();

	public Plant[] instancesPlant;

	public GameObject custCamera;
	public int currentParcel = 0;
	public int currentLevel = 0;
	public static int currentPlant = 0;
	public GameObject currentParcelGO;

	public float sensitivity = .01f;

	private bool reset = false;
	private float _horizontal;
	private bool _scroll;

	public delegate void Planting(Parcel parcel, Plant plant);
	public static event Planting PlantingThat;


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
		Application.targetFrameRate = 30;
		Load();
	}
	// Use this for initialization
	void Start () {
		
		GenerateGarden();

		GetInstancesPlant();

		UIManager.Instance.SetCoinText(coins.ToString());

		if(garden[currentParcel] != null)
			SetCamera(garden[currentParcel]);
	}
		
	void OnDisable(){
		if(reset == false)
			Save();
	}

	public void SetCamera(GameObject parcel){
		if(debugGame)
			Debug.Log(parcel.name);
		currentParcelGO = parcel;
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
		if(Input.GetButtonDown("Submit") && !TutorialManager.Instance.showTutorial && !UIManager.Instance.menu.GetComponent<CanvasGroup>().interactable)
			UIManager.Instance.DisplayMenu(true);
		if(Input.GetButtonDown("Cancel") && !TutorialManager.Instance.showTutorial && UIManager.Instance.menu.GetComponent<CanvasGroup>().interactable)
			UIManager.Instance.DisplayMenu(false);
	}

	public void WaterThat(){
		if(Well.Instance.levelUI.value > .1f){
			Well.Instance.UpdateLevel(false);

			if(currentParcelGO.GetComponent<Parcel>() != null){
				currentParcelGO.GetComponent<Parcel>().UpdateLevel(true);
			}
		}
	}

	public void UseSecator(){
		Parcel parcel = currentParcelGO.GetComponent<Parcel>();
		if(parcel != null){
			parcel.GetSeedOrProduct();
		} else {
			UIManager.Notify("No parcel");
		}
	}

	public void GetParcelReady(){
		Parcel parcel = currentParcelGO.GetComponent<Parcel>();
		if(parcel != null){
			if(debugGame)
				Debug.Log(parcel.ready);
			
			if(!parcel.ready){
				parcel.ready = true;

				if(parcel.GetComponentInChildren<Waste>() != null){
					GameObject waste = parcel.GetComponentInChildren<Waste>().gameObject;
					Destroy(waste);
				}

				GameObject ready = (GameObject)Instantiate(GameModel.Instance.parcelReady, transform.position,  Quaternion.Euler(-90, 0, 0));
				ready.transform.SetParent(parcel.transform, false);
			} else {
				parcel.ready = false;

				if(parcel.GetComponentInChildren<PlantPrefab>() != null){
					GameObject pp = parcel.GetComponentInChildren<PlantPrefab>().gameObject;
					Destroy(pp);
				}

				if(parcel.transform.FindChild("parcelReady(Clone)") != null){
					GameObject pp = parcel.transform.FindChild("parcelReady(Clone)").gameObject;
					Destroy(pp);
				}

				GameObject waste = (GameObject)Instantiate(GameModel.Instance.waste, transform.position,  Quaternion.Euler(-90, 0, 0));
				waste.transform.SetParent(parcel.transform, false);
			}
		}
	}

	public void GrowThatHere(Plant plant){
		currentParcelGO.GetComponent<Parcel>().SetPlant(plant);
		if(PlantingThat != null)
			PlantingThat(currentParcelGO.GetComponent<Parcel>(), plant);
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
		if(coins >= Parcel.price){
			gardenSize++;
			CreateParcel(gardenSize);
			AddCoin(-Parcel.price);
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

	public void BuyThat(){
		Plant plant = null;
		if(UIManager.Instance.currenPlant != null)
			plant = UIManager.Instance.currenPlant;
		
		if(plant != null){
			if(debugGame)
				Debug.Log("Buying plant " + plant.price + "$ with " + coins);
			if(plant.price <= coins){
				plant.seedNumber++;
				coins = coins - plant.price;
				UIManager.Instance.SetCoinText(coins.ToString());
				plant.plantBtn.GetComponent<BtnPlant>().RefreshUI();
			}
		}
	}


	void Load(){
//		if(PlayerPrefs.GetInt("tuto") != null){
//			if(PlayerPrefs.GetInt("tuto") == 1){
//				TutorialManager.Instance.showTutorial = false;
//			}
//		}

		if(PlayerPrefs.GetInt("coins") != null && PlayerPrefs.GetInt("coins") != 0){
			coins = PlayerPrefs.GetInt("coins");
			UIManager.Instance.SetCoinText(coins.ToString());
		}
	}

	void Save(){
		PlayerPrefs.SetInt("coins", coins);

		foreach(GameObject soil in garden){
			
		}

		PlayerPrefs.Save();
	}

	public void DeletePlayerPrefs(){
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("PlayerPrefs DELETED");
		reset = true;
	}
}
