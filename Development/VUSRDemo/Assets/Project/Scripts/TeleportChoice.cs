using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportChoice : MonoBehaviour 
{
    public GameObject Tripod;
    public GameObject[] SpawnPoints;

    public void GoToOrigin()
    {
       
            Tripod.transform.position = SpawnPoints[0].transform.position;

    }

    public void GoToPoint1()
    {
 
            Tripod.transform.position = SpawnPoints[1].transform.position;

    }

    public void GoToPoint2()
    {

            Tripod.transform.position = SpawnPoints[2].transform.position;

    }

    public void GoToPoint3()
    {

        Tripod.transform.position = SpawnPoints[3].transform.position;
    }
}
