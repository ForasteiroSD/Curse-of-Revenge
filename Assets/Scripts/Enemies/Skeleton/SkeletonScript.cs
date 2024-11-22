using System.Collections;
using UnityEngine;
using Utils;

public class SkeletonScript : EnemiesScript
{
    BoxCollider2D _collider;

    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        SFXManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        _collider = GetComponent<BoxCollider2D>();
        indexSFX = 6;
        _player = FindFirstObjectByType<Adventurer>().transform;
    }
    
    protected override Vector3 GetTextPosition()
    {
        Vector3 textPosition = _collider.bounds.center + new Vector3(0, _collider.bounds.extents.y + 0.5f, 0);
        return textPosition;
    }
}