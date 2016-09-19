using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;
using System;
using System.Text;

[Serializable]
public class GameManager : MonoBehaviour {

	public bool debugGame;

	public int coins = 5;

	public GameObject cursor;
	public GameObject custCamera;
	public float sensitivity = .01f;

	private float _horizontal;
	private bool _scroll;

	private static GameManager instance;
	public static GameManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GameManager>();
			}
			return instance;
		}
	}

	#region 0.Basics
	void Awake(){
		Application.targetFrameRate = 25;
	}

	void Start () {
		GardenManager.Instance.InitPlants();

		UIManager.Instance.Init();
		SoundManager.SetVolume(0f);

		SaveManager.InitSave();
		SaveManager.SignIn();
	}

//	void OnEnable(){}

//	void OnDisable(){}

	void OnApplicationPause(bool paused){
		if(paused)
			SaveManager.SaveGame();
	}

	void OnApplicationQuit(){
		SaveManager.SaveGame();
	}

	#endregion

	#region 1.Statics

	public static Transform CursorTransform{
		get { return Instance.cursor.transform; }
	}

	public static void SetCamera(Vector3 target){
		Instance.cursor.transform.position = target;
	}

	public static void UpdateCameraPosition(Vector3 delta){
		Instance.cursor.transform.position += delta;
	}
	#endregion

	#region 2.Publics

	public void AddCoin(int change){
		coins = coins + change;
		UIManager.Instance.SetCoinText(coins.ToString());
	}

	public void BuyCurrentPlantSeed(Plant plant){
		if(plant.price <= coins){
			GardenManager.Instance.IncreaseSeedNumber(plant, true);
			AddCoin(-plant.price);
		}
	}

	public bool HaveEnoughCoin(int price){
		return (price > coins);
	}

	public IEnumerator Save(){
		Debug.Log("gonna save");
		yield return new WaitForSeconds(60f);
		//		GPGSIds.Save();
		StartCoroutine(Save());
	}

	public void SellCurrentPlantProduct(Plant plant){
		if(UIManager.Instance.currenPlant){
			if(plant.productNumber > 0){
//				Debug.Log("Selling " + plant.plantName);
				GardenManager.IncreaseProductNumber(plant, false);
				AddCoin(plant.price);
			}
		}
	}




	#endregion

	#region 3.Privates

	#endregion






}