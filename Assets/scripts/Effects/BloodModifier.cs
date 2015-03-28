using UnityEngine;
using System.Collections;

public class BloodModifier : ParticleModifier
{
  public float m_forceTogether;
  public float m_forceSpace;
  public float m_tooClosePercent;
  public float m_zoneSize;

  public float m_randomness;

  private bool m_isFirst;

  Vector3 m_initialChange;

  protected override void Start ()
  {
    base.Start ();

    m_initialChange = new Vector3 (Random.Range(-1f, 1f),
                                   Random.Range(-1f, 1f),
                                   0f) * m_randomness;

    m_isFirst = true;
  }

  protected override void Modify (ref ParticleSystem.Particle[] particles, int particleCount) 
  {
    if (m_isFirst)
    {
      for (int i=0; i < particleCount; i++)
        particles [i].velocity = m_initialChange;

      m_isFirst = false;
    }

    for (int i=0; i < particleCount-1; i++)
    {

      for (int j=i+1; j < particleCount; j++)
      {
        Vector3 i2j  = particles [j].position - particles [i].position;
        i2j.z = 0;
        Vector3 i2jN = i2j.normalized;
        float sqrDistance = i2j.sqrMagnitude;

        float percent = sqrDistance / m_zoneSize;

        if (percent < m_tooClosePercent)
        {
          float adjustedPercent = percent/m_tooClosePercent;
          float F = (1f - adjustedPercent) * m_forceSpace;

          Vector3 force = i2jN * F;

          particles [i].velocity -= force;
          particles [j].velocity += force;
        }
        else if (percent < 1f)
        {
          float threshDelta = 1.0f - m_tooClosePercent;
          float adjustedPercent = (percent - m_tooClosePercent) / Mathf.Clamp (threshDelta, 0.2f, 1f);
          float F = (Mathf.Cos (adjustedPercent * Mathf.PI * 2.0f) * 0.5f + 0.5f) * m_forceTogether;

          Vector3 force = i2jN * F;
          
          particles [i].velocity += force;
          particles [j].velocity -= force;
        }
      }
    }
  }
}
