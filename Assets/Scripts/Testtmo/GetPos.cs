using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPos : MonoBehaviour
{
    Transform t;
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();  
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(t.position);
    }
}
