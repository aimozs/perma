using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GardenManager : MonoBehaviour {

	public bool debugGarden;
	public float gardenArea = 20f;
	public GameObject currentPlant;
	public int gardenSize;

	public Dictionary<string, Plant> AllPlants = new Dictionary<string, Plant>();

	public delegate void Planting(Parcel parcel, Plant plant);
	public static event Planting PlantingThat;

	private static ConditionTrigger.Condition _condition;
	public static ConditionTrigger.Condition currentCondition {
		get { return _condition; }
		set { 
			_condition = value;
//			UIManager.Notify(_condition.ToString());
			
		}
	}

	private static GardenManager instance;
	public static GardenManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GardenManager>();
			}
			return instance;
		}
	}

	public void InitPlants(){
		FindAndAddPlantsToDict();

		//From generated dictionnary, create related btn in shop panel
		foreach(KeyValuePair<string, Plant> plant in AllPlants){
				UIManager.Instance.AddBtnPlant(plant.Value);
		}

		UIManager.Instance.DisplaySelectedPlantType("herbs");

	}

	public void FindAndAddPlantsToDict () {
		Plant[] plants = FindObjectsOfType<Plant>();

		foreach(Plant plant in plants){
			AllPlants.Add(plant.plantName.ToString(), plant);
		}
	}

	public void IncreaseSeedNumber(Plant plant, bool inc){
		if(inc)
			plant.seedNumber++;
		else
			plant.seedNumber--;
		plant.plantBtn.GetComponent<BtnPlant>().RefreshInventory();
	}

	public static void IncreaseProductNumber(Plant plant, bool inc){
		if(inc)
			plant.productNumber++;
		else
			plant.productNumber--;
		plant.plantBtn.GetComponent<BtnPlant>().RefreshInventory();
	}

	public static Plant PlantFromString(string plantType){
		Plant plant = Instance.AllPlants[plantType];
		return plant;
	}

	#region GardenSizeAndMaintenance

	public void ResetCycle(Parcel thisParcel, Plant plant){
		ResetParcel(thisParcel);

		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		thisParcel.mesh = GameModel.Instance.seedling;
		newPlant.transform.SetParent(thisParcel.transform, false);
		newPlant.GetComponent<PlantPrefab>().plant = plant;
	}

	public void ResetParcel(Parcel parcel){
		PlantPrefab thisPlant = parcel.GetComponentInChildren<PlantPrefab>();
		if(thisPlant != null){
			Destroy(thisPlant.gameObject);
		}
	}

	public void GrowThatHere(Plant plant){
		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
//		newPlant.transform.SetParent(GardenCursor.Instance.transform, false);
		newPlant.transform.SetParent(GardenCursor.currentPlantTrigger.transform, false);
		newPlant.GetComponent<PlantPrefab>().plant = plant;
		if(plant.condition == currentCondition){
			Debug.Log(currentCondition.ToString());
			newPlant.GetComponent<ParticleSystem>().Play();
		}
		IncreaseSeedNumber(plant, false);
	}

	public static void GrowAtPosition(Plant plant, Vector3 pos, Plant.stageEnum stage = Plant.stageEnum.germination, int plantSize = 1, bool hasFlowers = false, bool hasProduct = false){
		Parcel newParcel = NewParcelAtPos(pos);

		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		newPlant.transform.SetParent(newParcel.transform, false);
		newParcel.mesh = GameModel.Instance.seedling;

		PlantPrefab newPP = newPlant.GetComponent<PlantPrefab>();
		newPP.plant = plant;
		newPP.IncreaseToSpecificStage(stage, hasFlowers, hasProduct);
		newPP.size = plantSize;
		newPP.IncreaseSize(true);
	}

	#endregion

	#region Tool Action

	public void UseWateringCan(){
		if(Well.Instance.levelUI.value > .1f){
			Well.Instance.UpdateLevel(false);
		} else {
			UIManager.Notify("The well is dry.");
		}
	}

	public void UseSecator(){
		if(currentPlant != null){
			PlantPrefab pp = currentPlant.GetComponentInChildren<PlantPrefab>();
			if(pp != null){
				switch(pp.plantStage){
				case Plant.stageEnum.pollination:
					if(pp.pollinationPrefab != null)
						IncreaseSeedNumber(pp.plant, true);
						Destroy(pp.pollinationPrefab);
					break;
				case Plant.stageEnum.product:
					if(pp.productPrefab != null){
						IncreaseProductNumber(pp.plant, true);
						Destroy(pp.productPrefab);
					}
					break;
				default:
					UIManager.Notify("There's nothing to harvest at that stage.");
					break;
				}

			} else {
				UIManager.Notify("Plant something first.");
			}
		} else {
			UIManager.Notify("Use the shovel to start planting.");
		}
	}

	public static void HarvestProduct(PlantPrefab pp){
//		Debug.Log("harvesting");
		IncreaseProductNumber(pp.plant, true);
		UIManager.DisplayHarvestedPlant(pp);
		Destroy(pp.productPrefab);
	}

	public void UseShovel(){
		if(currentPlant != null){
			Destroy(currentPlant);
		} else {
			NewParcelAtPos(GameManager.CursorTransform.position);
		}
	}

	static Parcel NewParcelAtPos(Vector3 pos){
		GameObject newParcel = Instantiate(GameModel.Instance.parcelReady);
		newParcel.transform.position = pos;
		return newParcel.GetComponent<Parcel>();
	}

	#endregion

}
