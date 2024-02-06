using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlateItems : MonoBehaviour
{
    [SerializeField] private List<string> _items;
    // [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _transform;
    [SerializeField] private GameObject _bunBottomModel;
    [SerializeField] private GameObject _pattyModel;
    [SerializeField] private GameObject _cheeseModel;
    [SerializeField] private GameObject _tomatoModel;
    [SerializeField] private GameObject _onionModel;
    [SerializeField] private GameObject _pickleModel;
    [SerializeField] private GameObject _bunTopModel;

    private bool _isBottomBun = false;
    private bool _isPatty = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bottom Bun") && !_items.Contains(other.tag))
        {
            other.gameObject.SetActive(false);
            _bunBottomModel.SetActive(true);
            _items.Add(other.tag);
            _isBottomBun = true;
        }

        if (other.CompareTag("Burger") && !_items.Contains(other.tag) && _isBottomBun)
        {
            other.gameObject.SetActive(false);
            _pattyModel.SetActive(true);
            _items.Add(other.tag);
            _isPatty = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _bunBottomModel.SetActive(false);
        _pattyModel.SetActive(false);
        _cheeseModel.SetActive(false);
        _tomatoModel.SetActive(false);
        _onionModel.SetActive(false);
        _pickleModel.SetActive(false);
        _bunTopModel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // [System.Obsolete]
    // public void OnSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
    // {
    //     Debug.Log("grab");
    //     Debug.Log(selectEnterEventArgs.interactorObject.transform.name);
    //     selectEnterEventArgs.manager.ForceSelect(VrRigReferences.Singleton._rightHandInteractor, _items[^1].GetComponent<XRGrabInteractable>());
    // }
}
