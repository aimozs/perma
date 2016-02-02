﻿using UnityEngine;
using System.Collections;

public class GameModel : MonoBehaviour {

	public GameObject btnPlantPrefab;

	private static GameModel instance;
	public static GameModel Instance {
		get {
			if(instance == null) {
				instance = GameObject.FindObjectOfType<GameModel>();
			}
			return instance;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
