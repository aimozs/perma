using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CustomInputManager : MonoBehaviour {

	public float rotSpeed = 1f;

	public float cameraFOVmin = 40f;
	public float cameraFOVmax = 70f;
	public float perspectiveZoomSpeed = 0.5f;
	public float orthoZoomSpeed = 0.5f;

	const float MOVE_SPEED = .5f;
	private float yRot = 0;
	private float xRot = 0;

	private Vector3 gardenPos;
	private float gardenArea;
	private Camera custCamera;
	private GameObject terrain;

	private float angle;
	private float previousAngle;
	private Vector3 dragOrigin;

	private Transform cursorTransform;
	private Transform m_CameraTarget;

	Touch touch0;
	Touch touch1;

	private static CustomInputManager instance;
	public static CustomInputManager Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<CustomInputManager>();
			}
			return instance;
		}
	}

	#region 0.Basics

//	void Awake(){}

	void Start(){
		custCamera = Camera.main;
		gardenPos = GardenManager.Instance.transform.position;
		gardenArea = GardenManager.Instance.gardenArea;
		terrain = GameObject.FindGameObjectWithTag("Terrain");
		m_CameraTarget = custCamera.transform.parent.transform;
	}

//	void Update(){}

//	void FixedUpdate(){}

	void LateUpdate () {
		if(!UIManager.IsCooking)
			GetInput();
	}

	#endregion

	#region 1.Statics

	#endregion

	#region 2.Publics

	#endregion

	#region 3.Privates

	void GetInput(){

		#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR

		if(Input.touchCount == 1){
			touch0 = Input.GetTouch(0);
			GetClick(touch0.position);
			if (touch0.phase == TouchPhase.Moved) {
				Vector2 touchDeltaPosition = touch0.deltaPosition;
				Vector3 move = new Vector3(-touchDeltaPosition.x * MOVE_SPEED *0.2f, 0f,-touchDeltaPosition.y * MOVE_SPEED *0.2f);

				TranslateConfined(move);
			}

		} else {

			if(Input.touchCount == 2){
				
				

				touch0 = Input.GetTouch(0);
				touch1 = Input.GetTouch(1);

				if(touch1.phase == TouchPhase.Ended){
					previousAngle = 0;
				}

				PinchToZoom(touch0, touch1);
				DragToRotate(touch0, touch1);
				
			}
		}

		#else

		float zoomDelta = Input.mouseScrollDelta.y;
		if(zoomDelta < -.1f || .1f < zoomDelta){
			ApplyZoom(zoomDelta);
		}

		DragCameraOnClick();

		if(Input.GetButtonDown("Fire1")){
			Vector2 pos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			GetClick(pos);
		}

		if(Input.GetButton("Fire2")){
			ClickToRotate();
		}

		#endif
	}

	#region Mouse controls
	void ClickToRotate(){
		yRot += Input.GetAxis("Mouse X");
		xRot += Input.GetAxis("Mouse Y");
		ApplyXRotation(xRot);
		ApplyYRotation(yRot);
	}

	void DragCameraOnClick(){
		if (Input.GetMouseButtonDown(0))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		if (!Input.GetMouseButton(0)) return;

		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector3 move = new Vector3(-pos.x * MOVE_SPEED, 0f, -pos.y * MOVE_SPEED);

		TranslateConfined(move);
	}

	#endregion

	#region Touch controls

	void DragToRotate(Touch touch0, Touch touch1){

		Vector2 newAngle = new Vector2(touch1.position.y - touch0.position.y, touch1.position.x - touch0.position.x);

		angle = Mathf.Atan2(newAngle.x, newAngle.y) * Mathf.Rad2Deg;

		float deltaHeight = touch1.deltaPosition.y - touch0.deltaPosition.y;
		ApplyXRotation(deltaHeight);

		float deltaDegree = 0;
		if(angle < -1f || angle > 1f)
			deltaDegree = angle - previousAngle;

		previousAngle = angle;

		if(deltaDegree != 0f)
			ApplyYRotation(deltaDegree);
		
//		UIManager.Notify(deltaDegree.ToString("0.F2"));
//		Vector3 rot = new Vector3(deltaHeight, deltaDegree, 0);

	}

	void PinchToZoom(Touch touch0, Touch touch1){

		{
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touch0.position - touch0.deltaPosition;
			Vector2 touchOnePrevPos = touch1.position - touch1.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touch0.position - touch1.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			ApplyZoom(deltaMagnitudeDiff);
		}
	}

	#endregion


	#region From Controls to Commons

	void ApplyXRotation(float xRot){
		Vector3 newCamRot = new Vector3(Mathf.Clamp(m_CameraTarget.eulerAngles.x + xRot, 20f, 40f), m_CameraTarget.eulerAngles.y, 0f);
		m_CameraTarget.rotation = Quaternion.Euler(newCamRot);
	}

	void ApplyYRotation(float yRot){
		cursorTransform = GameManager.CursorTransform;
		Vector3 newCursorRot = new Vector3(0f, cursorTransform.rotation.eulerAngles.y + yRot, 0f);
		cursorTransform.rotation = Quaternion.Euler(newCursorRot);
	}

	void ApplyZoom(float deltaMagnitudeDiff){
		// If the camera is orthographic...
		if (custCamera.orthographic)
		{
			// ... change the orthographic size based on the change in distance between the touches.
			custCamera.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

			// Make sure the orthographic size never drops below zero.
			custCamera.orthographicSize = Mathf.Max(custCamera.orthographicSize, 0.1f);
		}
		else
		{
			// Otherwise change the field of view based on the change in distance between the touches.
			custCamera.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

			// Clamp the field of view to make sure it's between 0 and 180.
			custCamera.fieldOfView = Mathf.Clamp(custCamera.fieldOfView, cameraFOVmin, cameraFOVmax);
		}
	}

	void GetClick(Vector2 screenPos){
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 1000)) 
		{
//			Debug.Log(hit.collider.gameObject.name);
			PickUp pp = hit.collider.GetComponentInChildren<PickUp>();
			if(pp != null){
				pp.Harvest();
			}
		}
	}
	#endregion


	void TranslateConfined(Vector3 direction){
		cursorTransform = GameManager.CursorTransform;
		cursorTransform.Translate(direction, Space.Self);
	}
//
//	void MoveToPoint(){
//
//		#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
//			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
//				// Check if finger is over a UI element
//				if(EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
//					return;
//				}
//			}
//		#else
//			if(EventSystem.current.IsPointerOverGameObject(-1))
//				return;
//		#endif
//
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//		RaycastHit hit;
//		Vector3 worldPosition = Vector3.zero;
//		if (terrain.GetComponent<Collider>().Raycast (ray, out hit, 1000)) 
//		{
//			worldPosition = hit.point;
//			worldPosition.x = Mathf.Clamp(worldPosition.x, gardenPos.x - gardenArea, gardenPos.x + gardenArea);
//			worldPosition.z = Mathf.Clamp(worldPosition.z, gardenPos.z - gardenArea, gardenPos.z + gardenArea);
//
//			GameManager.SetCamera(worldPosition);
//		}
//	}
	#endregion
}
