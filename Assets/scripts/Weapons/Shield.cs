using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Shield : MonoBehaviour
{
  public float m_damageAbsorb = 5f;
  public float m_forceAbsorb  = 1000f;

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
      charAnims.m_onStateChangeDelegate -= OnStateChange;
  }
  
  void OnStateChange (AnimationState oldState, AnimationState newState)
  {
    m_collider.enabled = ((newState == AnimationState.ProtectHigh) || (newState == AnimationState.ProtectLow));

    // Debug.Log (name + ": new state=" + newState + ", collider enabling=" + m_collider.enabled);
  }
}
