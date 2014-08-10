using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class AttackWeapon : MonoBehaviour
{
  public float m_damage = 10f;
  public float m_force  = 1000f;

  [HideInInspector] public Character       m_character;
  [HideInInspector] public BoxCollider2D   m_collider;
  [HideInInspector] public CharacterAnims  m_charAnims;

  void Start ()
  {
    m_collider  = GetComponent<BoxCollider2D> ();
    m_charAnims = GetComponentInParent<CharacterAnims> ();
    m_character = GetComponentInParent<Character> ();

    if (m_charAnims != null)
      m_charAnims.m_onStateChangeDelegate += OnStateChange;
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
