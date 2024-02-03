using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BurgerHealth : NetworkBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private Vector3 _canvasOffset;
    [SerializeField] private Vector3 _canvasRotation;
    public float cookedAmount;
    private bool _isVisable = false;
    private Transform _cameraTransform;
    public bool IsVisable
    {
        set
        {
            _isVisable = value;
            _canvasTransform.gameObject.SetActive(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthBar.fillAmount = 0;
        _canvasTransform.gameObject.SetActive(false);
        
        if(IsServer) return;
        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isVisable) return;

        var rot = Quaternion.LookRotation(_canvasTransform.position - _cameraTransform.position);
        _canvasTransform.rotation = new Quaternion(0, rot.y, 0, rot.w);

        _canvasTransform.position = transform.position + _canvasOffset;

        _healthBar.fillAmount = cookedAmount;
    }
}
