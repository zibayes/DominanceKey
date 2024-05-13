using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Actor : MonoBehaviour
{
    [SerializeField]
    public UnityEvent onActivateAction;
    // Start is called before the first frame update
    void Start()
    {
        if(onActivateAction == null)
            onActivateAction = new UnityEvent();
    }

    // Update is called once per frame
    private void OnTriggerExit(UnityEngine.Collider collider)
    {
        onActivateAction.Invoke();
    }

    public void OnLight()
    {
        Light light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        light.enabled = true;
    }

    public void OffLight()
    {
        Light light = GameObject.FindGameObjectWithTag("Light").GetComponent<Light>();
        light.enabled = false;
    }


}
