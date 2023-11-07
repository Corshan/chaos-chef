using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBorad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation.SetLookRotation(transform.position - Camera.main.transform.position);
    }
}
