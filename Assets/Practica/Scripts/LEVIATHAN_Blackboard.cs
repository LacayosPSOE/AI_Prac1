using UnityEngine;

public class LEVIATHAN_Blackboard : MonoBehaviour
{
	[Header("Times")]
	public float WanderTime = 0f;
	public float WanderMaxTime = 8f;
	public float eatMaxTimer = 1.5f;
	public float maxStamina = 6f;
	public float maxGuardTimer = 5f;

	[Header("Distances")]
	public float eatRadius = 1.5f;
	public float playerChaseRadius = 3f;
	public float homeArrivedRadius = 1.5f;
	public float screenBoundDistance = 20f;
	public float screenSafeBounds = 5f;

	[Header("Speeds")]
	public float speed = 2f;
	public float guardingSpeed = 4f;
	public float huntingSpeed = 4f;

	public GameObject Home;
	public GameObject Player;
}