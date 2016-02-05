using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parcel : MonoBehaviour {

  public float pH;
  
  void Start(){
    SetpH();
  }
  
  void SetpH(){
    pH = UnityEngine.Random.Range(3f, 11f);
  }

	public void SetPlant(Plant plant){
		GameObject newPlant = Instantiate(GameModel.Instance.plantPrefab) as GameObject;
		newPlant.GetComponent<PlantPrefab>().plant = plant;
		newPlant.transform.SetParent(transform, false);
	}

	public void ReceivesWater(){
		if(GetComponentInChildren<PlantPrefab>() != null)
			GetComponentInChildren<PlantPrefab>().IncreaseSize(pH);
	}
}
