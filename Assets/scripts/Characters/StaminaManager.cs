using UnityEngine;
using System.Collections;

[System.Serializable]
public class StaminaManager
{
  public                  float           m_max                = 100f;
  public                  float           m_regenerationSpeed  = 7f;       // Stamina regeneration per second

  private static readonly float           m_attackCost         = 20f;      // Stamina cost for attack
  private static readonly float           m_forceCost          = 0.003f;   // Stamina cost per 1N force impact
  private                 float           m_limit;                         // Under this limit stamina affects performance
  private                 float           m_current;

  public float LimitPercentage     {get {return (m_current>m_limit)? 1f : m_current / m_limit;}}
  public float Percentage          {get {return m_current / m_max;}}

  public StaminaManager ()
  {
    m_current           = m_max;
    m_limit             = m_max * 0.7f;
  }

  private void ReduceStamina (float staminaReduction)
  {
    m_current = Mathf.Clamp (m_current - staminaReduction, 0f, m_max);
  }

  private void IncreaseStamina (float staminaIncrease)
  {
    m_current = Mathf.Clamp (m_current + staminaIncrease, 0f, m_max);
  }
  
  public void ReduceByForce (float force)
  {
    ReduceStamina (force * m_forceCost);
  }

  public void ReduceAttack ()
  {
    ReduceStamina (m_attackCost);
  }

  public void Regenerate (float time)
  {
    IncreaseStamina (time * m_regenerationSpeed);
  }

}
