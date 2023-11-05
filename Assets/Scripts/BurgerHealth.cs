using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerHealth : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private float _speed = 1f;
    public bool isCooking = false;

    // Start is called before the first frame update
    void Start()
    {
        _healthBar.fillAmount = 0;
    }

    void UpdateHealthBar(float max, float current)
    {
        _healthBar.fillAmount = current / max;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = new Vector3(transform.position.x - Camera.main.transform.position.x, 0, 0);
        transform.rotation = Quaternion.LookRotation(pos);

        if(isCooking){
            _healthBar.fillAmount += _speed / 100 * Time.deltaTime;
        }
    }

    void OnTriggerEnter(){
        isCooking = true;
    }

    void OnTriggerExit(){
        isCooking = false;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.tag);
    }
}
