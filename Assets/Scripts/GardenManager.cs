using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GardenManager : MonoBehaviour {

	public bool debugGarden;
//	public float gardenArea = 20f;
	public GameObject currentPlant;
	public int gardenSize;
	[Range(1f, 50f)]
	public float waterCoef = 10f;
	[Range(1f, 50f)]
	public float tempCoef = 40f;
	[Range(1f, 50f)]
	public float phCoef = 200f;

	public Dictionary<string, Plant> AllPlants = new Dictionary<string, Plant>();

//	public delegate void Planting(Parcel parcel, Plant plant);
//	public static event Planting PlantingThat;

	private static List<ConditionTrigger> epicenterWater = new List<ConditionTrigger>();
	private static List<ConditionTrigger> epicenterCold = new List<ConditionTrigger>();
	private static List<ConditionTrigger> epicenterpH = new List<ConditionTrigger>();

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

	public void InitGarden(){
		InitPlants();
		InitEpicenters();
	}

	public static void InitPlants(){
		FindAndAddPlantsToDict();

		//From generated dictionnary, create related btn in shop panel
		foreach(KeyValuePair<string, Plant> plant in Instance.AllPlants){
				UIManager.Instance.AddBtnPlant(plant.Value);
		}
		UIManager.Instance.InitPanelsSize();

		UIManager.Instance.DisplaySelectedPlantType("herbs");

	}

	public static void InitEpicenters(){
		ConditionTrigger[] conditionTriggers = FindObjectsOfType<ConditionTrigger>();
		foreach(ConditionTrigger ct in conditionTriggers){
			switch(ct.condition){
			case ConditionTrigger.Condition.water:
				epicenterWater.Add(ct);
				break;
			case ConditionTrigger.Condition.temp:
				epicenterCold.Add(ct);
				break;
			case ConditionTrigger.Condition.pH:
				epicenterpH.Add(ct);
				break;
			}
		}

//		Debug.Log("Number of water epicenter found: " + epicenterWater.Count);
//		Debug.Log("Number of cold epicenter found: " + epicenterCold.Count);
//		Debug.Log("Number of ph epicenter found: " + epicenterpH.Count);
	}

	public static float GetWaterIndex(){
		float index = 0;
		float closestEpicenter = 999f;
		foreach(ConditionTrigger ew in epicenterWater){
			float distance = Vector3.Distance(ew.transform.position, GardenCursor.cursorPosition);
			if(distance < closestEpicenter){
				closestEpicenter = distance;
			}
		}
//		Debug.Log("closest water source: " + closestEpicenter);
		index = 1 - closestEpicenter / Instance.waterCoef;

//		Debug.Log("water epic index: " + index);
		return index;
	}

	public static float GetWaterIndex(Vector3 _specificPos){
		float index = 0;
		float closestEpicenter = 999f;
		foreach(ConditionTrigger ew in epicenterWater){
			float distance = Vector3.Distance(ew.transform.position, _specificPos);
			if(distance < closestEpicenter){
				closestEpicenter = distance;
			}
		}
		//		Debug.Log("closest water source: " + closestEpicenter);
		index = 1 - closestEpicenter / Instance.waterCoef;

		//		Debug.Log("water epic index: " + index);
		return index;
	}

	public static float GetTempIndex(){
		float index = 0;
		float closestEpicenter = 999f;
		foreach(ConditionTrigger ec in epicenterCold){
			float distance = Vector3.Distance(ec.transform.position, GardenCursor.cursorPosition);
			if(distance < closestEpicenter){
				closestEpicenter = distance;
			}
		}

		closestEpicenter = Mathf.Clamp(closestEpicenter, 2f, 100f);

		index = closestEpicenter / 50f;

//		Debug.Log("temp epic index: " + index);
		return index;
	}

	public static float GetTempIndex(Vector3 _specificPos){
		float index = 0;
		float closestEpicenter = 999f;
		foreach(ConditionTrigger ec in epicenterCold){
			float distance = Vector3.Distance(ec.transform.position, _specificPos);
			if(distance < closestEpicenter){
				closestEpicenter = distance;
			}
		}

		closestEpicenter = Mathf.Clamp(closestEpicenter, 2f, 100f);

		index = closestEpicenter / 50f;

		//		Debug.Log("temp epic index: " + index);
		return index;
	}

	public static float GetpHIndex(){
		float index = 7;
//		float closestEpicenter = 999f;
		foreach(ConditionTrigger eph in epicenterpH){
			float distance = Vector3.Distance(eph.transform.position, GardenCursor.cursorPosition);


			float ephi = eph.phVal - 7 / distance;
			index += ephi;
//			Debug.Log(eph.name + " index:" + ephi + "(" + distance + "*" + eph.val + ")");
//			if(distance < closestEpicenter){
//				closestEpicenter = distance;
//			}
		}

//		index = 1 - (Instance.phCoef * );
//		index = (index / epicenterpH.Count) *  Instance.phCoef;
//		Debug.Log("ph epic index: " + index / 14f);

		return index / 14f;
	}

	public static float GetpHIndex(Vector3 _specificPos){
		float index = 7;
		//		float closestEpicenter = 999f;
		foreach(ConditionTrigger eph in epicenterpH){
			float distance = Vector3.Distance(eph.transform.position, _specificPos);
			float ephi = eph.phVal - 7 / distance;
			index += ephi;
		}

		return index / 14f;
	}

	public static void FindAndAddPlantsToDict () {
		Plant[] plants = FindObjectsOfType<Plant>();

		foreach(Plant plant in plants){
			Instance.AllPlants.Add(plant.plantName.ToString(), plant);
		}
	}

	public static void IncreaseSeedNumber(Plant plant, bool inc){
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
			GameObject _waste = Instantiate(GameModel.Instance.waste);
			_waste.transform.SetParent(parcel.transform, false);
		}
	}

