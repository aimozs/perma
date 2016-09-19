using UnityEngine;

using System;
using System.Collections;

[Serializable]
public class SerializableParcel {

	[Header ("Transform")]
	public float posX;
	public float posY;
	public float posZ;

	[Header ("Plant")]
	public string plantType;
	public string plantStage;
	public int plantSize;
	public int productNumber;
	public int seedNumber;
	public bool hasFlowers;
	public bool hasProduct;


}
