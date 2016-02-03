using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClimateManager : MonoBehaviour {

  public List<Climate> AllClimates = new List<Climate>();
  
  public delegate void EmitClimate();
  public static event EmitClimate OnTriggerClimate;
  
	private static ClimateManager instance;
	public static ClimateManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<ClimateManager>();
			}
			return instance;
		}
	}
	
	void Start(){
	  InitClimate();
	
	  if(OnTriggerClimate != null)
	    OnTriggerClimate();
	}
	
	public void GetAllPlants () {
	Climate[] climates = GameObject.FindObjectsOfType<Climate>();

	foreach(Climate climate in climates){
		AllClimates.Add(climate.climateType.ToString(), climate);
	}
}
	
	void InitClimate(){
	  
	}
	


}
