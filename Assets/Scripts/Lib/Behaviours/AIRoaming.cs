using System;
using System.Collections;
using System.Collections.Generic;
using Lib;
using Lib.Behaviours.AIStates;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class AIRoaming : MonoBehaviour
{
    [Header("Roam settings")]
    [SerializeField] float roamRadius;
    [SerializeField] float roamDelay;
    [SerializeField] float roamSpeed;

    [Header("Chase settings")]
    [SerializeField] float visibilityRange;
    [SerializeField] float endChaseRadius;
    [SerializeField] float chaseSpeed;
    [SerializeField] string chaseForTag = "Player";

    [Header("Path finding")]
    [SerializeField] float pathfindStep = 0.2f;

    [Header("UI")]
    [SerializeField] Sprite roamIcon;
    [SerializeField] Sprite chaseIcon;
    
    StateMachine<AIRoaming> _fsm;
    State<AIRoaming> _roamingState;
    State<AIRoaming> _chasingState;
    
    AStar _pathfind;
    Transform _chaseTarget;
    Canvas _canvas;
    Image _statusIcon;
    
    bool _canChangeState;
    
    public float RoamDelay => roamDelay;
    public float RoamRadius => roamRadius;
    public float PathfindStep => pathfindStep;
    public float RoamSpeed => roamSpeed;
    public float ChaseSpeed => chaseSpeed;
    public Sprite RoamIcon => roamIcon;
    public Sprite ChaseIcon => chaseIcon;

    public Transform ChaseTarget => _chaseTarget;
    
    public bool IsDetectedTarget => Vector2.Distance(_chaseTarget.position, transform.position) <= visibilityRange;
    public bool TargetEscaped => Vector2.Distance(_chaseTarget.position, transform.position) > endChaseRadius;
    public bool CanChangeState => _canChangeState;

    void Awake()
    {
        _canvas = transform.Find("Canvas").GetComponent<Canvas>();
        _statusIcon = _canvas.transform.Find("Icon_Status").GetComponent<Image>();
        _canChangeState = true;
        _pathfind = new AStar(Vector3.zero, new Vector2(50, 50), pathfindStep);
        MakeFsm();
    }

    void MakeFsm()
    {
        _roamingState = new RoamingState(this);
        _chasingState = new ChasingState(this);
        _fsm = new StateMachine<AIRoaming>(_roamingState);
    }

    void Start()
    {
        _fsm.Start();
        _chaseTarget = GameObject.FindWithTag(chaseForTag).transform;
    }

    void Update()
    {
        ShowDebug();
        _fsm.Update();
    }

    void SwitchState(State<AIRoaming> state)
    {
        StartCoroutine(TransitionDelayRoutine());
        _fsm.ChangeState(state);
    }

    public void Chase()
    {
        SwitchState(_chasingState);
    }

    public void Roam()
    {
        SwitchState(_roamingState);
    }

    public List<Vector3> BuildPath(Vector2 target)
    {
        return _pathfind.FindPath(transform.position, target);
    }

    public Vector2 GetNewRoamPoint(Vector2 center)
    {
        return center + Random.insideUnitCircle * roamRadius;
    }

    public void MoveToPoint(Vector3 point, float speed)
    {
        Vector3 position = transform.position;
        position += speed * (point - position).normalized * Time.deltaTime;
        transform.position = position;
    }

    public void SetStatusIcon(Sprite icon)
    {
        _statusIcon.sprite = icon;
    }
    
    IEnumerator TransitionDelayRoutine()
    {
        _canChangeState = false;
        yield return new WaitForSeconds(1);
        _canChangeState = true;
    }

    void ShowDebug()
    {
        Helpers.DebugDrawCircle(transform.position, visibilityRange, 30, Color.red);
        Helpers.DebugDrawCircle(transform.position, endChaseRadius, 30, Color.magenta);
    }
}