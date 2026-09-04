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
    [SerializeField] private int extraRotations = 2;

    [Header("Win Effect")]
    [SerializeField] private float winScale = 1.2f;
    [SerializeField] private float pulseSpeed = 5f;

    private RectTransform[] symbols;
    private SymbolView[] symbolViews;

    private bool isSpinning;
    private Coroutine winCoroutine;

    public bool IsSpinning => isSpinning;

    private const float SymbolSpacing = 96f;
    private const float TopPosition = 96f;
    private const float BottomPosition = -672f;
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

    public IEnumerator SpinToResult(SymbolType targetSymbol)
    {
        if (isSpinning)
            yield break;

        StopWinEffect();

        isSpinning = true;

        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            MoveSymbols(spinSpeed * Time.deltaTime);

            yield return null;
        }

        RectTransform target = FindTarget(targetSymbol);

        if (target != null)
        {
            float currentY = target.anchoredPosition.y;

            float distanceToCenter;

            if (currentY <= 0f)
                distanceToCenter = -currentY;
            else
                distanceToCenter = ReelHeight - currentY;

            float totalDistance =
                distanceToCenter +
                ReelHeight * extraRotations;

            yield return StartCoroutine(
                SmoothStop(totalDistance)
            );

            MoveAllSymbols(-target.anchoredPosition.y);

            ArrangeReel(target);
        }

        isSpinning = false;

        Debug.Log("Reel stopped on: " + targetSymbol);
    }

    private void MoveSymbols(float movement)
    {
        foreach (RectTransform symbol in symbols)
        {
            Vector2 position = symbol.anchoredPosition;

            position.y += movement;

            while (position.y >= SymbolSpacing)
                position.y -= ReelHeight;

            while (position.y < BottomPosition)
                position.y += ReelHeight;

            symbol.anchoredPosition = position;
        }
    }

    private RectTransform FindTarget(SymbolType targetSymbol)
    {
        foreach (SymbolView symbolView in symbolViews)
        {
            if (symbolView == null)
                continue;

            if (symbolView.Type == targetSymbol)
                return symbolView.GetComponent<RectTransform>();
        }

        return null;
    }

    private IEnumerator SmoothStop(float totalDistance)
    {
        float elapsed = 0f;
        float previousDistance = 0f;

        while (elapsed < stopDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / stopDuration
            );

            float smoothT = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            float currentDistance =
                totalDistance * smoothT;

            float movement =
                currentDistance - previousDistance;

            MoveAllSymbols(movement);

            previousDistance = currentDistance;

            yield return null;
        }

        float remaining =
            totalDistance - previousDistance;

        if (Mathf.Abs(remaining) > 0.001f)
            MoveAllSymbols(remaining);
    }

    private void ArrangeReel(RectTransform target)
    {
        int targetIndex = -1;

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] == target)
            {
                targetIndex = i;
                break;
            }
        }

        if (targetIndex == -1)
            return;

        for (int i = 0; i < symbols.Length; i++)
        {
            int relativeIndex = i - targetIndex;

            float newY =
                -relativeIndex * SymbolSpacing;

            while (newY >= SymbolSpacing)
                newY -= ReelHeight;

            while (newY < BottomPosition)
                newY += ReelHeight;

            symbols[i].anchoredPosition =
                new Vector2(
                    symbols[i].anchoredPosition.x,
                    newY
                );
        }

        target.anchoredPosition =
            new Vector2(
                target.anchoredPosition.x,
                0f
            );
    }

    private void MoveAllSymbols(float movement)
    {
        foreach (RectTransform symbol in symbols)
        {
            Vector2 position = symbol.anchoredPosition;

            position.y += movement;

            while (position.y >= SymbolSpacing)
                position.y -= ReelHeight;

            while (position.y < BottomPosition)
                position.y += ReelHeight;

            symbol.anchoredPosition = position;
        }
    }

    // ==========================================
    // WIN EFFECT
    // ==========================================

    public void PlayWinEffect()
    {
        StopWinEffect();

        RectTransform middleSymbol = GetMiddleSymbol();

        if (middleSymbol != null)
        {
            winCoroutine = StartCoroutine(
                WinPulse(middleSymbol)
            );
        }
    }

    private RectTransform GetMiddleSymbol()
    {
        foreach (RectTransform symbol in symbols)
        {
            if (Mathf.Abs(symbol.anchoredPosition.y) < 0.1f)
                return symbol;
        }

        return null;
    }

    private IEnumerator WinPulse(RectTransform symbol)
    {
        Vector3 originalScale = symbol.localScale;

        while (true)
        {
            float value =
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            float scale =
                Mathf.Lerp(1f, winScale, value);

            symbol.localScale =
                originalScale * scale;

            yield return null;
        }
    }

    public void StopWinEffect()
    {
        if (winCoroutine != null)
        {
            StopCoroutine(winCoroutine);
            winCoroutine = null;
        }

        if (symbols == null)
            return;

        foreach (RectTransform symbol in symbols)
        {
            symbol.localScale = Vector3.one;
        }
    }
}