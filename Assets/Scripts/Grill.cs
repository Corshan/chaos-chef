using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        var b = other.GetComponentInChildren<BurgerHealth>();

        b.IsCooking = true;
        b.IsVisable = true;
    }

    private void OnTriggerExit(Collider other) {
        var b = other.GetComponentInChildren<BurgerHealth>();
        b.IsCooking = false;
        b.IsVisable = false;
    }
}
