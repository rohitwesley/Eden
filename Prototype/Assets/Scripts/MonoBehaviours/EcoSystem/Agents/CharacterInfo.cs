using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : AgentInfo
{
    public Slider energySlider;                                 // Reference to the UI's Energy bar.
    public Slider healthSlider;                                 // Reference to the UI's health bar.
    public AudioSource playerAudio;                             // Reference to the AudioSource component.
    public Image damageImage;                                   // Reference to an damageImage to flash on the screen on being hurt.
    public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.

    // Move data:
    bool moving;
    Coord moveFromCoord;
    Coord moveTargetCoord;
    Vector3 moveStartPos;
    Vector3 moveTargetPos;
    float moveTime;
    float moveSpeedFactor;
    float moveArcHeightFactor;
    public int currentHealth;                                   // The current health the player has.
    public int currentEnergy = 1000;                            // The current enery the player has.
    protected float hunger;
    protected float thirst;
    protected float enraged;
    protected float lifespan;
	bool isDead;                                                // Whether the player is dead.
    bool damaged;                                               // True when the player gets damaged.

	void Awake ()
    {
        // Set the initial health of the player.
        currentHealth = AgentSettings.startingHealth;
    }
    
    private void Update()
    {
        // Set the health bar's value to the current health.
        if(healthSlider != null)healthSlider.value = currentHealth;

        // If the player has just been damaged...
        if(damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            if(damageImage != null)damageImage.color = flashColour;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            if(damageImage != null)damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        // Reset the damaged flag.
        damaged = false;

    }

    public void TakeDamage (int amount)
    {
        Debug.Log(AgentSettings.AgentId +" currentHealth : " + currentHealth);
        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        currentHealth -= amount;

        // Set the health bar's value to the current health.
        healthSlider.value = currentHealth;

        // Play the hurt sound effect.
        playerAudio.Play ();


        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(currentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death ();
        }
    }

    void Death ()
    {
        // Set the death flag so this function won't be called again.
        isDead = true;

        // // Turn off any remaining shooting effects.
        // playerShooting.DisableEffects ();

        // // Tell the animator that the player is dead.
        // anim.SetTrigger ("Die");

        // // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        playerAudio.clip = AgentSettings.deathClip;
        playerAudio.Play ();

        // Turn off the movement and shooting scripts.
        // playerMovement.enabled = false;
        // playerShooting.enabled = false;
        if(AgentSettings.AgentId == AgentType.Player)
        {
            // Reload the level that is currently loaded.
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.Log(AgentSettings.AgentId + " Agent : " + AgentSettings._description + " Died");
            Destroy(this.gameObject);
            
        }
    } 

}

public class Surroundings {
    public Coord nearestWaterTile;
    public AgentInfo nearestFoodSource;
    public AgentInfo nearestCreature;
}