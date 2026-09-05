using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotGameManager : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private ReelController reel1;
    [SerializeField] private ReelController reel2;
    [SerializeField] private ReelController reel3;

    [Header("RNG")]
    [SerializeField] private SlotRNG slotRNG;

    [Header("Audio")]
    [SerializeField] private SlotAudioManager audioManager;

    [Header("Lever")]
    [SerializeField] private LeverController leverController;

    [Header("UI")]
    [SerializeField] private Button spinButton;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text payoutText;
    [SerializeField] private TMP_Text balanceText;

    [Header("Game Settings")]
    [SerializeField] private int startingCoins = 100;
    [SerializeField] private int spinCost = 10;

    [Header("Payouts")]
    [SerializeField] private int sevenPayout = 100;
    [SerializeField] private int cherryPayout = 50;
    [SerializeField] private int bellPayout = 30;
    [SerializeField] private int barPayout = 20;

    private int coins;
    private bool isSpinning;

    private SymbolType result1;
    private SymbolType result2;
    private SymbolType result3;

    private void Start()
    {
        coins = startingCoins;

        spinButton.onClick.AddListener(Spin);

        resultText.text = "READY!";
        payoutText.text = "";

        UpdateUI();
    }

    public void Spin()
    {
        if (isSpinning)
            return;

        if (coins < spinCost)
        {
            resultText.text = "NOT ENOUGH COINS!";
            payoutText.text = "";

            if (audioManager != null)
            {
                audioManager.PlayLoseSound();
            }

            return;
        }

        StartCoroutine(SpinAllReels());
    }

    private IEnumerator SpinAllReels()
    {
        isSpinning = true;

        spinButton.interactable = false;

        // Stop previous win effects
        reel1.StopWinEffect();
        reel2.StopWinEffect();
        reel3.StopWinEffect();

        // Play spin button sound
        if (audioManager != null)
        {
            audioManager.PlaySpinButtonSound();
        }

        // Pay for spin
        coins -= spinCost;

        UpdateUI();

        // Generate random results
        result1 = slotRNG.GenerateSymbol();
        result2 = slotRNG.GenerateSymbol();
        result3 = slotRNG.GenerateSymbol();

        Debug.Log(
            $"Results: {result1} | {result2} | {result3}"
        );

        resultText.text = "SPINNING...";
        payoutText.text = "";

        // Pull the lever
        if (leverController != null)
        {
            yield return StartCoroutine(
                leverController.PullLever()
            );
        }

        // Start reel spinning sound
        if (audioManager != null)
        {
            audioManager.PlayReelSpinSound();
        }

        // Start Reel 1
        StartCoroutine(
            reel1.SpinToResult(result1)
        );

        yield return new WaitForSeconds(0.15f);

        // Start Reel 2
        StartCoroutine(
            reel2.SpinToResult(result2)
        );

        yield return new WaitForSeconds(0.15f);

        // Start Reel 3
        StartCoroutine(
            reel3.SpinToResult(result3)
        );

        // Wait for all reels to stop
        while (
            reel1.IsSpinning ||
            reel2.IsSpinning ||
            reel3.IsSpinning
        )
        {
            yield return null;
        }

        // Stop reel spinning sound
        if (audioManager != null)
        {
            audioManager.StopReelSpinSound();
        }

        // Check result
        CheckResult();

        UpdateUI();

        isSpinning = false;

        spinButton.interactable = true;
    }

    private void CheckResult()
    {
        // JACKPOT
        if (
            result1 == result2 &&
            result2 == result3
        )
        {
            int payout = GetPayout(result1);

            coins += payout;

            resultText.text = "JACKPOT!";

            payoutText.text =
                "+" + payout + " COINS";

            reel1.PlayWinEffect();
            reel2.PlayWinEffect();
            reel3.PlayWinEffect();

            if (audioManager != null)
            {
                audioManager.PlayJackpotSound();
            }

            Debug.Log(
                $"JACKPOT! {result1} +{payout} coins"
            );

            return;
        }

        // SMALL WIN
        if (
            result1 == result2 ||
            result2 == result3 ||
            result1 == result3
        )
        {
            int payout = 10;

            coins += payout;

            resultText.text = "SMALL WIN!";

            payoutText.text =
                "+" + payout + " COINS";

            if (audioManager != null)
            {
                audioManager.PlaySmallWinSound();
            }

            Debug.Log(
                "SMALL WIN! +10 coins"
            );

            return;
        }

        // LOSS
        resultText.text = "TRY AGAIN!";

        payoutText.text = "0 COINS";

        if (audioManager != null)
        {
            audioManager.PlayLoseSound();
        }

        Debug.Log(
            $"TRY AGAIN: {result1} | " +
            $"{result2} | {result3}"
        );
    }

    private int GetPayout(SymbolType symbol)
    {
        switch (symbol)
        {
            case SymbolType.Seven:
                return sevenPayout;

            case SymbolType.Cherry:
                return cherryPayout;

            case SymbolType.Bell:
                return bellPayout;

            case SymbolType.Bar:
                return barPayout;

            default:
                return 0;
        }
    }

    private void UpdateUI()
    {
        balanceText.text =
            "COINS: " + coins;
    }
}