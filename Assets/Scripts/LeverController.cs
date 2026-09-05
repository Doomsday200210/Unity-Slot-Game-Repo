using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeverController : MonoBehaviour
{
    [Header("Lever Images")]
    [SerializeField] private Image normalLever;
    [SerializeField] private Image pulledLever;

    [Header("Animation")]
    [SerializeField] private float transitionDuration = 0.12f;
    [SerializeField] private float holdDuration = 0.12f;
    [SerializeField] private float returnDuration = 0.18f;

    private Coroutine leverCoroutine;

    private void Awake()
    {
        SetNormalState();
    }

    public IEnumerator PullLever()
    {
        if (leverCoroutine != null)
        {
            StopCoroutine(leverCoroutine);
        }

        leverCoroutine = StartCoroutine(PullAnimation());

        yield return leverCoroutine;

        leverCoroutine = null;
    }

    private IEnumerator PullAnimation()
    {
        // Make sure both images are available
        if (normalLever == null || pulledLever == null)
        {
            Debug.LogWarning("LeverController: Lever images are not assigned.");
            yield break;
        }

        // Start with normal lever
        normalLever.gameObject.SetActive(true);
        pulledLever.gameObject.SetActive(true);

        SetAlpha(normalLever, 1f);
        SetAlpha(pulledLever, 0f);

        // Smoothly switch to pulled lever
        yield return StartCoroutine(
            CrossFade(
                normalLever,
                pulledLever,
                transitionDuration
            )
        );

        // Hold pulled lever
        yield return new WaitForSeconds(holdDuration);

        // Smoothly return to normal
        yield return StartCoroutine(
            CrossFade(
                pulledLever,
                normalLever,
                returnDuration
            )
        );

        SetNormalState();
    }

    private IEnumerator CrossFade(
        Image from,
        Image to,
        float duration
    )
    {
        float elapsed = 0f;

        to.gameObject.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            SetAlpha(from, 1f - t);
            SetAlpha(to, t);

            yield return null;
        }

        SetAlpha(from, 0f);
        SetAlpha(to, 1f);

        from.gameObject.SetActive(false);
    }

    private void SetNormalState()
    {
        if (normalLever == null || pulledLever == null)
            return;

        normalLever.gameObject.SetActive(true);
        pulledLever.gameObject.SetActive(false);

        SetAlpha(normalLever, 1f);
        SetAlpha(pulledLever, 0f);
    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}