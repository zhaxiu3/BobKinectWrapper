using UnityEngine;
using System.Collections;

public class setAspecrRatio : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.camera.aspect = (float)Screen.width / (float)Screen.height;
	}
}
