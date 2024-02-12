using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateItems : MonoBehaviour
{
    private List<PlateItemTags> _items = new();
    [SerializeField] private List<GameObject> _models;
    [SerializeField] private Transform _attachPoint;
    private Dictionary<PlateItemTags, GameObject> _dict = new();

    private bool _isBottomBun = false;
    private bool _isTopBun = false;
    private bool _isPatty = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemTag>() == null) return;

        var tag = other.GetComponent<ItemTag>().itemTag;

        if (tag == PlateItemTags.BUTTOM_BUN)
        {
            var go = _dict[tag];

            go.transform.position = _attachPoint.position;
            other.gameObject.SetActive(false);

            go.SetActive(true);
            _items.Add(tag);

            _isBottomBun = true;
        }
        else if (tag == PlateItemTags.BURGER_PATTY && _isBottomBun)
        {
            ActiveModel(tag, other.gameObject);
            _isPatty = true;
        }
        else if (_dict.ContainsKey(tag) && _isBottomBun && _isPatty && !_isTopBun)
        {
            ActiveModel(tag, other.gameObject);
            if (tag == PlateItemTags.TOP_BUN) _isTopBun = true;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in _models)
        {
            item.SetActive(false);
            var tag = item.GetComponent<ItemTag>().itemTag;

            _dict.Add(tag, item);
        }
    }

    private void ActiveModel(PlateItemTags tag, GameObject other)
    {
        Debug.Log(tag);

        var go = _dict[tag];
        var prevGo = _dict[_items[^1]];
        go.transform.position = prevGo.GetComponent<SnapLocations>().top.position;

        other.SetActive(false);
        go.SetActive(true);
        _items.Add(tag);
    }
}
