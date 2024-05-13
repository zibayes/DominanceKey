using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public void OnTriggerEnter(UnityEngine.Collider collider)
    {
        Light light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        light.enabled = true;
    }

    public void OnTriggerExit(UnityEngine.Collider collider)
    {
        Light light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        light.enabled = false;
    }
}
