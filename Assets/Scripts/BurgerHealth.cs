using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerHealth : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private RectTransform _canvasRectRansform;
    [SerializeField] private Vector3 _canvasOffset;
    private bool isCooking = false;
    private Transform _cameraTransform;

    public bool IsCooking
    {
        set { isCooking = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthBar.fillAmount = 0;
        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = new Vector3(_canvasTransform.position.x - _cameraTransform.position.x, 0, 0);
        // _canvasTransform.rotation = Quaternion.LookRotation(pos);
        // var rot = Quaternion.LookRotation(pos);
        _canvasRectRansform.rotation = Quaternion.LookRotation(pos);
        
        // Debug.Log(pos);
        // Debug.Log(_cameraTransform.rotation);
        // Debug.Log(_canvasTransform.rotation);


        _canvasTransform.position = transform.position + _canvasOffset;

        if (isCooking)
        {
            _healthBar.fillAmount += _speed / 100 * Time.deltaTime;
        }
    }
}
