using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BurgerHealth : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Transform _canvasTransform;
    // [SerializeField] private RectTransform _canvasRectRansform;
    [SerializeField] private Vector3 _canvasOffset;
    [SerializeField] private Vector3 _canvasRotation;
    private bool isCooking = false;
    private Transform _cameraTransform;

    public bool IsCooking
    {
        set { isCooking = value; }
    }

    public bool IsVisable {
        set { _canvasTransform.gameObject.SetActive(value); }
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthBar.fillAmount = 0;
        _cameraTransform = Camera.main.transform;
        _canvasTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    
        var rot = Quaternion.LookRotation(_canvasTransform.position - _cameraTransform.position);
        _canvasTransform.rotation = new Quaternion(0, rot.y, 0, rot.w);


        _canvasTransform.position = transform.position + _canvasOffset;

        if (isCooking)
        {
            _healthBar.fillAmount += _speed / 100 * Time.deltaTime;
        }
    }
}
