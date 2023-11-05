using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grill : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        var b = other.GetComponentInChildren<BurgerHealth>();

        b.isCooking = true;
    }

    private void OnTriggerExit(Collider other) {
        var b = other.GetComponentInChildren<BurgerHealth>();
        b.isCooking = false;
    }
}
