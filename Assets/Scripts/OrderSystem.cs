using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class OrderSystem : NetworkBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _toppingText;
    private List<PlateItemTags> _currentOrder;
    private List<PlateItemTags> _prevOrder;
    private Dictionary<PlateItemTags, string> _dict = new()
    {
        {PlateItemTags.BUTTOM_BUN, "Button Bun"},
        {PlateItemTags.BURGER_PATTY, "Patty"},
        {PlateItemTags.CHEESE, "Cheese"},
        {PlateItemTags.TOMATO, "Tomato"},
        {PlateItemTags.ONION, "Onion"},
        {PlateItemTags.PICKLES, "Pickles"},
        {PlateItemTags.TOP_BUN, "Top Bun"},
    };

    [SerializeField] private bool _generateOrder = true;

    // private NetworkVariable<int[]> _currentOrderNetwork = new();
    // private NetworkList<int> _currentOrderNetwork = new();

    // Start is called before the first frame update
    void Start()
    {
        ClearDisplay();
        // _currentOrderNetwork.OnListChanged += OnValueChanged;
    }

    // private void OnValueChanged(NetworkListEvent<int> changeEvent)
    // {
    //     ClearDisplay();
    //     DisplayOrder();
    // }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (_generateOrder)
        {
            GenerateOrder();
            ClearDisplay();
            DisplayOrder();
            _generateOrder = false;
        }
    }

    public void Innit()
    {
        GenerateOrder();
        ClearDisplay();
        DisplayOrder();
    }

    void GenerateOrder()
    {
        if (_currentOrder != null) _prevOrder = new List<PlateItemTags>(_currentOrder);

        _currentOrder = new List<PlateItemTags>
        {
            PlateItemTags.BUTTOM_BUN,
            PlateItemTags.BURGER_PATTY
        };

        bool done = false;

        while (_currentOrder.Count < 8 && !done)
        {
            int randomNum = Random.Range(3, 8);

            if (_currentOrder.Contains((PlateItemTags)randomNum)) continue;

            switch (randomNum)
            {
                case (int)PlateItemTags.CHEESE:
                    _currentOrder.Add(PlateItemTags.CHEESE);
                    break;

                case (int)PlateItemTags.TOMATO:
                    _currentOrder.Add(PlateItemTags.TOMATO);
                    break;

                case (int)PlateItemTags.ONION:
                    _currentOrder.Add(PlateItemTags.ONION);
                    break;

                case (int)PlateItemTags.PICKLES:
                    _currentOrder.Add(PlateItemTags.PICKLES);
                    break;

                default:
                    _currentOrder.Add(PlateItemTags.TOP_BUN);
                    done = true;
                    break;
            }
        }

        _currentOrder.Sort();

        if (_prevOrder != null && _currentOrder.All(_prevOrder.Contains)) GenerateOrder();

        // _currentOrderNetwork = _currentOrder.Select(x => (int) x).ToArray();
        // _currentOrderNetwork = new NetworkList<int>(_currentOrder.Select(x => (int) x).ToArray());
        // _currentOrderNetwork.Clear();

        // foreach (var item in _currentOrder)
        // {
        //     _currentOrderNetwork.Add((int)item);
        // }

        GenerateOrderClientRpc(_currentOrder.Select(x => (int)x).ToArray());
    }

    void DisplayOrder()
    {
        for (int i = 0; i < _currentOrder.Count; i++)
        {
            string txt = _dict[_currentOrder[i]];
            _toppingText[i].text = txt;
        }
    }

    void ClearDisplay()
    {
        foreach (var text in _toppingText)
        {
            text.text = "";
        }
    }

    [ClientRpc]
    public void GenerateOrderClientRpc(int[] order)
    {
        _currentOrder = order.Select(x => (PlateItemTags)x).ToList();
        ClearDisplay();
        DisplayOrder();
    }

    // void OnValueChanged(int[] previous, int[] current)
    // {
    //     _currentOrder = current.Select(x => (PlateItemTags) x).ToList();
    // }

    public bool CheckOrder(List<PlateItemTags> tags)
    {
        // return tags.SequenceEqual(_currentOrder);
        tags.Sort();
        return _currentOrder.SequenceEqual(tags);
    }
}
