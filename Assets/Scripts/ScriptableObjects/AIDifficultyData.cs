using UnityEngine;

[CreateAssetMenu(fileName = "AIDifficultyData", menuName = "Pisti/AI Difficulty Data")]
public class AIDifficultyData : ScriptableObject
{
    public AILevel difficultyLevel;
    
    [Range(0, 100)]
    public int memoryCapability = 50;  // How well AI remembers played cards
    
    [Range(0, 100)]
    public int strategicThinking = 50;  // How well AI plans ahead
    
    [Range(0, 100)]
    public int riskTaking = 50;  // How likely AI is to take risks
    
    [Range(0, 100)]
    public int cardValueAwareness = 50;  // How well AI prioritizes high-value cards
    
    [Tooltip("Delay between AI moves to simulate thinking (seconds)")]
    [Range(0.1f, 2.0f)]
    public float playDelay = 0.5f;
} 