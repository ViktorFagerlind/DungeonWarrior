using UnityEngine;
using System.Collections;

[RequireComponent (typeof(ParticleSystem))]
public abstract class ParticleModifier : MonoBehaviour
{
  protected ParticleSystem            m_particleSystem;
  protected ParticleSystem.Particle[] m_particles = new ParticleSystem.Particle[1000];
  protected int                       m_particleCount;

  protected virtual void Start()
  {
    m_particleSystem = GetComponent<ParticleSystem>();
  }

  protected void Update()
  {
    m_particleCount = GetComponent<ParticleSystem>().GetParticles (m_particles);

    if (m_particleCount != 0)
    {
      Modify (ref m_particles, m_particleCount);

      m_particleSystem.SetParticles (m_particles, m_particleCount);
    }
  }

  protected void LateUpdate () 
  {
    if (!particleSystem.IsAlive())
      GameObject.Destroy (gameObject); 
  }

  protected abstract void Modify(ref ParticleSystem.Particle[] particles, int particleCount);
}
