using System.Collections;
using UnityEngine;

public class ReelController : MonoBehaviour
{
    [SerializeField] private RectTransform symbolContainer;

    [Header("Spin")]
    [SerializeField] private float spinSpeed = 700f;
    [SerializeField] private float spinDuration = 2f;

    [Header("Stop")]
    [SerializeField] private float stopDuration = 1f;

    private RectTransform[] symbols;
    private SymbolView[] symbolViews;

    private bool isSpinning;

    private const float TopPosition = 96f;
    private const float ReelHeight = 768f;

    private void Awake()
    {
        CacheSymbols();
    }

    private void CacheSymbols()
    {
        int count = symbolContainer.childCount;

        symbols = new RectTransform[count];
        symbolViews = new SymbolView[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = symbolContainer.GetChild(i);

            symbols[i] = child.GetComponent<RectTransform>();
            symbolViews[i] = child.GetComponent<SymbolView>();
        }
    }

    public void StartSpin()
    {
        if (isSpinning)
            return;

        SymbolType result = GenerateRandomResult();

        Debug.Log("RNG Result: " + result);

        StartCoroutine(SpinRoutine(result));
    }

    private SymbolType GenerateRandomResult()
    {
        int randomIndex = Random.Range(0, 4);

        return (SymbolType)randomIndex;
    }

    private IEnumerator SpinRoutine(SymbolType target)
    {
        isSpinning = true;

        // -------------------------
        // SPIN
        // -------------------------

        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            MoveSymbols(spinSpeed);

            yield return null;
        }

        // -------------------------
        // FIND TARGET
        // -------------------------

        RectTransform targetSymbol = FindTarget(target);

        if (targetSymbol != null)
        {
            yield return StartCoroutine(
                MoveToCenter(targetSymbol)
            );
        }

        isSpinning = false;

        Debug.Log("Reel stopped on: " + target);
    }

    private void MoveSymbols(float speed)
    {
        float movement =
            speed * Time.deltaTime;

        foreach (RectTransform symbol in symbols)
        {
            Vector2 position =
                symbol.anchoredPosition;

            position.y += movement;

            if (position.y >= TopPosition)
            {
                position.y -= ReelHeight;
            }

            symbol.anchoredPosition = position;
        }
    }

    private RectTransform FindTarget(SymbolType target)
    {
        foreach (SymbolView symbolView in symbolViews)
        {
            if (symbolView.Type == target)
            {
                return symbolView.GetComponent<RectTransform>();
            }
        }

        return null;
    }

    private IEnumerator MoveToCenter(
        RectTransform targetSymbol)
    {
        float startOffset =
            -targetSymbol.anchoredPosition.y;

        float elapsed = 0f;

        while (elapsed < stopDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / stopDuration
                );

            // Smooth deceleration.
            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            float currentOffset =
                Mathf.Lerp(
                    0f,
                    startOffset,
                    smoothT
                );

            float previousT =
                Mathf.Clamp01(
                    (elapsed - Time.deltaTime)
                    / stopDuration
                );

            float previousSmoothT =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    previousT
                );

            float movement =
                currentOffset -
                Mathf.Lerp(
                    0f,
                    startOffset,
                    previousSmoothT
                );

            MoveAllSymbols(movement);

            yield return null;
        }

        // Make absolutely sure the target
        // is exactly in the center.
        float finalOffset =
            -targetSymbol.anchoredPosition.y;

        MoveAllSymbols(finalOffset);
    }

    private void MoveAllSymbols(float movement)
    {
        foreach (RectTransform symbol in symbols)
        {
            Vector2 position =
                symbol.anchoredPosition;

            position.y += movement;

            symbol.anchoredPosition = position;
        }
    }
}