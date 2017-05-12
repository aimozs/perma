using UnityEngine;
using UnityEngine.SocialPlatforms;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

#if (UNITY_ANDROID)

using GooglePlayGames;
using GooglePlayGames.OurUtils;

using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

#endif




public class SaveManager : MonoBehaviour {

	public static bool loadFinished = false;

	public static string fileName = "playerInfo.dat";

	private static bool _signedIn = false;
	private static DateTime gameStart;
	#if (UNITY_ANDROID)
	private static ISavedGameMetadata gameMetaData;

	#endif

	private static byte[] dataBytes;

	#region 0.Basics

//	void Awake(){}

//	void Start(){}

//	void Update(){}

//	void LateUpdate(){}

	#endregion

	#region 1.Statics

	public static bool signedIn {
		get { return _signedIn; }
		set {
			_signedIn = value;
			UIManager.signedIn = _signedIn;
		}
	}

	public static void InitSave(){
		#if (UNITY_ANDROID)
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
			.EnableSavedGames()
			// registers a callback to handle game invitations received while the game is not running.
			//	.WithInvitationDelegate(<callback method>)
			// registers a callback for turn based match notifications received while the
			// game is not running.
			//	.WithMatchDelegate(<callback method>)
			// require access to a player's Google+ social graph (usually not needed)
			//	.RequireGooglePlus()
			.Build();

		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();

		#endif
	}

	public static void SignIn(){
		SetGameStart();

		Social.localUser.Authenticate((bool success) => {
			signedIn = success;

		});

	}

	public static void SignOut(){
		#if (UNITY_ANDROID)
		PlayGamesPlatform.Instance.SignOut();
		signedIn = false;
		#endif
	}

	public static void SaveGame(){
		if(loadFinished){
			PlayerData playerData = new PlayerData();
			playerData.coins = GameManager.Instance.coins;
			playerData.hunger = UIManager.hungerLevel;
			playerData.lsPlants = GetListOfSerializablePlant();
			playerData.lsParcels = GetListOfSerializableParcel();

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = null;

			if(File.Exists(Application.persistentDataPath + fileName)){
				file = File.OpenWrite(Application.persistentDataPath + fileName);
			} else {
				file = File.Create(Application.persistentDataPath + fileName);
			}
			bf.Serialize(file, playerData);

			UIManager.DisplaySaveState(UIManager.SaveState.saved);

			if(_signedIn){
				UIManager.Notify("Saving online");
				#if (UNITY_ANDROID)
				OpenSavedGame(fileName, SaveOnSavedGameOpened);
				#endif
			} else {
				file.Close();
			}
		}
	}

	public static void DeleteSaveFile(){
		
		File.Delete(Application.persistentDataPath + fileName);
		UIManager.DisplaySaveState(UIManager.SaveState.error);
	}

	public static void LoadGame(){
		if(_signedIn){
			#if (UNITY_ANDROID)
			OpenSavedGame(fileName, LoadOnSavedGameOpened);
			#endif
		} else {
			DeserializeFromFile();
		}

		loadFinished = true;

	}
	#if (UNITY_ANDROID)
	/// <summary>
	/// Opens GPGS saved game
	/// </summary>
	/// <param name="_filename">Filename.</param>
	/// <param name="callback">Callback.</param>
	static void OpenSavedGame(string _filename, Action<SavedGameRequestStatus, ISavedGameMetadata> callback) {
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(_filename, DataSource.ReadNetworkOnly,
				ConflictResolutionStrategy.UseLongestPlaytime, callback);
	}

