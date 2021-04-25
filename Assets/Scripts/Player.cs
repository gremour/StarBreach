using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Tooltip("Delay between subsequent blaster shots")]
    [SerializeField] float blasterDelay = 0.35f;

    [Tooltip("Blaster energy consumption per shot")]
    [SerializeField] float blasterConsumption = 15f;

    [Tooltip("Distance from ship center to side for projectile spawn point")]
    [SerializeField] float blasterSideMount = 0.8f;

    [Tooltip("Distance from ship center to front for projectile spawn point")]
    [SerializeField] float blasterFrontMount = 1f;

    [Tooltip("Energy recharge rate, percent per second")]
    [SerializeField] float energyRecharge = 20f;

    [Tooltip("Active shield energy consumption, percent per second")]
    [SerializeField] float shieldConsumption = 50f;

    [Tooltip("Hull integrity points (player only lose them by 1)")]
    [SerializeField] int maxIntegrity = 3;

    [Tooltip("Invulnerability window when take damage, seconds")]
    [SerializeField] float invulWindow = 1;

    [Tooltip("Distance ship is moving per second in world coords")]
    [SerializeField] float maneurability = 10;

    [Tooltip("Maximum rotation angle around Z axis when moving sideways")]
    [SerializeField] float maxRotationAngle = 30;

    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] float rotationSpeed = 180;

    [Tooltip("Blaster projectile prefab")]
    [SerializeField] Projectile projectilePrefab;

    [Tooltip("Shield color when player activates it")]
    [SerializeField] Color activeShieldColor = new Color(0.3f, 0.6f, 1f, 1f);

    [Tooltip("invulnerability shield color")]
    [SerializeField] Color invulShieldColor = new Color(1f, 0.5f, 0f, 1f);

    // Game reference
    Game game;
    // Heads up display reference
    HUD hud;
    // Shield animator reference
    Animator shieldAnim;
    // Shield sprite renderer reference
    SpriteRenderer shieldSprite;
    // Explosion animator reference
    Animator explosionAnim;

    // Current hull integrity (player only lose them by 1)
    int integrity;
    // Time ship is still invulnerable after hit
    float invulTime;
    // Player is actively shielding
    bool shielding;

    // Current ship energy (0 to 1)
    float energy = 1;

    // Remaining cooldown on blaster
    float blasterCooldown;

    // Target ship rotation when moving
    float targetRotationAngle;

    // Start is called before the first frame update
    void Start()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        hud = GameObject.Find("HUD").GetComponent<HUD>();
        shieldSprite = transform.Find("Shield").GetComponent<SpriteRenderer>();
        shieldAnim = transform.Find("Shield").GetComponent<Animator>();
        explosionAnim = transform.Find("Explosion").GetComponent<Animator>();
        integrity = maxIntegrity;
        hud.SetPlayerHull(integrity);
    }

    // Update is called once per frame
    void Update()
    {
        if (game.IsPaused())
        {
            return;
        }
        ProcessCooldowns();
        ProcessInput();
        UpdateRotation();
        hud.SetEnergy(energy);
    }

    void OnCollisionEnter(Collision other) {
        var projectileCollision = other.gameObject.tag == "Projectile";
        var enemyCollision = other.gameObject.tag == "Enemy";
        if (projectileCollision)
        {
            Destroy(other.gameObject, 0.01f);
        }
        if (projectileCollision || enemyCollision) 
        {
            TakeDamage(1);
        }
    }

    void TakeDamage(int amount)
    {
        if (invulTime > 0 || shielding)
        {
            return;
        }
        integrity -= amount;
        if (integrity < 0)
        {
            integrity = 0;
        }
        hud.SetPlayerHull(integrity);
        if (integrity == 0)
        {
            Terminate(true);
        } else {
            invulTime = invulWindow;
            shieldSprite.color = invulShieldColor;
            shieldAnim.Play("ShieldActive");
        }
    }

    void Terminate(bool explode = false)
    {
        Destroy(gameObject, 1f);
        explosionAnim.Play("Explode");
        game.End();
    }

    // Process ship cooldowns
    void ProcessCooldowns()
    {
        // Recharge energy
        energy += Time.deltaTime * energyRecharge / 100f;
        if (energy > 1)
        {
            energy = 1;
        }

        // Recharge blaster
        blasterCooldown -= Time.deltaTime;
        if (blasterCooldown < 0 ) {
            blasterCooldown = 0;
        }

        // Process invulnerability
        var invul = invulTime > 0;
        if (invul)
        {
            invulTime -= Time.deltaTime;
        }

        // Stop invulnerability
        if (invul && invulTime <= 0)
        {
            invulTime = 0;
            if (shielding)
            {
                shieldSprite.color = activeShieldColor;
            }
            else
            {
                shieldAnim.Play("ShieldFaded");
            }
        }

        // Process shielding energy consumption
        var cost = shieldConsumption * Time.deltaTime / 100f;
        if (energy <= cost)
        {
            Shielding(false);
        }
        else
        if (!invul && shielding)
        {
            energy -= shieldConsumption * Time.deltaTime / 100f;
        }
    }

    // Process player input
    void ProcessInput()
    {
        var movingForward = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        var movingBack = Input.GetKey(KeyCode.S)|| Input.GetKey(KeyCode.DownArrow);
        var movingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        var movingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        // Move the ship
        if (movingForward)
        {
            Move(Game.dirForward * maneurability * Time.deltaTime);
        }
        if (movingBack)
        {
            Move(Game.dirBack * maneurability * Time.deltaTime);
        }
        if (movingLeft)
        {
            Move(Game.dirLeft * maneurability * Time.deltaTime);
        }
        if (movingRight)
        {
            Move(Game.dirRight * maneurability * Time.deltaTime);
        }
        LimitPositionToBounds();

        // Set target rotation angle
        if (movingLeft != movingRight)
        {
            if (movingLeft) {
                targetRotationAngle = maxRotationAngle;
            } 
            else if (movingRight) {
                targetRotationAngle = -maxRotationAngle;
            }
        } else {
            targetRotationAngle = 0;
        }

        var shooting = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space);
        if (shooting)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Shielding(true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Shielding(false);
        }
    }

    void Shoot()
    {
        var cost = blasterConsumption / 100f;
        if (blasterCooldown > 0f || energy < cost)
        {
            return;
        }
        energy -= cost;
        blasterCooldown = blasterDelay;

        SpawnProjectile(Game.dirForward * blasterFrontMount + Game.dirLeft * blasterSideMount);
        SpawnProjectile(Game.dirForward * blasterFrontMount + Game.dirRight * blasterSideMount);
    }

    // Spaw projectile at delta position relative to player position
    void SpawnProjectile(Vector3 delta)
    {
        var p = Instantiate<Projectile>(
            projectilePrefab, 
            transform.position +delta, 
            Quaternion.identity);
        p.isPlayerProjectile = true;
    }

    // Update ship rotation due to movement
    void UpdateRotation()
    {
        var rot = transform.rotation.eulerAngles.z;
        if (rot >= 180) {
            rot = rot - 360;
        }
        var rotDiff = Math.Abs(targetRotationAngle - rot);
        if (rotDiff > 0.1)
        {
            float dir = targetRotationAngle > rot? 1 : -1;
            var delta = rotationSpeed * Time.deltaTime;
            if (delta > rotDiff) 
            {
                delta = rotDiff;
            }
            transform.rotation = Quaternion.Euler(0, 0, rot + dir * delta);
        }
        // Fix random Y coord drift
        var pos = transform.position;
        pos.y = 0;
        transform.position = pos;
    }

    // Move ship in direction
    void Move(Vector3 dir) 
    {
        transform.position += dir;
    }

    // Keep ship in game field
    void LimitPositionToBounds()
    {
        var pos = transform.position;
        if (pos.x < game.boundsMin.x)
        {
            pos.x = game.boundsMin.x;
        }
        else if(pos.x > game.boundsMax.x)
        {
            pos.x = game.boundsMax.x;
        }

        if (pos.z < game.boundsMin.z)
        {
            pos.z = game.boundsMin.z;
        }
        else if (pos.z > game.boundsMax.z)
        {
            pos.z = game.boundsMax.z;
        }

        if (pos != transform.position)
        {
            transform.position = pos;
        }
    }

    void Shielding(bool start)
    {
        if (start)
        {
            shielding = true;
            if (invulTime <= 0)
            {
                shieldSprite.color = activeShieldColor;
                shieldAnim.Play("ShieldActive");
            }
        } 
        else
        {
            shielding = false;
            if (invulTime <= 0)
            {
                shieldAnim.Play("ShieldFaded");
            }
        }
    }
}
