using UnityEngine;

public class FISH_blackboard : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject leviathan;
    
    [Header("Distances")]
    public float dangerousDistanceFromMonster = 5.0f;
    public float safeDistanceFromMonster = 15.0f;

    [Header("Speeds")]
    public float speed = 5.0f;
    public float fleeSpeed = 10.0f;
}
