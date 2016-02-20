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
	public bool saveLocally = true;
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

	//SAVING DATA
	private DateTime gameStart;
	private ISavedGameMetadata gameMetaData;



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

	}
	// Use this for initialization
	void Start () {

		ActivateGPGS();

		gameStart = DateTime.Now;
		
		GenerateGarden();

		GetInstancesPlant();

		UIManager.Instance.SetCoinText(coins.ToString());

		if(garden[currentParcel] != null)
			SetCamera(garden[currentParcel]);

		if(saveLocally)
			LoadLocally();
	}

	void ActivateGPGS(){
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
			.EnableSavedGames()
			// registers a callback to handle game invitations received while the game is not running.
//			.WithInvitationDelegate(<callback method>)
			// registers a callback for turn based match notifications received while the
			// game is not running.
//			.WithMatchDelegate(<callback method>)
			// require access to a player's Google+ social graph to sign in
			.RequireGooglePlus()
			.Build();

		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
		SignIn();

	}

	void SignIn(){
			// authenticate user:
			Social.localUser.Authenticate((bool success) => {
				if(success){
					OpenSavedGame("garden");
				} else {
					UIManager.Notify("Could not sign in. The game will be saved locally.");
					saveLocally = true;
				}
			});

	}

//	void ShowSelectUI() {
//		uint maxNumToDisplay = 5;
//		bool allowCreateNew = false;
//		bool allowDelete = true;
//
//		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
//		savedGameClient.ShowSelectSavedGameUI("Select saved game",
//			maxNumToDisplay,
//			allowCreateNew,
//			allowDelete,
//			OnSavedGameSelected);
//	}
//
//
//	public void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata game) {
//		if (status == SelectUIStatus.SavedGameSelected) {
//			// handle selected game save
////			gameMetaData = game;
////			OpenSavedGame("garden");
//		} else {
//			// handle cancel or error
//		}
//	}

	void OpenSavedGame(string filename) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
	}

	public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			gameMetaData = game;
			UIManager.Notify("Save was retreived properly");

			LoadGameData(gameMetaData);
		} else {
			UIManager.Notify("Save couldn't be opened");
		}
	}

	void LoadGameData (ISavedGameMetadata game) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
	}

	public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		if (status == SavedGameRequestStatus.Success) {
			// handle processing the byte array data
			LoadData(ByteToString(data));
		} else {
			// handle error
			UIManager.Notify("Could not read saved data");
		}
	}

	public void SaveGameToCloud(){
		string inventory = "$" + coins.ToString();
		foreach(GameObject parcelGO in garden){
			inventory += "@" + parcelGO.name.Substring(parcelGO.name.Length-1, 1);
			inventory += "ph:" + parcelGO.GetComponent<Parcel>().pH.ToString("0.0");
			PlantPrefab pp = parcelGO.GetComponentInChildren<PlantPrefab>();
			if(pp != null)
				inventory+= "pt:" + pp.plant.plantType.ToString();
		}

		Debug.Log(inventory);

		if(saveLocally){
			Save(inventory);
			
		} else {
			byte[] savedData = StringToBytes(inventory);
			TimeSpan totalPlaytime = DateTime.Now - gameStart;
			Debug.Log(totalPlaytime.TotalMilliseconds);
			SaveGame(gameMetaData, savedData, totalPlaytime);
		}

	}

	void SaveGame (ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		Texture2D savedImage = getScreenshot();

		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime(totalPlaytime)
			.WithUpdatedDescription("Saved game at " + DateTime.Now);
		if (savedImage != null) {
			// This assumes that savedImage is an instance of Texture2D
			// and that you have already called a function equivalent to
			// getScreenshot() to set savedImage
			// NOTE: see sample definition of getScreenshot() method below
			byte[] pngData = savedImage.EncodeToPNG();
			builder = builder.WithUpdatedPngCoverImage(pngData);
		}
		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
	}

	public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			// handle reading or writing of saved game.
		} else {
			// handle error
		}
	}

	public Texture2D getScreenshot() {
		// Create a 2D texture that is 1024x700 pixels from which the PNG will be
		// extracted
		Texture2D screenShot = new Texture2D(1024, 700);

		// Takes the screenshot from top left hand corner of screen and maps to top
		// left hand corner of screenShot texture
		screenShot.ReadPixels(
			new Rect(0, 0, Screen.width, (Screen.width/1024)*700), 0, 0);
		return screenShot;
	}

	byte[] StringToBytes(string text){
		return Encoding.UTF8.GetBytes(text);
	}

	string ByteToString(byte[] bytes){
		return Encoding.UTF8.GetString(bytes);
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

	void CreateParcel(int arrayPos){
		Vector3 position = new Vector3(arrayPos, 0, 0);
		GameObject parcel = Instantiate(tile, position, Quaternion.identity) as GameObject;
		parcel.name = parcel.name + arrayPos;
		garden.Add(parcel);
	}

	public void AddCoin(int change){
		coins = coins + change;
		UIManager.Instance.SetCoinText(coins.ToString());
		GPGSIds.coins = coins;
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

	void UnlockAchivement(string achievement, float completion = 100f){
		Social.ReportProgress(achievement, completion, (bool success) => {
			if(success){
				UIManager.Notify("Achievement unlocked: " + achievement);
			}
		});
	}
	void LoadLocally(){
		if(PlayerPrefs.GetString("inventory") != null){
			LoadData(PlayerPrefs.GetString("inventory"));
		}
	}

	void LoadData(string data){
		string[] listParcel;
		listParcel = data.Split("@"[0]);

		string coin = listParcel[0].Substring(1);
		coins = 0;
		AddCoin(int.Parse(coin));

		for(int s = 1; s < listParcel.Length; s++){
			Debug.Log(listParcel[s]);
			if(listParcel[s] != null && listParcel[s] != ""){
				string ph = listParcel[s].Substring(4, 3);

				if(s-1 >= garden.Count){
					gardenSize++;
					CreateParcel(gardenSize);
				}

					
				garden[s-1].GetComponent<Parcel>().SetpH(float.Parse(ph));

				if(listParcel[s].Contains("pt:")){

					currentParcelGO = garden[s-1];
					GrowThatHere(GardenManager.Instance.PlantFromString(listParcel[s].Substring(10)));
				}
			}

		}
	}

	void Save(string inventory){
		if(!reset){
			Debug.Log("saving locally");
			PlayerPrefs.SetString("inventory", inventory);
			PlayerPrefs.Save();
		}
	}

	public void DeletePlayerPrefs(){
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("PlayerPrefs DELETED");
		reset = true;
	}
}
