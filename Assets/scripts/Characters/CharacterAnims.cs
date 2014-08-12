using UnityEngine;
using System.Collections;

public enum AnimationState
{ 
  IdleToRun,
  Fall,
  SwingHigh,
  SwingLow,
  ProtectHigh,
  ProtectLow,
  DamageHigh,
  DamageLow,
  Death,
  LieDead
}

// ---------------------------------------------------------------------------------------------------------------------------------

public class CharacterAnims : MonoBehaviour 
{
  public delegate void OnStateChange (AnimationState oldState, AnimationState newState);
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public OnStateChange m_onStateChangeDelegate;
  
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
  private Animator 	m_animator;

  AnimationState m_previousState = AnimationState.IdleToRun;

  // state hashes
  private int m_idleToRunStateHash    = Animator.StringToHash("Base Layer.Stand to Run Blend Tree");
  private int m_fallStateHash         = Animator.StringToHash("Base Layer.Fall");
  private int m_swingHighStateHash    = Animator.StringToHash("Base Layer.High-Swing");
  private int m_swingLowStateHash     = Animator.StringToHash("Base Layer.Low-Swing");
  private int m_protectHighStateHash  = Animator.StringToHash("Base Layer.High-Protect");
  private int m_protectLowStateHash   = Animator.StringToHash("Base Layer.Low-Protect");
  private int m_damageHighStateHash   = Animator.StringToHash("Base Layer.High-Damage");
  private int m_damageLowStateHash    = Animator.StringToHash("Base Layer.Low-Damage");
  private int m_deathStateHash        = Animator.StringToHash("Base Layer.Death");
  private int m_lieDeadStateHash      = Animator.StringToHash("Base Layer.Lie dead");

  // parameter hashes
  private int m_groundedHash          = Animator.StringToHash ("Grounded");
  private int m_speedHash             = Animator.StringToHash ("Speed");
  private int m_protectHighHash       = Animator.StringToHash ("ProtectHigh");
  private int m_protectLowHash        = Animator.StringToHash ("ProtectLow");

  // trigger hashes
  private int m_swingHighHash         = Animator.StringToHash ("SwingHigh");
  private int m_swingLowHash          = Animator.StringToHash ("SwingLow");
  private int m_damageHighHash        = Animator.StringToHash ("DamageHigh");
  private int m_damageLowHash         = Animator.StringToHash ("DamageLow");
  private int m_deathHash             = Animator.StringToHash ("Death");
  private int m_abortSwingHash        = Animator.StringToHash ("AbortSwing");

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Awake ()
	{
		// cache components to save on performance
    m_animator  = GetComponentInChildren<Animator> ();
	}

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Start ()
  {
    logger.LogEnabled = false;
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private AnimationState DecodeState (AnimatorStateInfo stateInfo)
  {
    AnimationState state;
    int            stateNameHash = stateInfo.nameHash;

    if (stateNameHash == m_idleToRunStateHash)
      state = AnimationState.IdleToRun;
    else if (stateNameHash == m_fallStateHash)
      state = AnimationState.Fall;
    else if (stateNameHash == m_swingHighStateHash)
      state = AnimationState.SwingHigh;
    else if (stateNameHash == m_swingLowStateHash)
      state = AnimationState.SwingLow;
    else if (stateNameHash == m_protectHighStateHash)
      state = AnimationState.ProtectHigh;
    else if (stateNameHash == m_protectLowStateHash)
      state = AnimationState.ProtectLow;
    else if (stateNameHash == m_damageHighStateHash)
      state = AnimationState.DamageHigh;
    else if (stateNameHash == m_damageLowStateHash)
      state = AnimationState.DamageLow;
    else if (stateNameHash == m_deathStateHash)
      state = AnimationState.Death;
    else if (stateNameHash == m_lieDeadStateHash)
      state = AnimationState.LieDead;
    else
    {
      logger.Error ("Unknown state!!!");
      state = AnimationState.IdleToRun;
    }

    return state;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public AnimationState GetState ()
  {
    AnimationState state = DecodeState (m_animator.GetCurrentAnimatorStateInfo (0));

    // For some animations we want the target state if an animation has started...
    if (m_animator.IsInTransition (0))
    {
      AnimationState nextState = DecodeState (m_animator.GetNextAnimatorStateInfo (0));

      if ((state != AnimationState.DamageLow   &&
           state != AnimationState.DamageHigh  &&
           state != AnimationState.SwingHigh   &&
           state != AnimationState.SwingLow)   &&
          (nextState == AnimationState.IdleToRun  ||
           nextState == AnimationState.Fall       ||
           nextState == AnimationState.DamageHigh ||
           nextState == AnimationState.DamageLow  ||
           nextState == AnimationState.Death      ||
           nextState == AnimationState.LieDead))
        state = nextState;
    }

    if (state != m_previousState)
    {
      logger.LogEnabled = false;
      logger.Debug (gameObject.name + " changed state to " + state);
      m_onStateChangeDelegate (m_previousState, state);
      m_previousState = state;
    }

    return state;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void Death       ()    {m_animator.SetTrigger (m_deathHash);}
  public void SwingHigh   ()    {m_animator.SetTrigger (m_swingHighHash);}
  public void SwingLow    ()    {m_animator.SetTrigger (m_swingLowHash);}
  public void DamageHigh  ()    {m_animator.SetTrigger (m_damageHighHash);}
  public void DamageLow   ()    {m_animator.SetTrigger (m_damageLowHash);}
  public void AbortSwing  ()    {m_animator.SetTrigger (m_abortSwingHash);}

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void SetProtectHigh (bool protect)       {m_animator.SetBool   (m_protectHighHash, protect);}
  public void SetProtectLow  (bool protect)       {m_animator.SetBool   (m_protectLowHash,  protect);}
  public void SetGrounded    (bool grounded)      {m_animator.SetBool   (m_groundedHash,    grounded);}
  public void SetSpeed       (float speed)        {m_animator.SetFloat  (m_speedHash, Mathf.Abs (speed));}

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Update() 
	{
  }
}
