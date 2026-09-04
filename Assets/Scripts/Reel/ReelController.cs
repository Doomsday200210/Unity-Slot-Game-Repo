using UnityEngine;

public class ReelController : MonoBehaviour
{
    [Header("Reel References")]
    [SerializeField] private RectTransform symbolContainer;

    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 700f;

    [SerializeField] private float topWrapPosition = 96f;
    [SerializeField] private float bottomWrapPosition = -672f;

    private RectTransform[] symbols;

    private void Awake()
    {
        CacheSymbols();
    }

    private void Update()
    {
        SpinReel();
    }

    private void CacheSymbols()
    {
        int childCount = symbolContainer.childCount;

        symbols = new RectTransform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            symbols[i] = symbolContainer.GetChild(i)
                .GetComponent<RectTransform>();
        }
    }

    private void SpinReel()
    {
        foreach (RectTransform symbol in symbols)
        {
            symbol.anchoredPosition += Vector2.up * spinSpeed * Time.deltaTime;

            if (symbol.anchoredPosition.y >= topWrapPosition)
            {
                Vector2 position = symbol.anchoredPosition;

                position.y = bottomWrapPosition;

                symbol.anchoredPosition = position;
            }
        }
    }
}