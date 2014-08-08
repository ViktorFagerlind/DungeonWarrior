using UnityEngine;
using System.Collections;

public class AttackableCharacter : Character
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  public  float m_health                   = 100f;
  public  float m_damageAnimLimit          = 6f;
  public  float m_relativeMidpointY        = 1f;
  public  float m_damageCooldownInSeconds  = 1f;

  private float m_latestInjuryTime         = 0f;

  public static float absorbCalc (float input, float absorbtion)
  {
    if (absorbtion > input)
      return 0f;

    return input - absorbtion;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void inflictInjury (float damageOutput, float forceOutput, Vector2 contactPoint)
  {
    // logger.Debug ("Damage output: " + damage);

    m_health -= damageOutput;

    if (contactPoint.x > transform.position.x)
      rigidbody2D.AddForce (new Vector2 (-forceOutput, 0));
    else
      rigidbody2D.AddForce (new Vector2 (forceOutput, 0));

    if (m_health <= 0f)
    {
      GetComponent<BoxCollider2D> ().enabled = false;
      m_characterAnims.Death ();
      logger.Debug ("============ Died ============");
    }
    else if (damageOutput >= m_damageAnimLimit)
    {
      //UnityEditor.EditorApplication.isPaused = true;
      if (contactPoint.y >= transform.position.y + m_relativeMidpointY)
        m_characterAnims.DamageHigh ();
      else
        m_characterAnims.DamageLow ();
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnCollisionEnter2D (Collision2D collision)
  {
    ContactPoint2D contact    = collision.contacts[0];   

    GameObject myHitObject    = contact.otherCollider.gameObject;
    GameObject otherHitObject = contact.collider.gameObject;

    if (otherHitObject.tag != "AttackWeapon")
    {
      // logger.Debug ("No weapon hit");
      return;
    }

    float damageAbsorb = 0f;
    float forceAbsorb = 0f;
    if (myHitObject.tag == "Shield")
    {
      Shield shield = myHitObject.GetComponent<Shield> ();
      damageAbsorb  = shield.m_damageAbsorb;
      forceAbsorb   = shield.m_forceAbsorb;
    }

    // Add absorbtion from armor...

    AttackWeapon attackWeapon = otherHitObject.GetComponent<AttackWeapon> ();


    float damageOutput = absorbCalc (attackWeapon.m_damage, damageAbsorb);
    float forceOutput  = absorbCalc (attackWeapon.m_force,  forceAbsorb);

    if (damageOutput <= 0f && forceOutput <= 0f)
      return;
    
    float currentTime = Time.time;
    if (currentTime < m_latestInjuryTime + m_damageCooldownInSeconds)
      return;
    
    m_latestInjuryTime = currentTime;
    
    logger.Debug (myHitObject.name  + " (y:" + myHitObject.transform.position.y + " injured by " + otherHitObject.name + " at y:" + contact.point.y +
                  ". damageOutput: " + damageOutput + " forceOutput: " + forceOutput);

    inflictInjury (damageOutput, forceOutput, contact.point);
  }
}
