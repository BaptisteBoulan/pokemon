using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharacterAnimator animator;
    public float speed;
    public bool IsMoving { get; private set; }
    public float OffsetY { get; private set; } = 0.3f;

    public CharacterAnimator Animator { get { return animator; } }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;
        transform.position = pos;
    }
    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver = null, bool checkForCollision = true)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;


        if (checkForCollision && !IsPathClear(targetPos))
        {
            yield break;
        }

        if (animator.IsSurfing && !Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.WaterLayer)) animator.IsSurfing = false;


        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
     
        OnMoveOver?.Invoke();

        IsMoving = false;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        var length = diff.magnitude;

        var collisionLayers = GameLayers.i.SolidObjectLayer | GameLayers.i.InteractableLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer;
        if (!animator.IsSurfing)
            collisionLayers |= GameLayers.i.WaterLayer;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, length-1, collisionLayers))
        {
            return false;
        } else {
            return true;
        }
    }

    public void HandleUpdtate()
    {
        animator.IsMoving = IsMoving;
    }

    public void LookToward(Vector3 targetPos)
    {
        var xDiff = Mathf.Round(targetPos.x - transform.position.x);
        var yDiff = Mathf.Round(targetPos.y - transform.position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
        else Debug.Log("dont look diagonally");
    }
}
