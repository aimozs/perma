using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Parcel : MonoBehaviour {

	private MeshFilter _parcelMesh;

	void Awake(){
		_parcelMesh = GetComponentInChildren<MeshFilter>();
	}

	public Mesh mesh{
		set { _parcelMesh.mesh = value; }
	}

//	public void GetSeedOrProduct(){
//		PlantPrefab pp = GetComponentInChildren<PlantPrefab>();
//		if(pp != null){
//			switch(pp.plantStage){
//			case Plant.stageEnum.pollination:
//				if(pp.pollinationPrefab != null)
//					pp.pollinationPrefab.GetComponent<PickUp>().Harvest();
//				break;
//			case Plant.stageEnum.product:
//				if(pp.productPrefab != null)
//					pp.productPrefab.GetComponent<PickUp>().Harvest();
//				break;
//			default:
//				UIManager.Notify("There's nothing to harvest at that stage.");
//				break;
//			}
//
//		} else {
//			UIManager.Notify("You try to look for results, but you havent started yet. Use the shovel and plant a seed first!");
//		}
//	}

	void OnTriggerEnter(Collider other){
		GardenCursor.currentPlantTrigger = this.gameObject;

	}

	void OnTriggerExit(Collider other){
		GardenCursor.currentPlantTrigger = null;

	}

}
