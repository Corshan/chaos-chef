using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BurgerHealth : NetworkBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Vector3 _canvasOffset;
    [SerializeField] private Mesh _cookedMeshFilter;
    [SerializeField] private Mesh _burntMeshFilter;
    private MeshFilter _currentMeshFilter;
    public float cookedAmount = 0;
    private bool _isVisable = false;
    private Transform _cameraTransform;
    public BurgerState state = BurgerState.NOT_COOKED;
    public bool IsVisable
    {
        set
        {
            _isVisable = value;
            // _canvasTransform.gameObject.SetActive(value);
            _canvas.gameObject.SetActive(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _healthBar.fillAmount = 0;
        // _canvasTransform.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(false);
        _cameraTransform = Camera.main.transform;
        _currentMeshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isVisable || state == BurgerState.BURNT) return;

        var rot = Quaternion.LookRotation(_canvasTransform.position - _cameraTransform.position);
        _canvasTransform.rotation = new Quaternion(0, rot.y, 0, rot.w);

        _canvasTransform.position = transform.position + _canvasOffset;

        _healthBar.fillAmount = cookedAmount;

        if (NetworkManager.Singleton.IsServer) RunOnServer();
        // _canvas.enabled = true;
    }

    [ClientRpc]
    public void UpdateBurgerAmountClientRPC(float newCookedAmount)
    {
        cookedAmount = newCookedAmount;
    }

    [ClientRpc]
    public void BurgerStateChangeClientRPC(BurgerState s)
    {
        state = s;

        if (state == BurgerState.COOKED)
        {
            _currentMeshFilter.mesh = _cookedMeshFilter;
        }
        else if (state == BurgerState.BURNT)
        {
            _currentMeshFilter.mesh = _burntMeshFilter;
        }
    }

    private void RunOnServer()
    {
        UpdateBurgerAmountClientRPC(cookedAmount);

        if (cookedAmount > 0.7 && cookedAmount < 1)
        {
            _currentMeshFilter.mesh = _cookedMeshFilter;
            state = BurgerState.COOKED;
            BurgerStateChangeClientRPC(state);
        }

        if (cookedAmount > 1)
        {
            _currentMeshFilter.mesh = _burntMeshFilter;
            state = BurgerState.BURNT;
            BurgerStateChangeClientRPC(state);
        }
    }

    [ClientRpc]
    public void DeactiveClientRPC()
    {
        gameObject.SetActive(false);
    }
}
