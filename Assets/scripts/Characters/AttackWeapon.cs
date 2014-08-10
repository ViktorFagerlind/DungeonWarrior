﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class AttackWeapon : MonoBehaviour
{
  public float m_damage = 10f;
  public float m_force  = 1000f;

  private BoxCollider2D   m_collider;
  private CharacterAnims  m_charAnims;

  public void DisableCollider ()
  {
    m_collider.enabled = false;
  }

  public void AbortSwing ()
  {
    m_charAnims.AbortSwing ();
  }
  
  void Start ()
  {
    m_collider  = GetComponent<BoxCollider2D> ();
    m_charAnims = GetComponentInParent<CharacterAnims> ();

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
