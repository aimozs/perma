using UnityEngine;
using System.Collections;

public class PlantPrefab : MonoBehaviour {

	public Plant plant;
	public Plant.stageEnum plantStage = Plant.stageEnum.seedling;
	public float size = 1;

	// Use this for initialization
	void Start () {
//		Instantiate(GameModel.Instance.seedling);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IncreaseSize(float pH){
		if(4< pH && pH < 11)
			size++;
		else
			size--;

		if(size == 3)
			IncreaseStage();

		if(size == 5)
			IncreaseStage();
	}

	public void IncreaseStage(){
		switch(plantStage){
		case Plant.stageEnum.seedling:
			plantStage = Plant.stageEnum.germination;
			GameObject germinatation = (GameObject)Instantiate(GameModel.Instance.germination, transform.position,  Quaternion.Euler(-90, 0, 0));
			germinatation.transform.SetParent(transform);
			break;
		case Plant.stageEnum.germination:
			plantStage = Plant.stageEnum.growth;
			GameObject growth = (GameObject)Instantiate(plant.growth, transform.position, Quaternion.Euler(-90, 0, 0));
			growth.transform.SetParent(transform);
			break;
		case Plant.stageEnum.growth:
			plantStage = Plant.stageEnum.product;
			GameObject product = (GameObject)Instantiate(plant.product, transform.position, Quaternion.Euler(-90, 0, 0));
			product.transform.SetParent(transform);
			break;
		default:
			break;
		}
	}
}
