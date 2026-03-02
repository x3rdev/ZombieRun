using UnityEngine;
using TMPro;

public class MultiplierWall : MonoBehaviour
{
    public enum ModifierType
    {
        Add,
        Sub,
        Mul,
        Div
    }

    [Header("Gate Settings")]
    public ModifierType operation;
    public int value;

    [Header("Visuals")]
    [SerializeField] private TextMeshPro gateText;
    [SerializeField] private MeshRenderer gateRenderer;
    [SerializeField] private Color positiveColor = Color.blue;
    [SerializeField] private Color negativeColor = Color.red;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
