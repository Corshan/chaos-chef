using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NPC : NetworkBehaviour
{
    [SerializeField] private GameObject _burgerBox;
    [SerializeField] private MeshFilter _hairMeshFilter;
    [SerializeField] private List<GameObject> _models;
    [SerializeField] private List<Mesh> _hairMeshes;
    private NetworkVariable<int> _currentModelIndex = new(0);
    private NetworkVariable<int> _currentHairIndex = new(0);

    // Start is called before the first frame update
    void Start()
    {
        _burgerBox.SetActive(false);

        _currentModelIndex.OnValueChanged += OnModelChange;
        _currentHairIndex.OnValueChanged += OnHairChange;

        if (NetworkManager.Singleton.IsClient)
        {
            _hairMeshFilter.mesh = _hairMeshes[_currentHairIndex.Value];

            ResetAllModels();

            _models[_currentModelIndex.Value].SetActive(true);
        }

    }

    private void OnHairChange(int previousValue, int newValue)
    {
        _hairMeshFilter.mesh = _hairMeshes[newValue];
    }

    private void OnModelChange(int previousValue, int newValue)
    {
        ResetAllModels();

        _models[newValue].SetActive(true);
    }

    public void ShowBurgerBox(bool value)
    {
        _burgerBox.SetActive(value);
        ShowBurgerBoxClientRpc(value);
    }


    [ClientRpc]
    private void ShowBurgerBoxClientRpc(bool value)
    {
        _burgerBox.SetActive(value);
    }

    private void ResetAllModels()
    {
        _models.ForEach(item => item.SetActive(false));
    }

    public void ChooseModel()
    {
        int randomNum, randomHair;

        do
        {
            randomNum = Random.Range(0, _models.Count);
            randomHair = Random.Range(0, _hairMeshes.Count);

            var prevModel = _models[_currentModelIndex.Value];
            var currentModel = _models[randomNum];

            prevModel.SetActive(false);
            currentModel.SetActive(true);

            if (currentModel.CompareTag("santa")) _hairMeshFilter.mesh = null;
            else _hairMeshFilter.mesh = _hairMeshes[randomHair];

        } while (randomNum == _currentModelIndex.Value);

        _currentModelIndex.Value = randomNum;
        _currentHairIndex.Value = randomHair;
    }
}
