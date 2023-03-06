using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    private List<Stack> _stacksList = new List<Stack>();
    [SerializeField] private Transform _firstStackTransform;
    [SerializeField] private float _distBetweenStacks = 0.1f;
    [SerializeField] private float _durationStack = 0.5f;

    [SerializeField] private UIView _uiView;

    public void AddStack(Stack stack)
    {
        if (IsFull()) return; 

        stack.transform.parent = _firstStackTransform.parent;
        stack.transform.rotation = _firstStackTransform.rotation;
        stack.SetNewPos(_durationStack, NextStackPos());
        _stacksList.Add(stack);

        Repository.stackCount++;

        _uiView.RenderStacksCount(true, transform); 
    }
    public IEnumerator SoldStack(Barn barn)
    {
        while (true)
        {
            if (_stacksList.Count == 0) yield break;

            Stack stack = _stacksList[_stacksList.Count - 1];
            stack.transform.parent = barn.transform;
            stack.SetNewPos(_durationStack, barn.CollectingStackTransform().localPosition, true);
            _stacksList.Remove(stack);

            Repository.cointCount += barn.PriceStuck();
            Repository.stackCount--; 

            _uiView.RenderStacksCount();
            _uiView.RenderCoinsCount(true, barn.transform); 

            yield return new WaitForSeconds(0.1f);
        }
    }
    private Vector3 NextStackPos()
    {
        return _firstStackTransform.localPosition + Vector3.up * (_distBetweenStacks * _stacksList.Count);
    }
    public bool IsFull()
    {
        if (_stacksList.Count >= Repository.maxCountStack) return true;
        else return false;
    }
}
