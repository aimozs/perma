using UnityEngine;
using System.Collections;

public class PlantPrefab : MonoBehaviour {

	public enum FriendStatus {none, friend, foe}
	public Plant plant;
	public Plant.stageEnum plantStage = Plant.stageEnum.seedling;
	public GameObject germinationPrefab;
	public GameObject growthPrefab;
	public GameObject pollinationPrefab;
	public GameObject productPrefab;
	public int size = 1;
	public FriendStatus friendStatus = FriendStatus.none;


	private bool alreadyProduced = false;

	#region 0.Basics
	void OnEnable(){
//		GardenManager.PlantingThat += UpdateFF;
		ClimateManager.OnTriggerClimate += TestGrowth;
	}

	void OnDisable(){
//		GardenManager.PlantingThat -= UpdateFF;
		ClimateManager.OnTriggerClimate -= TestGrowth;
	}

	#endregion

	#region 1.Statics

	#endregion

	#region 2.Publics
	public void IncreaseSize(bool up){
		if(plant != null){
			if(up){
				if(size <= plant.maxSize){
					size++;

					if(size >= plant.productSize){
						if(!alreadyProduced){
							IncreaseToSpecificStage(Plant.stageEnum.product, false, true);
							alreadyProduced = true;
						}
					} else {
						if(size >= plant.pollinationSize)
							IncreaseToSpecificStage(Plant.stageEnum.pollination,true, false);
						else {
							if(size >= plant.growthSize){
								IncreaseToSpecificStage(Plant.stageEnum.growth);
							} else {
								if(size == plant.germinationSize)
									IncreaseToSpecificStage(Plant.stageEnum.germination);
							}
						}
					}

				} else {
					if(productPrefab == null){
						Parcel thisParcel = GetComponentInParent<Parcel>();
						if(thisParcel != null) {
							if(pollinationPrefab != null){
								GardenManager.Instance.ResetCycle(thisParcel, plant);
							} else {
								GardenManager.Instance.ResetParcel(thisParcel);
							}
						} else {
							UIManager.Notify("Could not get PP's parcel");
						}
					}
				}
				transform.localScale = new Vector3(1+size/10f, 1+size/10f, 1+size/10f);

			}
		}
	}

	public void IncreaseToSpecificStage(Plant.stageEnum stage, bool hasFlowers = false, bool hasProducts = false){
		plantStage = stage;

		switch(stage){
		case Plant.stageEnum.germination:
			germinationPrefab = (GameObject)Instantiate(GameModel.Instance.germination, transform.position,  Quaternion.Euler(-90, 0, 0));
			germinationPrefab.transform.SetParent(transform);
			break;
		case Plant.stageEnum.growth:
			AddGrowth();
			break;
		case Plant.stageEnum.pollination:
			if(hasFlowers)
				AddPollination();
			break;
		case Plant.stageEnum.product:
			if(hasFlowers)
				AddPollination();
			if(hasProducts)
				AddProduct();
			break;
		default:
			break;
		}
	}

//	public void UpdateFF(Parcel parcel, Plant newPlant){
//		
//		if(parcel != null) {
//			Parcel thisParcel = GetComponentInParent<Parcel>();
//
//			if(thisParcel != null){
//				float distance = Vector3.Distance(parcel.transform.position, thisParcel.transform.position);
//
//				if(0.1f < distance && distance <= 1.1f){
//					if(plant.friends.Contains(newPlant.plantName)){
//						friendStatus = FriendStatus.friend;
//						parcel.GetComponentInChildren<PlantPrefab>().friendStatus = FriendStatus.friend;
//
//					} else {
//						if(plant.foes.Contains(newPlant.plantName)){
//							StartCoroutine(PlantedFoe());
//						}
//					}
//				}
//			}
//		}
//	}

	#endregion

	#region 3.Privates

	void TestGrowth(Climate climate){
		IncreaseSize(true);
	}
	
	void AddGrowth(){
		if(plant.growth != null && growthPrefab == null) {
			growthPrefab = (GameObject)Instantiate(plant.growth, transform.position, Quaternion.Euler(-90, 0, 0));
			growthPrefab.transform.SetParent(transform);
			growthPrefab.transform.localScale = Vector3.one;
		}

		if(germinationPrefab != null)
			Destroy(germinationPrefab);
	}

	void AddPollination(){
		if(plant.pollination != null && pollinationPrefab == null) {
			pollinationPrefab = (GameObject)Instantiate(plant.pollination, transform.position, Quaternion.Euler(-90, 0, 0));
			pollinationPrefab.transform.SetParent(transform);
			pollinationPrefab.transform.localScale = Vector3.one;
		}

		AddGrowth();
	}

	void AddProduct(){
		if(plant.product != null && productPrefab == null) {
			productPrefab = (GameObject)Instantiate(plant.product, transform.position, Quaternion.Euler(-90, 0, 0));
			productPrefab.transform.SetParent(transform);
			productPrefab.transform.localScale = Vector3.one;
		}

		AddGrowth();
	}

	#endregion
}
