using UnityEngine;
using System.Collections;

public class HarvestedPlantPrefab : MonoBehaviour {

	public AudioClip pickUpFx;

	private Vector3 endPosition;
	private float distance;

	// Use this for initialization
	void Start () {
		SoundManager.PlaySound(pickUpFx);
		endPosition = UIManager.Instance.healthSlider.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector3.Slerp(transform.position, endPosition, Time.deltaTime);

		distance = Vector3.Distance(transform.position, endPosition);
//		Debug.Log(distance);
		if(distance < 50f)
			Destroy(gameObject);
	}
}
