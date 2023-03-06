using System.Collections;
using UnityEngine;

public class CharacterPlayer : CharacterBase, IControllabe
{
    private void Start()
    {
        StartCoroutine(MowAnimationState()); 
    }
    #region Movement
    private const string _animMovedName = "Moved";
    private Vector3 _moveVector;
    public void Move(Vector3 moveVector)
    {
        _moveVector = moveVector;
        transform.position += moveVector * _moveSpeed * Time.deltaTime;
        TurnTowardsDirection(moveVector);
        _animator.SetBool(_animMovedName, OnMoved());
    }
    private void TurnTowardsDirection(Vector3 direction)
    {
        float angle = Vector3.Angle(Vector3.forward, direction);
        if (angle > 1 || angle == 0)
        {
            Vector3 direct = Vector3.RotateTowards(transform.forward, direction, _moveSpeed, 0.0f);
            transform.rotation = Quaternion.LookRotation(direct);
        }
    }
    private bool OnMoved()
    {
        if (_moveVector != Vector3.zero) return true;
        else return false;
    }
    #endregion

    #region MowGrass
    [Space(5)]
    [Header("MowGrass")]
    private const string _animMowName = "Mowed";
    private const string _whatIsPicked = "Stack";

    [SerializeField] private float _radiusCheckSphere = 0.7f;
    [SerializeField] private float _distCheckSphere = 0.7f; 
    [SerializeField] private LayerMask _whatIsGrass;
    [SerializeField] private StackManager _stackManager;

    private bool CheckGrassAround()
    {
        if (OnMoved() || _stackManager.IsFull()) { return false; }
        Vector3 _origin = transform.position + transform.up;
        Vector3 _direction = transform.forward;
        return Physics.CheckSphere(_origin + _direction * _distCheckSphere, _radiusCheckSphere, _whatIsGrass);
    }
    private void Picked(Stack stack)
    {
        _stackManager.AddStack(stack);
    }
    private IEnumerator MowAnimationState()
    {
        while (true)
        {
            _animator.SetBool(_animMowName, CheckGrassAround());
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_whatIsPicked))
            Picked(other.GetComponent<Stack>());
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position + transform.up, transform.position + transform.up + transform.forward * _distCheckSphere);
        Gizmos.DrawWireSphere(transform.position + transform.up + transform.forward * _distCheckSphere, _radiusCheckSphere);
    }
}
