using UnityEngine;

public class SlotRNG : MonoBehaviour
{
    public SymbolType GenerateSymbol()
    {
        int randomIndex = Random.Range(0, 4);

        return (SymbolType)randomIndex;
    }
}