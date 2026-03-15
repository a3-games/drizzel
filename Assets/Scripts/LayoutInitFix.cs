using UnityEngine;
using UnityEngine.UI;

public class LayoutInitFix : MonoBehaviour
{
    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }
}
