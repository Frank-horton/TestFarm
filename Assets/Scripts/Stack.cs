using UnityEngine;
using DG.Tweening;
public class Stack : MonoBehaviour
{
    public void SetNewPos(float duration, Vector3 followingLocalPos, bool destroyOnCpmplete = false)
    {
        transform.DOLocalJump(followingLocalPos, 1, 1, duration).OnComplete(() => 
        {
            if (destroyOnCpmplete) Destroy(gameObject); 
        });
        GetComponent<Collider>().enabled = false;
    }
}
