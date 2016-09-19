using UnityEngine;
using System.Collections;

public class GardenCursor : MonoBehaviour {

	public GameObject plantTrigger;
	public GameObject areaMarker;
	public ParticleSystem highlight;
	private MeshRenderer meshRend;

	private static GardenCursor instance;
	public static GardenCursor Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GardenCursor>();
			}
			return instance;
		}
	}

	#region 0.Basics
//	void Awake(){}

//	void Start(){
//		meshRend = GetComponentInChildren<MeshRenderer>();
//	}

//	void Update(){}

//	void FixedUpdate(){}

	#endregion

	#region 1.Statics
	public static GameObject currentPlantTrigger{
		get {return Instance.plantTrigger;}
		set {
			Instance.plantTrigger = value;
			Instance.UpdateCursor(currentPlantTrigger);
		}
	}

	#endregion

	#region 2.Publics


	#endregion


	public Vector3 position{
		get {return gameObject.transform.position;}
	}

	void UpdateCursor(GameObject plant){
		GardenManager.Instance.currentPlant = plantTrigger;
		if(plant != null){
//			meshRend.material.color = Color.red;
//			highlight.Play();
//			areaMarker.SetActive(true);
			UIManager.Instance.DisplayMenu(true);
		} else {
//			meshRend.material.color = Color.green;
//			highlight.Stop();
//			areaMarker.SetActive(false);
			UIManager.Instance.DisplayMenu(false);
		}
	}

	public static void UpdatePlantSurface(float rayon){
		Instance.areaMarker.transform.localScale = new Vector3(rayon, rayon, 1f);
	}

}
