using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateItems : NetworkBehaviour
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
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.GetComponent<ItemTag>() == null) return;

        var tag = other.GetComponent<ItemTag>().itemTag;
        var otherNetworkObject = other.GetComponent<NetworkObject>();

        if (tag == PlateItemTags.BUTTOM_BUN)
        {
            var go = _dict[tag];
            otherNetworkObject.Despawn();

            go.transform.position = _attachPoint.position;
            other.gameObject.SetActive(false);

            go.SetActive(true);
            _items.Add(tag);

            _isBottomBun = true;
            PlaceClientRpc(tag, _attachPoint.position);
        }
        else if (tag == PlateItemTags.BURGER_PATTY && _isBottomBun)
        {
            var burger = other.GetComponent<BurgerHealth>();
            if(burger.state == BurgerState.COOKED){
                ActiveModel(tag, other.gameObject, other.transform.parent.name);
                other.GetComponent<NetworkOwnerShip>().GetNetworkObject().Despawn();
                burger.DeactiveClientRPC();
                _isPatty = true;
            }
        }
        else if (_dict.ContainsKey(tag) && _isBottomBun && _isPatty && !_isTopBun)
        {
            ActiveModel(tag, other.gameObject);
            otherNetworkObject.Despawn();
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

    private void ActiveModel(PlateItemTags tag, GameObject other, string name = "")
    {
        Debug.Log(tag);

        var go = _dict[tag];
        var prevGo = _dict[_items[^1]];
        go.transform.position = prevGo.GetComponent<SnapLocations>().top.position;

        other.SetActive(false);
        go.SetActive(true);
        _items.Add(tag);

        PlaceClientRpc(tag, go.transform.position, name);
    }

    [ClientRpc]
    public void PlaceClientRpc(PlateItemTags tag, Vector3 pos, string name = "")
    {
        var go = _dict[tag];
        go.transform.position = pos;
        go.SetActive(true);

        if (name.Equals("")) return;

        var otherGo = GameObject.Find(name);
        otherGo.SetActive(false);
    }
}
