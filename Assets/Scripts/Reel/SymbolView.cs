using UnityEngine;

public class SymbolView : MonoBehaviour
{
    [SerializeField] private SymbolType symbolType;

    public SymbolType Type => symbolType;
}