	/// <summary>
	/// Prepare data to be saved on GPGS after saved game has been opened
	/// </summary>
	/// <param name="status">Status.</param>
	/// <param name="_gameMD">Game M.</param>
	static void SaveOnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata _gameMD) {
		if (status == SavedGameRequestStatus.Success) {
			UIManager.DisplaySaveState(UIManager.SaveState.open);
			UIManager.Notify(File.Exists(Application.persistentDataPath + fileName).ToString());
			if(File.Exists(Application.persistentDataPath + fileName)){
				UIManager.Notify("to bytes");
				byte[] savedData = File.ReadAllBytes(Application.persistentDataPath + fileName);
				UIManager.Notify("savedData lenght " + savedData.Length);
				TimeSpan totalPlaytime = (DateTime.Now - gameStart) + _gameMD.TotalTimePlayed;
				SetGameStart();

				SaveOnGPGS(_gameMD, savedData, totalPlaytime);
			} else {
				UIManager.Notify("file doesnt exist");
			}
		} else {
			// handle error
		}
	}

	/// <summary>
	/// Restore game data after saved game has been opened on GPGS
	/// </summary>
	/// <param name="status">Status.</param>
	/// <param name="_gameMD">Game M.</param>
	static void LoadOnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata _gameMD) {
		if (status == SavedGameRequestStatus.Success) {
			ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
			savedGameClient.ReadBinaryData(_gameMD, OnSavedGameDataRead);
		} else {
			// handle error
		}
	}

	#endif

	/// <summary>
	/// Open local file, deserialize and restore locally saved data
	/// </summary>
	static void DeserializeFromFile(){
//		UIManager.Notify("deserialize local file");
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
		PlayerData _playerData = (PlayerData)bf.Deserialize(file);
		file.Close();
		UIManager.DisplaySaveState(UIManager.SaveState.loaded);

		ApplyRetreivedPlayerData(_playerData);
	}

	#if (UNITY_ANDROID)
	/// <summary>
	/// Prepare retreived data from GPGS saved game to be applied
	/// </summary>
	/// <param name="status">Status.</param>
	/// <param name="data">Data.</param>
	static void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		UIManager.Notify("game data read status " + status.ToString());
		if (status == SavedGameRequestStatus.Success) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(Application.persistentDataPath + "retreivedGPGS.dat");
			File.WriteAllBytes(Application.persistentDataPath + "retreivedGPGS.dat", data);
			PlayerData _playerData = (PlayerData)bf.Deserialize(file);
			ApplyRetreivedPlayerData(_playerData);
			UIManager.DisplaySaveState(UIManager.SaveState.loadOnline);
		}
	}

	#endif

	/// <summary>
	/// Applies the retreived player data class.
	/// </summary>
	/// <param name="_pd">Pd.</param>
	static void ApplyRetreivedPlayerData(PlayerData _pd){
		GameManager.Instance.coins = _pd.coins;
		UIManager.Instance.SetCoinText();
		UIManager.hungerLevel = _pd.hunger;
		RestoreParcelsFromSerializedList(_pd.lsParcels);
		RestorePlantsFromSerializedList(_pd.lsPlants);
	}

	static List<SerializableParcel> GetListOfSerializableParcel(){
		List<SerializableParcel> listSerializableParcels = new List<SerializableParcel>();
		Parcel[] parcels = GameObject.FindObjectsOfType<Parcel>();

		foreach(Parcel parcel in parcels){
			PlantPrefab pp = parcel.GetComponentInChildren<PlantPrefab>();
			if(pp != null){
				SerializableParcel _sp = new SerializableParcel();
				_sp.posX = parcel.transform.position.x;
				_sp.posY = parcel.transform.position.y;
				_sp.posZ = parcel.transform.position.z;

//				Debug.Log("serialized plant: " + pp.plant.plantName.ToString());
				_sp.plantType = pp.plant.plantName.ToString();
				_sp.plantStage = pp.plantStage.ToString();
				_sp.plantSize = pp.size;

				_sp.hasProduct = pp.productPrefab != null;
				_sp.hasFlowers = pp.pollinationPrefab != null;

				listSerializableParcels.Add(_sp);
			}

		}
		return listSerializableParcels;
	}

	static List<SerializablePlant> GetListOfSerializablePlant(){
		List<SerializablePlant> listSerializablePlants = new List<SerializablePlant>();
		Plant[] _plants = GameObject.FindObjectsOfType<Plant>();

		foreach(Plant _plant in _plants){
			SerializablePlant _sp = new SerializablePlant();

			_sp.plantName = _plant.plantName.ToString();
			_sp.productNumber = _plant.productNumber;
			_sp.seedNumber = _plant.seedNumber;

			listSerializablePlants.Add(_sp);
		}
		return listSerializablePlants;
	}

	static void RestoreParcelsFromSerializedList(List<SerializableParcel> lsParcels){
		foreach(SerializableParcel lsParcel in lsParcels){
			Plant plant = GardenManager.PlantFromString(lsParcel.plantType);

			Vector3 pos = new Vector3(lsParcel.posX, lsParcel.posY, lsParcel.posZ);
			GardenManager.GrowAtPosition(plant, pos, (Plant.stageEnum)Enum.Parse(typeof(Plant.stageEnum), lsParcel.plantStage),lsParcel.plantSize, lsParcel.hasFlowers, lsParcel.hasProduct);
		}
	}

	static void RestorePlantsFromSerializedList(List<SerializablePlant> lsPlants){
		foreach(SerializablePlant lsPlant in lsPlants){
			Plant plant = GardenManager.PlantFromString(lsPlant.plantName);
			plant.productNumber = lsPlant.productNumber;
			plant.seedNumber = lsPlant.seedNumber;
		}

		UIManager.DisplaySaveState(UIManager.SaveState.loaded);
	}

	static void SetGameStart(){
		gameStart = DateTime.Now;
	}

