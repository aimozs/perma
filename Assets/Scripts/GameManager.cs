using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;
using System.Text;


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
		Application.targetFrameRate = 25;

	}
	// Use this for initialization
	void Start () {

		GenerateGarden();

		GetInstancesPlant();

		UIManager.Instance.SetCoinText(coins.ToString());
		SoundManager.Instance.SetVolume(0f);

		if(garden[currentParcel] != null)
			SetCamera(garden[currentParcel]);

		GPGSIds.ActivateGPGS();


	}



	//	void OnDisable(){
	//		if(reset == false)
	//			Save();
	//	}

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
			ResetParcel(parcel);
		}
	}

	public void ResetParcel(Parcel parcel){
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

	public void CreateParcel(int arrayPos){
		Vector3 position = new Vector3(arrayPos, 0, 0);
		GameObject parcel = Instantiate(tile, position, Quaternion.identity) as GameObject;
		parcel.name = parcel.name + arrayPos;
		garden.Add(parcel);
	}

	public void AddCoin(int change){
		coins = coins + change;
		UIManager.Instance.SetCoinText(coins.ToString());
//		GPGSIds.coins = coins;
	}

	public void BuyThat(){
		Plant plant = null;
		if(UIManager.Instance.currenPlant != null)
			plant = UIManager.Instance.currenPlant;

		if(plant != null){
			if(debugGame)
				Debug.Log("Buying plant " + plant.price + "$ with " + coins);
			if(plant.price <= coins){
				GardenManager.Instance.IncreaseSeedNumber(plant.plantType.ToString(), true);
				AddCoin(-plant.price);
			}
		}
	}

	public IEnumerator Save(){
		Debug.Log("gonna save");
		yield return new WaitForSeconds(60f);
		GPGSIds.Save();
		StartCoroutine(Save());
	}

//	public void ShowSavedGame(){
//		GPGSIds.ShowSelectUI();
//	}

}