using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] private WeaponMow _weaponMow;
    [SerializeField] private StackManager stackBag;  
    public void WeaponEnabled()
    {
        _weaponMow.gameObject.SetActive(true);
    }
    public void WeaponDissabled()
    {
        _weaponMow.gameObject.SetActive(false);
    }
    public void Walk()
    {
        //
    }
}
