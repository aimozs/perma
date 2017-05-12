using UnityEngine;
using System.Collections;

public class HarvestedPlantPrefab : MonoBehaviour {

	public AudioClip pickUpFx;

	private Vector3 _endPosition;
	private float distance;

	// Use this for initialization
	void Start () {
		SoundManager.PlaySound(pickUpFx);
	}
	
	// Update is called once per frame
	void Update () {
//		if(_endPosition != null)
		transform.position = Vector3.Slerp(transform.position, _endPosition, Time.deltaTime);

		distance = Vector3.Distance(transform.position, _endPosition);
//		Debug.Log(distance);
		if(distance < 50f)
			Destroy(gameObject);
	}

	public void SetDestination(Vector3 position){
		_endPosition = position;
	}
}
