using UnityEngine;
using System.Collections;

public class GardenCursor : MonoBehaviour {

	public GameObject plantTrigger;
	public MeshFilter _previewPlant;
	private MeshRenderer meshRend;

	private static DeferredNightVisionEffect _visionEffect;
	private static int _triggerCounter = 0;


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

	void Start(){
//		_visionEffect = GetComponentInChildren<DeferredNightVisionEffect>();
//		_visionEffect.enabled = false;
//		meshRend = GetComponentInChildren<MeshRenderer>();
	}

//	void Update(){}

//	void FixedUpdate(){}

	#endregion

	#region 1.Statics
	public static GameObject currentPlantTrigger{
		get {return Instance.plantTrigger;}
		set {
			if(value != null){
				_triggerCounter++;
				Instance.plantTrigger = value;
			} else {
				_triggerCounter--;
				if(_triggerCounter == 0){
					Instance.plantTrigger = null;
					UIManager.Instance.DisplayMenu(false);
				}
			}

//			Instance.UpdateCursor(currentPlantTrigger);

		}
	}

	public static Vector3 cursorPosition{
		get { return Instance.transform.position; }
	}


	public static void DisplayPreviewPlant(Plant plant = null){
		if(plant != null){
			Instance._previewPlant.mesh = plant.growth.GetComponent<MeshFilter>().sharedMesh;
		 } else {
			Instance._previewPlant.mesh = null;

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




}
