using UnityEngine;
using System.Collections;

public class AttackableCharacter : Character
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  [HideInInspector] public  float m_health;
  [HideInInspector] public  float m_stamina;
  
  public  float     m_maxHealth                = 100f;
  public  float     m_maxStamina               = 100f;

  public  float     m_damageAnimLimit          = 6f;
  public  float     m_relativeMidpointY        = 1f;
  public  float     m_damageCooldownInSeconds  = 1f;

  public bool isDead {get {return m_health <= 0f;}}

  //private float m_latestInjuryTime         = 0f;

  private Shield m_shield;    // current shield, null if none.


  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Start () 
  {
    base.Start();

    m_health  = m_maxHealth;
    m_stamina = m_maxStamina;

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
      m_hitForceToApply = new Vector2 (forceOutput, 0f);
    else
      m_hitForceToApply = new Vector2 (-forceOutput, 0f);

    if (isDead)
    {
      weapon.OnHit (AttackWeapon.HitType.Unprotected);

      SetFacingDirection (hitFromLeft);
      m_characterAnims.Death ();
    }
    else if (damageOutput >= m_damageAnimLimit)
    {
      weapon.OnHit (AttackWeapon.HitType.Unprotected);
      
      if (highHit)
        m_characterAnims.DamageHigh ();
      else
        m_characterAnims.DamageLow ();
    }
    else // Not much damage, typically shield protection
    {
      weapon.OnHit (AttackWeapon.HitType.Protected);

      logger.Debug ("Abort swing");
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

    AttackWeapon attackWeapon = otherHitObject.GetComponent<AttackWeapon> ();
    attackWeapon.m_collider.enabled = false; // Disable collider to avoid awkward physics

    bool highHit      = contact.point.y >= transform.position.y + m_relativeMidpointY;
    bool hitFromLeft  = attackWeapon.m_character.transform.position.x < transform.position.x;
    
    float damageOutput, forceOutput;
    calculateOutput (attackWeapon.m_damage, attackWeapon.m_force, highHit, hitFromLeft, 
                     out damageOutput, out forceOutput);

    GameObject myHitObject = contact.otherCollider.gameObject;
    logger.Debug (myHitObject.name  + " (y:" + myHitObject.transform.position.y + " injured by " + otherHitObject.name + " at y:" + contact.point.y +
                  ". damageOutput: " + damageOutput + " forceOutput: " + forceOutput);
    
    //UnityEditor.EditorApplication.isPaused = true;
    inflictInjury (damageOutput, forceOutput, hitFromLeft, highHit, attackWeapon);
  }
}
