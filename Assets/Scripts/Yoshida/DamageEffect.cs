using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageEffect : MonoBehaviour
{
    public Text text;

    public void ShowDamage(int damage)
    {
        text.text = damage.ToString();
        text.rectTransform.DOLocalMove(new Vector3(0, 80, 0), 0.3f)
            .SetEase(Ease.OutElastic)
            .OnComplete(DestroyEffect);
    }

    void DestroyEffect()
    {
        text.gameObject.SetActive(false);
    }
}
