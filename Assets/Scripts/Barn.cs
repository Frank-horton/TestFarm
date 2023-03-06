using UnityEngine;

public class Barn : MonoBehaviour
{
    [SerializeField] private Transform _collectingStackTransform;
    [SerializeField] private int _pricePerStack = 15;

    private const string _playerTag = "Player"; 
    public Transform CollectingStackTransform()
    {
        return _collectingStackTransform;
    }
    public int PriceStuck()
    {
        return _pricePerStack; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            StackManager SB = other.GetComponent<StackManager>();
            SB.StartCoroutine(SB.SoldStack(this));
        }
    }
}
