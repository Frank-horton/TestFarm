using UnityEngine;

public class WeaponMow : MonoBehaviour 
{
    [SerializeField] private int _damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        ITakeDamage iTakeDamage = other.GetComponent<ITakeDamage>();
        if (iTakeDamage != null) iTakeDamage.TakeDamage(_damage);
    }
}