//	static void DeserializeData(string data){
//		string[] ListRecipes;
//		ListRecipes = data.Split("@"[0]);
//
//		for(int s = 0; s < ListRecipes.Length; s++){
//
//			if(ListRecipes[s] != null && ListRecipes[s] != ""){
//
//				string recipe = ListRecipes[s];
//				string recipeName = recipe.Substring(0, recipe.Length - 10);
//				//				Debug.Log(recipeName);
//				int suc = int.Parse(recipe.Substring(recipe.Length - 9, 4));
//				//				Debug.Log(suc);
//				int tri = int.Parse(recipe.Substring(recipe.Length - 4, 4));
//				//				Debug.Log(tri);
//
//				//				if(s-1 >= GameManager.Instance.garden.Count){
//				//					GameManager.Instance.gardenSize++;
//				//					GameManager.Instance.CreateParcel(GameManager.Instance.gardenSize);
//				//				}
//				//
//				//
//				//				GameManager.Instance.garden[s-1].GetComponent<Parcel>().SetpH(float.Parse(ph));
//				//
//				//				if(listParcel[s].Contains("pt:")){
//				//
//				//					GameManager.Instance.currentParcelGO = GameManager.Instance.garden[s-1];
//				//					GameManager.Instance.currentParcelGO.GetComponent<Parcel>().SetPlant(GardenManager.Instance.PlantFromString(listParcel[s].Substring(10)));
//				//				}
//			}
//
//		}
//		loadFinished = true;
//		TutorialManager.Instance.ShowNextTip();
//	}
	#if (UNITY_ANDROID)
	static void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata _gameMD) {
		if (status == SelectUIStatus.SavedGameSelected) {
			// handle selected game save
			ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
			savedGameClient.ReadBinaryData(_gameMD, OnSavedGameDataRead);
		} else {
			// handle cancel or error
		}
	}

	/// <summary>
	/// Prepare updated metadata and commit update to GPGS
	/// </summary>
	/// <param name="_gameMetaData">Game meta data.</param>
	/// <param name="savedData">Saved data.</param>
	/// <param name="totalPlaytime">Total playtime.</param>
	static void SaveOnGPGS (ISavedGameMetadata _gameMetaData, byte[] savedData, TimeSpan totalPlaytime) {
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
		UIManager.Notify("commiting");
		savedGameClient.CommitUpdate(_gameMetaData, updatedMetadata, savedData, OnSavedGameWritten);

	}

	static void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			UIManager.Notify("Cloud saved");
			UIManager.DisplaySaveState(UIManager.SaveState.savedOnline);
		}
	}



	public static void ClearSavedGames(){
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(
			fileName, 
			DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLongestPlaytime, 
			DeleteGames);
	}

	static void DeleteGames(SavedGameRequestStatus status, ISavedGameMetadata gameMetaData){
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.Delete(gameMetaData);
	}


	public static void ShowSelectUI() {

		uint maxNumToDisplay = 5;
		bool allowCreateNew = true;
		bool allowDelete = true;

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		if(savedGameClient != null){
			UIManager.Notify("showing online saved games ");

			savedGameClient.ShowSelectSavedGameUI("Select saved game",
				maxNumToDisplay,
				allowCreateNew,
				allowDelete,
				OnSavedGameSelected);
		} else {
			UIManager.Notify("client is null");
		}
	}

	#endif

	static Texture2D getScreenshot() {
		// Create a 2D texture that is 1024x700 pixels from which the PNG will be
		// extracted
		Texture2D screenShot = new Texture2D(1024, 700);

		// Takes the screenshot from top left hand corner of screen and maps to top
		// left hand corner of screenShot texture
		screenShot.ReadPixels(
			new Rect(0, 0, Screen.width, (Screen.width/1024)*700), 0, 0);
		return screenShot;
	}

	static byte[] StringToBytes(string text){
		return Encoding.UTF8.GetBytes(text);
	}

	static string BytesToString(byte[] bytes){
		return Encoding.UTF8.GetString(bytes);
	}

//	static void SaveLocally(string inventory){
//		UIManager.Notify("saving locally");
//		PlayerPrefs.SetString("inventory", inventory);
//		PlayerPrefs.Save();
//	}
//
//	static void LoadLocally(){
//		if(PlayerPrefs.GetString("inventory") != null){
//			DeserializeData(PlayerPrefs.GetString("inventory"));
//		}
//	}

	static void DeletePlayerPrefs(){
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("PlayerPrefs DELETED");
	}

	static void UnlockAchivement(string achievement, float completion = 100f){
		Social.ReportProgress(achievement, completion, (bool success) => {
			if(success){
				UIManager.Notify("Achievement unlocked: " + achievement);
			}
		});
	}
	#endregion

	#region 2.Publics

	#endregion

	#region 3.Privates

	#endregion


}
