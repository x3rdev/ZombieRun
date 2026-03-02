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
    [SerializeField] private Color positiveColor = new Color(0, 0, 1, 0.5F);
    [SerializeField] private Color negativeColor = new Color(1, 0, 0, 0.5F);

    [Header("Movement")]
    
    [SerializeField] public float speed;

    private bool hasBeenTriggered = false;

    private void OnStart()
    {
      Destroy(gameObject, 5);
    }

    private void Update()
    {
      transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
    private void OnValidate()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (gateText == null || gateRenderer == null) return;

        // 1. Set the Text based on the enum
        switch (operation)
        {
            case ModifierType.Add: gateText.text = "+" + value; break;
            case ModifierType.Sub: gateText.text = "-" + value; break;
            case ModifierType.Mul: gateText.text = "×" +value; break;
            case ModifierType.Div: gateText.text = "÷" + value; break;
        }

        // 2. Set the Color safely using MaterialPropertyBlock
        // This prevents the "Leaking Material" error
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        gateRenderer.GetPropertyBlock(propBlock);
        
        Color targetColor = (operation == ModifierType.Add || operation == ModifierType.Mul) 
                            ? positiveColor : negativeColor;

        propBlock.SetColor("_BaseColor", targetColor); // URP Standard
        propBlock.SetColor("_Color", targetColor);     // Legacy Standard
        gateRenderer.SetPropertyBlock(propBlock);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if it's a soldier and if we haven't hit a wall yet
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            hasBeenTriggered = true; // Lock the gate
            

            // 3. Visual feedback: Disable the gate or play an effect
            gameObject.SetActive(false); 
        }
    }
}
