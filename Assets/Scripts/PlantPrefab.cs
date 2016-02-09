using UnityEngine;
using System.Collections;

public class PlantPrefab : MonoBehaviour {

	public Plant plant;
	public Plant.stageEnum plantStage = Plant.stageEnum.seedling;
	public GameObject germinationPrefab;
	public GameObject growthPrefab;
	public GameObject pollinationPrefab;
	public GameObject productPrefab;
	public float size = 1;
	
	public void IncreaseSize(bool up){
		if(up)
			size++;
//		else
//			size--;

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
	}

	void IncreaseModelScale(GameObject go){
		Vector3 scale = go.transform.localScale;
		go.transform.localScale = new Vector3(1+size/10, 1+size/10, 1+size/10);
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
			break;
		case Plant.stageEnum.growth:
			if(plant.growth != null)
				growthPrefab = (GameObject)Instantiate(plant.growth, transform.position, Quaternion.Euler(-90, 0, 0));
			growthPrefab.transform.SetParent(transform);
			break;
		case Plant.stageEnum.pollination:
			if(plant.pollination != null)
				pollinationPrefab = (GameObject)Instantiate(plant.pollination, transform.position, Quaternion.Euler(-90, 0, 0));
			pollinationPrefab.transform.SetParent(transform);
			break;
		case Plant.stageEnum.product:
			if(plant.product != null)
				productPrefab = (GameObject)Instantiate(plant.product, transform.position, Quaternion.Euler(-90, 0, 0));
			productPrefab.transform.SetParent(transform);
			break;
		default:
			break;
		}
		plantStage = stage;

		Debug.Log("raising the stage of " + plant.plantType.ToString() + " to " + stage);
	}
}
