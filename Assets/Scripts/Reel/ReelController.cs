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

    private RectTransform[] symbols;
    private SymbolView[] symbolViews;

    private bool isSpinning;

    public bool IsSpinning => isSpinning;

    private const float SymbolSpacing = 96f;
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

        isSpinning = true;

        // ==========================================
        // 1. NORMAL SPIN
        // ==========================================

        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            MoveSymbols(spinSpeed * Time.deltaTime);

            yield return null;
        }

        // ==========================================
        // 2. FIND TARGET
        // ==========================================

        RectTransform target = FindTarget(targetSymbol);

        if (target != null)
        {
            // Calculate how far the target needs to travel
            // to reach the middle.
            float currentY = target.anchoredPosition.y;

            float distanceToCenter;

            if (currentY <= 0f)
            {
                distanceToCenter = -currentY;
            }
            else
            {
                distanceToCenter = ReelHeight - currentY;
            }

            float totalDistance =
                distanceToCenter +
                (ReelHeight * extraRotations);

            // ==========================================
            // 3. SMOOTH STOP
            // ==========================================

            yield return StartCoroutine(
                SmoothStop(totalDistance)
            );

            // ==========================================
            // 4. FORCE TARGET TO EXACT CENTER
            // ==========================================

            float correction =
                -target.anchoredPosition.y;

            MoveAllSymbols(correction);

            // ==========================================
            // 5. FORCE ENTIRE REEL INTO GRID
            // ==========================================

            ArrangeReel(target);
        }

        isSpinning = false;

        Debug.Log("Reel stopped on: " + targetSymbol);
    }

    // ==================================================
    // NORMAL SPIN
    // ==================================================

    private void MoveSymbols(float movement)
    {
        foreach (RectTransform symbol in symbols)
        {
            Vector2 position = symbol.anchoredPosition;

            position.y += movement;

            // Proper wrapping
            while (position.y >= SymbolSpacing)
            {
                position.y -= ReelHeight;
            }

            while (position.y < -672f)
            {
                position.y += ReelHeight;
            }

            symbol.anchoredPosition = position;
        }
    }

    // ==================================================
    // FIND TARGET
    // ==================================================

    private RectTransform FindTarget(SymbolType targetSymbol)
    {
        foreach (SymbolView symbolView in symbolViews)
        {
            if (symbolView == null)
                continue;

            if (symbolView.Type == targetSymbol)
            {
                return symbolView.GetComponent<RectTransform>();
            }
        }

        return null;
    }

    // ==================================================
    // SMOOTH STOP
    // ==================================================

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

            // Ease out
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

        // Finish remaining distance
        float remaining =
            totalDistance - previousDistance;

        if (Mathf.Abs(remaining) > 0.001f)
        {
            MoveAllSymbols(remaining);
        }
    }

    // ==================================================
    // ARRANGE COMPLETE REEL
    // ==================================================

    private void ArrangeReel(RectTransform target)
    {
        // Find the target's index
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

        /*
         * Force the target to the exact middle.
         *
         * Example:
         *
         *        Symbol above
         *             +96
         *
         *        TARGET
         *              0
         *
         *        Symbol below
         *             -96
         */

        for (int i = 0; i < symbols.Length; i++)
        {
            int relativeIndex = i - targetIndex;

            float newY =
                -relativeIndex * SymbolSpacing;

            // Keep the reel inside the 768px loop
            while (newY >= SymbolSpacing)
            {
                newY -= ReelHeight;
            }

            while (newY < -672f)
            {
                newY += ReelHeight;
            }

            symbols[i].anchoredPosition =
                new Vector2(
                    symbols[i].anchoredPosition.x,
                    newY
                );
        }

        // Final guarantee:
        // target is exactly in the middle.
        target.anchoredPosition =
            new Vector2(
                target.anchoredPosition.x,
                0f
            );
    }

    // ==================================================
    // MOVE EVERYTHING
    // ==================================================

    private void MoveAllSymbols(float movement)
    {
        foreach (RectTransform symbol in symbols)
        {
            Vector2 position = symbol.anchoredPosition;

            position.y += movement;

            while (position.y >= SymbolSpacing)
            {
                position.y -= ReelHeight;
            }

            while (position.y < -672f)
            {
                position.y += ReelHeight;
            }

            symbol.anchoredPosition = position;
        }
    }
}