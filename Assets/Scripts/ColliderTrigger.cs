using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ColliderTrigger : MonoBehaviour
{
    [SerializeField] public UnityEvent MyEvent;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            MyEvent.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