//	public void GrowThatHere(Plant plant){
////		GameObject newParcel = Instantiate(GameModel.Instance.parcelReady);
//		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
////		newPlant.transform.SetParent(GardenCursor.Instance.transform, false);
//		newPlant.transform.SetParent(GardenCursor.currentPlantTrigger.transform, false);
//		newPlant.GetComponent<PlantPrefab>().plant = plant;
//		if(plant.condition == currentCondition){
//			Debug.Log(currentCondition.ToString());
//			newPlant.GetComponent<ParticleSystem>().Play();
//		}
//		IncreaseSeedNumber(plant, false);
//	}

	public static void GrowAtPosition(Plant plant, Vector3 pos, Plant.stageEnum stage = Plant.stageEnum.germination, int plantSize = 1, bool hasFlowers = false, bool hasProduct = false){
		Parcel newParcel = NewParcelAtPos(pos);

		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		newPlant.transform.SetParent(newParcel.transform, false);
		newParcel.mesh = GameModel.Instance.seedling;

		PlantPrefab newPP = newPlant.GetComponent<PlantPrefab>();
		newPP.plant = plant;
		newPP.soilBonus = GetSoilBonus(plant, pos);
		newPP.IncreaseToSpecificStage(stage, hasFlowers, hasProduct);
		newPP.size = plantSize;
		newPP.IncreaseSize(true);

	}

	public static int GetSoilBonus(Plant plant, Vector3 pos){
		int bonus = 0;

		if(plant.pHAve - 1 < GetpHIndex(pos) * 14f && GetpHIndex(pos) * 14f < plant.pHAve + 1){
			bonus++;
//			Debug.Log("soil bonus +1 ph");
		}

		if(plant.tempMin < GetTempIndex(pos) * 50f && GetTempIndex(pos) * 50f < plant.tempMax){
			bonus++;
//			Debug.Log("soil bonus +1 temp");
		}

		float waterIndex = GetWaterIndex(pos);
		switch(plant.waterNeeds){
		case Plant.waterEnum.generous:
			if(.75 < waterIndex){
				bonus++;
//				Debug.Log("soil bonus +1 water generous");
			}
			break;
		case Plant.waterEnum.little:
			if(.5 < waterIndex && waterIndex < .75){
				bonus++;
//				Debug.Log("soil bonus +1 water little");
			}
			break;
		case Plant.waterEnum.moist:
			if(.25 < waterIndex && waterIndex < .5){
				bonus++;
//				Debug.Log("soil bonus +1 water moist");
			}
			break;
		default:
			if(waterIndex < .25){
				bonus++;
//				Debug.Log("soil bonus +1 water dry");
			}
			break;
		}

		return bonus;
	}

	#endregion

	#region Tool Action

	public void UseWateringCan(){
//		if(Well.Instance.levelUI.value > .1f){
//			Well.Instance.UpdateLevel(false);
//		} else {
//			UIManager.Notify("The well is dry.");
//		}
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
		UIManager.DisplayHarvestedProduct(pp);
		Destroy(pp.productPrefab);
	}

	public static void HarvestSeed(PlantPrefab pp){
		//		Debug.Log("harvesting");
		IncreaseSeedNumber(pp.plant, true);
		UIManager.DisplayHarvestedSeed(pp);
		Destroy(pp.pollinationPrefab);
	}

	public void UseShovel(){
		if(currentPlant != null){
			Destroy(currentPlant);
		} else {
			NewParcelAtPos(GameManager.CursorTransform.position);
		}
		SoundManager.PlayShovel();
	}

	static Parcel NewParcelAtPos(Vector3 pos){
		GameObject newParcel = Instantiate(GameModel.Instance.parcelReady);
		newParcel.transform.position = pos;
		return newParcel.GetComponent<Parcel>();
	}

	#endregion

}
