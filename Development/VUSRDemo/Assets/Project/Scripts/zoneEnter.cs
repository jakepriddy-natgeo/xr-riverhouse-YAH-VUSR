using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

    
public class zoneEnter : MonoBehaviour
{
    public UnityEvent onZoneEnter; 
    public UnityEvent onZoneStay;
    public UnityEvent onZoneExit; 

    private void OnTriggerEnter()
    {
        if (onZoneEnter != null)
        {
            onZoneEnter.Invoke();
        }

    }

    private void OnTriggerStay()
    {
        if (onZoneStay != null)
        {
            onZoneStay.Invoke();
        }
    }

    private void OnTriggerExit()
    {
        if (onZoneExit != null)
        {
            onZoneExit.Invoke();
        }
    }


}
