using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class LerpMove : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] bool flipX = true;
    
    public bool IsMoving { get; set; }

    public event Action<Vector3> ReachedPointEvent;

    public void MoveToPoint(Vector2 position)
    {
        TurnSprite(transform.position, position);
        StartCoroutine(MoveRoutine(position, moveSpeed));
    }

    public IEnumerator MoveToPointRoutine(Vector2 position)
    {
        TurnSprite(transform.position, position);
        yield return StartCoroutine(MoveRoutine(position, moveSpeed));
    }

    public void MoveAlongPoints(List<Vector3> path)
    { 
        TurnSprite(transform.position, path.Last());
        StartCoroutine(MoveAlongPointsRoutine(path));
    }

    public IEnumerator MoveAlongPointsRoutine(List<Vector3> path)
    {
        foreach (Vector3 point in path)
        {
            yield return StartCoroutine(MoveRoutine(point, moveSpeed));
            ReachedPointEvent?.Invoke(point);
        }
    }

    IEnumerator MoveRoutine(Vector2 target, float speed)
    {
        Vector3 pos = transform.position;
        var startPosition = new Vector2(pos.x, pos.y);
        var distance = (target - startPosition).magnitude;
        var time = distance / speed;
        var elapsedTime = 0f;
        IsMoving = true;

        while (elapsedTime < time)
        {
            var t = Mathf.Clamp(elapsedTime / time, 0f, 1f);
            transform.position = Vector3.Lerp(startPosition, target, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = target;
        IsMoving = false;
    }

    void TurnSprite(Vector2 from, Vector2 to)
    {
        if (flipX)
        {
            Vector3 scale = transform.localScale;
            var sign = Math.Abs(to.x - from.x) > 0.01 ? Math.Sign(to.x - from.x) : Math.Sign(scale.x);
            transform.localScale = new Vector3(Math.Abs(scale.x) * sign, scale.y, scale.z);
        }
    }
}