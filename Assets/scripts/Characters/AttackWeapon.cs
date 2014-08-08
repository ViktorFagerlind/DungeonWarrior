using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class AttackWeapon : MonoBehaviour
{
  public float m_damage = 10f;
  public float m_force  = 1000f;

  private BoxCollider2D m_collider;

  void Start ()
  {
    m_collider = GetComponent<BoxCollider2D> ();
    
    CharacterAnims charAnims = GetComponentInParent<CharacterAnims> ();

    if (charAnims != null)
      charAnims.m_onStateChangeDelegate += OnStateChange;
  }

  void OnDestroy ()
  {
    CharacterAnims charAnims = GetComponentInParent<CharacterAnims> ();
    
    if (charAnims != null)
      charAnims.m_onStateChangeDelegate += OnStateChange;
  }

  void OnStateChange (AnimationState oldState, AnimationState newState)
  {
    m_collider.enabled = ((newState == AnimationState.SwingHigh) || (newState == AnimationState.SwingLow));

    // Debug.Log (name + ": new state=" + newState + ", collider enabling=" + m_collider.enabled);
  }
}
