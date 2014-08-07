using UnityEngine;
using System.Collections;

public class AttackableCharacter : Character
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  public float m_health = 100f;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void inflictInjury (float damage, float resistance, bool high)
  {
    float damageOutput = damage - resistance;

    if (damageOutput <= 0f)
      return;
      
    logger.Debug ("Damage output: " + damageOutput + " (" + (high?"high":"low") + ")");
    
    m_health -= damageOutput;

    if (m_health <= 0f)
      m_characterAnims.Die ();
    else
    {
      if (high)
        m_characterAnims.HitHigh ();
      else
        m_characterAnims.HitLow ();
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnCollisionEnter2D (Collision2D collision)
  {
    ContactPoint2D contact    = collision.contacts[0];   

    GameObject myHitObject    = contact.otherCollider.gameObject;
    GameObject otherHitObject = contact.collider.gameObject;

    logger.Debug (myHitObject.name  + " hit by " + otherHitObject.name);

    if (otherHitObject.tag != "AttackWeapon")
    {
      logger.Debug ("No weapon hit");
      return;
    }

    float resistance = 0f;
    if (myHitObject.tag == "Shield")
    {
      logger.Debug ("Hit shield");
      Shield shield = myHitObject.GetComponent<Shield> ();
      resistance = shield.m_resistance;
    }

    AttackWeapon attackWeapon = otherHitObject.GetComponent<AttackWeapon> ();

    inflictInjury (attackWeapon.m_damage, resistance, contact.point.y >= myHitObject.transform.position.y);
  }
}
