using UnityEngine;

public class SlotRNG : MonoBehaviour
{
    public SymbolType GenerateSymbol()
    {
        return (SymbolType)Random.Range(0, 4);
    }
}