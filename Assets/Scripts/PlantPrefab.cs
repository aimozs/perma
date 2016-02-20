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

	void OnEnable(){
		GameManager.PlantingThat += UpdateFF;
	}

	void OnDisable(){
		GameManager.PlantingThat -= UpdateFF;
	}

	void UpdateFF(Parcel parcel, Plant newPlant){
		Parcel thisParcel = GetComponentInParent<Parcel>();
		if(parcel != null) {
//			Debug.Log(Vector3.Distance(parcel.transform.position, thisParcel.transform.position));
			float distance = Vector3.Distance(parcel.transform.position, thisParcel.transform.position);
			if(0.1f < distance && distance <= 1.1f){
				if(plant.friends.Contains(newPlant.plantType)){
//					Debug.Log("contains new plant as friend for " + plant.plantType);
					friendStatus = FriendStatus.friend;
					parcel.GetComponentInChildren<PlantPrefab>().friendStatus = FriendStatus.friend;
//					Debug.Log("current parcel friend status for " + parcel.GetComponentInChildren<PlantPrefab>().plant.plantType + ": " + parcel.GetComponentInChildren<PlantPrefab>().friendStatus);

				} else {
					if(plant.foes.Contains(newPlant.plantType)){
//						Debug.Log("contains new plant as friend");
						friendStatus = FriendStatus.foe;
					} else {
						friendStatus = FriendStatus.none;
//						Debug.Log("DOESNT contains new plant as friend");
					}
				}


			}
		} else {
			UIManager.Notify("Couldn't load parcel");
		}
	}

//	IEnumerator SetFriendDelayed
	
	public void IncreaseSize(bool up){
		if(up){
			if(size <= plant.maxSize){
				size++;

				if(size >= plant.germinationSize) {
					if(size == plant.germinationSize) {
						IncreaseToSpecificStage(Plant.stageEnum.germination);
					}
					if(germinationPrefab != null){
						IncreaseModelScale(germinationPrefab);
					}
				}

				if(size >= plant.growthSize) {
					if(size == plant.growthSize)
						IncreaseToSpecificStage(Plant.stageEnum.growth);
					if(growthPrefab != null)
						IncreaseModelScale(growthPrefab);
				}

				if(size >= plant.pollinationSize) {
					if(size == plant.pollinationSize)
						IncreaseToSpecificStage(Plant.stageEnum.pollination);
					if(pollinationPrefab != null)
						IncreaseModelScale(pollinationPrefab);
				}

				if(size >= plant.productSize) {
					if(size == plant.productSize)
						IncreaseToSpecificStage(Plant.stageEnum.product);
					if(productPrefab != null)
						IncreaseModelScale(productPrefab);
				}
			} else {
				if(productPrefab == null){
					Parcel thisParcel = GetComponentInParent<Parcel>();
					if(thisParcel != null) {
						Debug.Log(thisParcel.name);
						if(pollinationPrefab != null){
							GardenManager.Instance.ResetCycle(thisParcel, plant);
						} else {
							GameManager.Instance.ResetParcel(thisParcel);
						}
					} else {
						UIManager.Notify("Could not get PP's parcel");
					}
				}
			}
		}
	}

	void IncreaseModelScale(GameObject go){
		Vector3 scale = go.transform.localScale;
		go.transform.localScale = new Vector3(1+size/10f, 1+size/10f, 1+size/10f);
			
		if(GameManager.Instance.debugGame)
			Debug.Log("And increasing the scale of " + plant.plantType.ToString() + " to " + scale);
	}

//	public void IncreaseStage(){
//		switch(plantStage){
//		case Plant.stageEnum.seedling:
//			plantStage = Plant.stageEnum.germination;
//			GameObject germinatation = (GameObject)Instantiate(GameModel.Instance.germination, transform.position,  Quaternion.Euler(-90, 0, 0));
//			germinatation.transform.SetParent(transform);
//			break;
//		case Plant.stageEnum.germination:
//			plantStage = Plant.stageEnum.pollination;
//			GameObject growth = (GameObject)Instantiate(plant.growth, transform.position, Quaternion.Euler(-90, 0, 0));
//			growth.transform.SetParent(transform);
//			break;
//		case Plant.stageEnum.pollination:
//			plantStage = Plant.stageEnum.product;
//			GameObject product = (GameObject)Instantiate(plant.product, transform.position, Quaternion.Euler(-90, 0, 0));
//			product.transform.SetParent(transform);
//			break;
//		default:
//			break;

	public void IncreaseToSpecificStage(Plant.stageEnum stage){
		
		switch(stage){
		case Plant.stageEnum.germination:
			germinationPrefab = (GameObject)Instantiate(GameModel.Instance.germination, transform.position,  Quaternion.Euler(-90, 0, 0));
			germinationPrefab.transform.SetParent(transform);
			plantStage = stage;
			break;
		case Plant.stageEnum.growth:
			if(plant.growth != null) {
				growthPrefab = (GameObject)Instantiate(plant.growth, transform.position, Quaternion.Euler(-90, 0, 0));
				growthPrefab.transform.SetParent(transform);
				plantStage = stage;
			}
			if(germinationPrefab != null)
				Destroy(germinationPrefab);
			break;
		case Plant.stageEnum.pollination:
			if(plant.pollination != null) {
				pollinationPrefab = (GameObject)Instantiate(plant.pollination, transform.position, Quaternion.Euler(-90, 0, 0));
				pollinationPrefab.transform.SetParent(transform);
				plantStage = stage;
			}
			break;
		case Plant.stageEnum.product:
			if(plant.product != null) {
				productPrefab = (GameObject)Instantiate(plant.product, transform.position, Quaternion.Euler(-90, 0, 0));
				productPrefab.transform.SetParent(transform);
				plantStage = stage;
			}

//			if(pollinationPrefab != null)
//				Destroy(pollinationPrefab);
			break;
		default:
			break;
		}

		if(GameManager.Instance.debugGame)
			Debug.Log("raising the stage of " + plant.plantType.ToString() + " to " + stage);
	}
}
