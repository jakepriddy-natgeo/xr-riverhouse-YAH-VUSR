using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigFollower : MonoBehaviour {
   public GameObject rig;
	void Update () {
        transform.position = rig.transform.position;
	}

}
