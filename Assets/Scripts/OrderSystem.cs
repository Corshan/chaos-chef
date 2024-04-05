using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class OrderSystem : NetworkBehaviour
{
    [SerializeField] private int _timerAmount = 60;
    [Header("UI")]
    [SerializeField] private Image _timerImage;
    [SerializeField][Range(0, 1)] private float _percentageToRed = 0.3f;
    [SerializeField] private List<TextMeshProUGUI> _toppingText;
    [Header("Particle system")]
    [SerializeField] private ParticleSystem _particleSystem;
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    [Header("NPC")]
    [SerializeField] private GameObject _npc;
    [SerializeField] private Transform _counterPos;
    [SerializeField] private Transform _startPos;
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
    private Animator _anim;
    private AnimatorStateInfo _info;
    private NavMeshAgent _agent;
    private NPC _npcSettings;
    private NetworkVariable<float> _timer = new(0);
    private NetworkVariable<bool> _inRound = new(false);
    private NetworkVariable<Vector3> _target = new();
    private NetworkVariable<bool> _agentIsStopped = new(false);
    private bool _hasOrdered = false;
    private bool _newNpcModel = false;

    // Start is called before the first frame update
    void Start()
    {
        _anim = _npc.GetComponent<Animator>();
        _npcSettings = _npc.GetComponent<NPC>();

        _info = _anim.GetCurrentAnimatorStateInfo(0);
        _agent = _npc.GetComponent<NavMeshAgent>();

        // _startPos = transform.position;

        _target.OnValueChanged += OnTargetChanged;
        _agentIsStopped.OnValueChanged += OnAgentChanged;

        ClearDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        _timerImage.fillAmount = _timer.Value / (float)_timerAmount;
        _agent.isStopped = _agentIsStopped.Value;

        if (!NetworkManager.Singleton.IsServer) return;

        NpcState();
    }


    public void Innit()
    {
        GenerateOrder();
        ClearDisplay();
        DisplayOrder();

        _timer.Value = _timerAmount;
        _inRound.Value = true;
    }

    public void RoundOver()
    {
        ClearOrderAndDisplay();
        _timer.Value = _timerAmount;

        _inRound.Value = false;
        _agentIsStopped.Value = false;

        _anim.SetTrigger("leave");
        _anim.ResetTrigger("toCounter");
    }

    public void RoundStart()
    {
        _anim = _npc.GetComponent<Animator>();
        _anim.SetTrigger("toCounter");
    }

    private void NpcState()
    {
        _info = _anim.GetCurrentAnimatorStateInfo(0);

        if (_info.IsName("Move to Counter")) MovetoCounter();
        else if (_info.IsName("Wait for Order")) WaitForOrder();
        else if (_info.IsName("Leave")) Leave();
        else if (_info.IsName("IDLE")) Idle();
    }

    private void Idle()
    {
        ResetTriggers();

        if (_inRound.Value) _anim.SetTrigger("toCounter");

        if (!_newNpcModel)
        {
            _npcSettings.ChooseModel();
            _newNpcModel = true;
        }
    }

    private void MovetoCounter()
    {
        _target.Value = _counterPos.position;

        if (Vector3.Distance(_npc.transform.position, _counterPos.position) < 1)
        {
            _anim.SetTrigger("wait");
            _timer.Value = _timerAmount;
        }
    }

    private void WaitForOrder()
    {
        _timer.Value -= Time.deltaTime;

        if (!_hasOrdered)
        {
            Innit();
            _hasOrdered = true;

            _timerImage.color = Color.white;
            ChangeImageColorClientRpc(Color.white);

            _agentIsStopped.Value = true;
        }

        if (_timer.Value <= 0)
        {
            _anim.SetTrigger("leave");
            ClearOrderAndDisplay();
        }

        if ((_timer.Value / _timerAmount) < _percentageToRed)
        {
            _timerImage.color = Color.red;
            ChangeImageColorClientRpc(Color.red);
        }
    }

    private void Leave()
    {
        _target.Value = _startPos.position;
        if (Vector3.Distance(_npc.transform.position, _startPos.position) < 1)
        {
            _anim.SetTrigger("idle");
            _npcSettings.ShowBurgerBox(false);
            _newNpcModel = false;
        }

        _agentIsStopped.Value = false;
        ClearOrderAndDisplay();
        _hasOrdered = false;
    }

    private void ResetTriggers()
    {
        _anim.ResetTrigger("wait");
        _anim.ResetTrigger("leave");
        _anim.ResetTrigger("idle");
    }

    private void OnTargetChanged(Vector3 previousValue, Vector3 newValue)
    {
        _agent.SetDestination(newValue);
    }

    private void OnAgentChanged(bool previousValue, bool newValue)
    {
        _agent.isStopped = newValue;
    }

    public void TriggerOrderDone()
    {
        TriggerEffects();
        TriggereffectsClientRpc();

        _anim.SetTrigger("success");
        _npcSettings.ShowBurgerBox(true);
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
        _timer.Value = _timerAmount;
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

    [ClientRpc]
    public void ChangeImageColorClientRpc(Color color)
    {
        _timerImage.color = color;
    }
}
