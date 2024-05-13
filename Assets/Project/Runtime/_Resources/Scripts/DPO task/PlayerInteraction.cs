using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public void OnTriggerEnter(UnityEngine.Collider collider)
    {
        collider.GetComponent<Actor>().OnLight();
    }

    public void OnTriggerExit(UnityEngine.Collider collider)
    {
        collider.GetComponent<Actor>().OffLight();
    }
}
