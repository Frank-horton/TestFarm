using UnityEngine;
using DG.Tweening;
public class Grass : MonoBehaviour, ITakeDamage
{
    [SerializeField] private GameObject _instObject;
    [SerializeField] private int _countGrass;

    private void Awake()
    {
        _countGrass = transform.childCount - 1;
    }
    public void TakeDamage(int damage)
    {
        transform.GetChild(_countGrass).DOScale(Vector3.one * 0, 1); 
        _countGrass -= damage;
        if (_countGrass < 0) GrassEmpty();
        InstStack();
    }
    private void GrassEmpty()
    {
        GetComponent<Collider>().enabled = false;
        Invoke(nameof(ReturnGrass), 10f);
    }
    private void ReturnGrass()
    {
        _countGrass = transform.childCount - 1;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).DOScale(Vector3.one, 1);
        GetComponent<Collider>().enabled = true;
    }
    private void InstStack()
    {
        GameObject newStack = Instantiate(_instObject, transform.position, Quaternion.identity);
        float rndmHorizPos = Random.Range(-1f, 1f);
        Vector3 jumpPos = transform.position + new Vector3(rndmHorizPos, 0.1f, rndmHorizPos);
        newStack.transform.rotation = Quaternion.Euler(0, rndmHorizPos, 0); 
        newStack.transform.DOJump(jumpPos, 1, 1, 1)
            .OnComplete(() => newStack.GetComponent<Collider>().enabled = true);
    }
}
