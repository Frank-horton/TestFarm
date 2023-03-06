using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [SerializeField] protected float _moveSpeed;
    protected Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>(); 
    }
}
