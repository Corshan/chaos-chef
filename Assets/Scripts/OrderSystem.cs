using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class OrderSystem : NetworkBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _toppingText;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
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

    // Start is called before the first frame update
    void Start()
    {
        ClearDisplay();
    }

    // Update is called once per frame
    void Update()
    {

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

    public void ClearOrderAndDisplay()
    {
        _currentOrder = new List<PlateItemTags>();
        ClearDisplay();
        ClearOrderAndDisplayClientRpc();
    }

    [ClientRpc]
    public void ClearOrderAndDisplayClientRpc()
    {
        _currentOrder = new List<PlateItemTags>();
        ClearDisplay();
    }

    public bool CheckOrder(List<PlateItemTags> tags)
    {
        tags.Sort();
        return _currentOrder.SequenceEqual(tags);
    }

    private void TriggerEffects()
    {
        _particleSystem.Play();
        _audioSource.PlayOneShot(_audioClip);
    }

    [ClientRpc]
    public void TriggereffectsClientRpc()
    {
        TriggerEffects();
    }
}
