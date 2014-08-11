using UnityEngine;
using System.Collections;

public class AttackableCharacter : Character
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  public  float m_maxHealth                = 100f;
  public  float m_maxStamina               = 100f;

  [HideInInspector] public  float m_health;
  [HideInInspector] public  float m_stamina;
  
  public  float m_damageAnimLimit          = 6f;
  public  float m_relativeMidpointY        = 1f;
  public  float m_damageCooldownInSeconds  = 1f;

  public bool isDead {get {return m_health <= 0f;}}

  //private float m_latestInjuryTime         = 0f;

  private Shield m_shield;    // current shield, null if none.

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Start () 
  {
    base.Start();

    m_health  = m_maxHealth;
    m_stamina = m_maxStamina;

    m_characterAnims.m_onStateChangeDelegate += OnStateChange;

    updateCurrentShield ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void updateCurrentShield ()
  {
    m_shield = GetComponentInChildren <Shield> ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public static float absorbCalc (float input, float absorbtion)
  {
    if (absorbtion > input)
      return 0f;

    return input - absorbtion;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnStateChange (AnimationState oldState, AnimationState newState)
  {
    if (newState == AnimationState.LieDead)
    {
      logger.Debug ("============ Lies dead ============");
      Destroy (GetComponent<Rigidbody2D> ());
      GetComponent<BoxCollider2D> ().enabled    = false;
      GetComponent<CircleCollider2D> ().enabled = false;
      enabled = false; // Disable script
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void calculateOutput (float attackDamage, float attackForce, bool highHit, bool hitFromLeft, 
                               out float damageOutput, out float forceOutput)
  {
    float damageAbsorb  = 0f;
    float forceAbsorb   = 0f;
    
    // Facing towards hit?
    if (m_facingLeft == hitFromLeft)
    {
      AnimationState animState = m_characterAnims.GetState ();
      // Got hit on shield?
      if (highHit  && (animState == AnimationState.ProtectHigh) ||
          !highHit && (animState == AnimationState.ProtectLow))
      {
        damageAbsorb  = m_shield.m_damageAbsorb;
        forceAbsorb   = m_shield.m_forceAbsorb;
      }
    }
    
    // Add absorbtion from armor...
    
    damageOutput = absorbCalc (attackDamage, damageAbsorb);
    forceOutput  = absorbCalc (attackForce,  forceAbsorb);

    return;
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void inflictInjury (float damageOutput, float forceOutput, bool hitFromLeft, bool highHit, AttackWeapon weapon)
  {
    // logger.Debug ("Damage output: " + damage);

    m_health -= damageOutput;

    if (hitFromLeft)
      rigidbody2D.velocity = new Vector2 (forceOutput, rigidbody2D.velocity.y);
    else
      rigidbody2D.velocity = new Vector2 (-forceOutput, rigidbody2D.velocity.y);

    if (isDead)
    {
      SetFacingDirection (hitFromLeft);
      m_characterAnims.Death ();
    }
    else if (damageOutput >= m_damageAnimLimit)
    {
      SetFacingDirection (hitFromLeft);

      if (highHit)
        m_characterAnims.DamageHigh ();
      else
        m_characterAnims.DamageLow ();
    }
    else // Successful shield protection
    {
      weapon.m_charAnims.AbortSwing ();
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnCollisionEnter2D (Collision2D collision)
  {
    ContactPoint2D contact    = collision.contacts[0];   
    
    GameObject otherHitObject = contact.collider.gameObject;
    
    if (otherHitObject.tag != "AttackWeapon")
    {
      // logger.Debug ("No weapon hit");
      return;
    }

//    UnityEditor.EditorApplication.isPaused = true;
        
    AttackWeapon attackWeapon = otherHitObject.GetComponent<AttackWeapon> ();
    attackWeapon.m_collider.enabled = false; // Disable collider to avoid awkward physics

    bool highHit      = contact.point.y >= transform.position.y + m_relativeMidpointY;
    bool hitFromLeft  = attackWeapon.m_character.transform.position.x < transform.position.x;
    
    float damageOutput, forceOutput;
    calculateOutput (attackWeapon.m_damage, attackWeapon.m_force, highHit, hitFromLeft, 
                     out damageOutput, out forceOutput);

    if (damageOutput <= 0f && forceOutput <= 0f)
      return;

    /*
    float currentTime = Time.time;
    if (currentTime < m_latestInjuryTime + m_damageCooldownInSeconds)
      return;
    
    m_latestInjuryTime = currentTime;
    */
    
    // GameObject myHitObject    = contact.otherCollider.gameObject;
    // logger.Debug (myHitObject.name  + " (y:" + myHitObject.transform.position.y + " injured by " + otherHitObject.name + " at y:" + contact.point.y +
    //               ". damageOutput: " + damageOutput + " forceOutput: " + forceOutput);
    
    inflictInjury (damageOutput, forceOutput, hitFromLeft, highHit, attackWeapon);
  }
}
