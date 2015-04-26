using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D), typeof (AudioSource))]
public class AttackWeapon : Item
{
  public enum HitType
  { 
    Unprotected,
    Protected
  };

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
  public float            m_damage = 10f;
  public float            m_force  = 1000f;
  public AudioClip        m_hitArmorSound;
  public AudioClip        m_hitShieldSound;
  public AnimationState[] m_activateStates;

  [HideInInspector] public Character       m_character;
  [HideInInspector] public BoxCollider2D   m_collider;
  [HideInInspector] public CharacterAnims  m_charAnims;

  private bool  m_swingingBack;
  private float m_previousRelX;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void OnHit (HitType hitType)
  {
    switch (hitType)
    {
      case HitType.Protected: 
        GetComponent<AudioSource>().clip = m_hitShieldSound;
        break;
      case HitType.Unprotected: 
        GetComponent<AudioSource>().clip = m_hitArmorSound;
        break;
    }

    GetComponent<AudioSource>().Play ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Start ()
  {
    m_collider  = GetComponent<BoxCollider2D> ();
    m_charAnims = GetComponentInParent<CharacterAnims> ();
    m_character = GetComponentInParent<Character> ();

    m_collider.enabled = false;

    m_swingingBack = false;

    if (m_charAnims != null)
      m_charAnims.m_onStateChangeDelegate += OnStateChange;

    logger.LogEnabled = false;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Update ()
  {
    if (!m_swingingBack)
      return;

    float currentRelX = transform.position.x - m_character.transform.position.x;

    logger.Debug (gameObject.name + "prev x:" + m_previousRelX + ", x:" + currentRelX + "(" + transform.position.x + ", " + m_character.transform.position.x + ")");

    // Enable the collider when the weapon is moving forward, to avoid hit when raising the sword...
    if (( m_character.m_facingLeft && (currentRelX < m_previousRelX)) ||
        (!m_character.m_facingLeft && (currentRelX > m_previousRelX)))
    {
      m_collider.enabled = true;
      m_swingingBack = false;
      //logger.Debug (gameObject.name + ": swing forward");
      //UnityEditor.EditorApplication.isPaused = true;
    }
    else
      m_previousRelX = currentRelX;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnDestroy ()
  {
    if (m_charAnims != null)
      m_charAnims.m_onStateChangeDelegate -= OnStateChange;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnStateChange (AnimationState oldState, AnimationState newState)
  {
    bool IsActivated = false;
    foreach (AnimationState s in m_activateStates)
    {
      if (s == newState)
      {
        IsActivated = true;
        break;
      }
    }

    if (IsActivated)
    {
      m_swingingBack = true;      // Starting swing by raising weapon
      m_previousRelX = transform.position.x - m_character.transform.position.x;
      //logger.Debug (gameObject.name + ": Raise sword");
    }
    else
    {
      m_swingingBack      = false;  // Should not be neccesary, but...
      m_collider.enabled  = false;  // Swing ended
      //logger.Debug (gameObject.name + ": Swing done");
      //UnityEditor.EditorApplication.isPaused = true;
    }
  }
}
