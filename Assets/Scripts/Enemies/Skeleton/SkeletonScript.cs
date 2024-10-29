using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

public class SkeletonScript : EnemiesScript
{

    [SerializeField] float _move = 0.5f;

    protected override void Flip()
    {
        if (_death) return;
        _horSpeed *= -1;
        transform.position = new Vector3(transform.position.x + ((Mathf.Sign(_horSpeed)) * _move), transform.position.y, transform.position.z);
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(_horSpeed), transform.localScale.y, transform.localScale.z);
    }

}