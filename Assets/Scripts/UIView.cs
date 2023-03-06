using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening; 

public class UIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stackCountText;
    [SerializeField] private Image _stackImagePrefab;
    [SerializeField] private Image _stackImage;

    [SerializeField] private TextMeshProUGUI _coinCountText;
    [SerializeField] private Image _coinImagePrefab;
    [SerializeField] private Image _coinImage;

    public void RenderCoinsCount(bool animationUI = false, Transform startTransform = null)
    {
        if (!animationUI) { _coinCountText.text = Repository.cointCount.ToString(); return; }
        AnimationView(_coinCountText, Repository.cointCount.ToString(), startTransform, _coinImage, _coinImagePrefab);
    }
    public void RenderStacksCount(bool animationUI = false, Transform startTransform = null)
    {
        if (!animationUI) { _stackCountText.text = Repository.stackCount.ToString() + " / " + Repository.maxCountStack; return; }
        AnimationView(_stackCountText, Repository.stackCount.ToString() + " / " + Repository.maxCountStack, startTransform, _stackImage, _stackImagePrefab);
    }
    private void AnimationView(TextMeshProUGUI text, string valueText, Transform startTransform, Image startImage, Image instPrefab)
    {
        Image newStackCell = Instantiate(instPrefab, startImage.transform.parent);
        newStackCell.transform.position = Camera.main.WorldToScreenPoint(startTransform.position);
        newStackCell.transform.DOMove(startImage.transform.position, 1)
            .OnComplete(() =>
            {
                Vector3 scale = new Vector3(0.1f, 0.1f, 0);
                text.transform.DOShakeScale(1f, scale).OnComplete(() => text.transform.DOScale(Vector2.one, 0.5f));
                text.text = valueText;
                Destroy(newStackCell.gameObject);
            });
    }
}
