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

}
