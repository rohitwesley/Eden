using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (menuName = "PluggableAI/AgentStats")]
public class AgentSettings : ScriptableObject {

	// Agent Info
    public AgentType AgentId = AgentType.Food;
	public AgentDiet diet;
    public string _name = "Default";
    public string _description = "Default";
	public Text textPlaceHolder;
    public Sprite sprite;

	public float crouchSpeed = 1f;
	public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 1.0f;         //Max Jump height
    public float jumpSpeed = 1f;
	public float glideSpeed = 5f;
	public float swingSpeed = 10f;
    [Range(0,1)]
	public float airControlPercent = 0.5f;
    public float turnSmoothTime = 0.2f;
	public float speedSmoothTime = 0.1f;
	// public float moveSpeed = 1;
	public float lookRange = 40f;
	public float lookSphereCastRadius = 1f;

	public float attackRange = 1f;
	public float attackRate = 1f;
	public float attackForce = 15f;
	public int attackDamage = 50;

	public float searchDuration = 4f;
	public float searchingTurnSpeed = 120f;

	public int startingEnergy = 100;                            // The amount of enery the player starts the game with.
    public int startingHealth = 100;                            // The amount of health the player starts the game with.
    public AudioClip deathClip;                                 // The audio clip to play when the player dies.
	public const int maxViewDistance = 10;
    
    
}

public enum AgentType {
	Player,
	TreeOfLife,
	Soul,
	Food,
	Portion,
	Path,
	Predator,
	Elephant,
	Hooks
}

public enum AgentDiet { Herbivore, Carnivore, Souls }

public enum AgentAction { 
    Idle,Patrol, Scan, Chase, Hook, Attack,
    Resting, Exploring, GoingToFood, GoingToWater, GoingToAgent, Eating, Drinking, AttackAgent }
// public enum AgentAction { Resting, Exploring, GoingToFood, GoingToWater, GoingToAgent, Eating, Drinking, AttackAgent }
