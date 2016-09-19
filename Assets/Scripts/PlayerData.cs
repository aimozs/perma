using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PlayerData {

	[Header ("General")]
	public int coins;
	public float hunger;

	public List<SerializablePlant> lsPlants = new List<SerializablePlant>();
	public List<SerializableParcel> lsParcels = new List<SerializableParcel>();
}
