using UnityEngine;
using System.Collections;

public class CharacterAnims : MonoBehaviour 
{
	private Animator 	m_animator;

  // state hashes
  //private int m_idleStateHash       = Animator.StringToHash("Base Layer.Idle");
  private int m_walkStateHash         = Animator.StringToHash("Base Layer.Walk");
  private int m_fallStateHash         = Animator.StringToHash("Base Layer.Fall");
  private int m_swingHighStateHash    = Animator.StringToHash("Base Layer.High-Swing");
  private int m_swingLowStateHash     = Animator.StringToHash("Base Layer.Low-Swing");
  private int m_protectHighStateHash  = Animator.StringToHash("Base Layer.High-Protect");
  private int m_protectLowStateHash   = Animator.StringToHash("Base Layer.Low-Protect");

  // parameter hashes
  private int m_groundedHash          = Animator.StringToHash ("Grounded");
  private int m_speedHash             = Animator.StringToHash ("Speed");
  private int m_protectHighHash       = Animator.StringToHash ("ProtectHigh");
  private int m_protectLowHash        = Animator.StringToHash ("ProtectLow");

  // trigger hashes
  private int m_swingHighHash         = Animator.StringToHash ("SwingHigh");
  private int m_swingLowHash          = Animator.StringToHash ("SwingLow");

	void Awake()
	{
		// cache components to save on performance
    m_animator  = GetComponentInChildren<Animator> ();
	}

  public AnimationState GetState ()
  {
    AnimatorStateInfo stateInfo;

    // We want the target state if an animation has started...
    if (m_animator.IsInTransition (0))
      stateInfo = m_animator.GetNextAnimatorStateInfo (0);
    else
      stateInfo = m_animator.GetCurrentAnimatorStateInfo (0);

    int stateNameHash = stateInfo.nameHash;

    if (stateNameHash == m_walkStateHash)
      return AnimationState.Walk;
    if (stateNameHash == m_fallStateHash)
      return AnimationState.Fall;
    if (stateNameHash == m_swingHighStateHash)
      return AnimationState.SwingHigh;
    if (stateNameHash == m_swingLowStateHash)
      return AnimationState.SwingLow;
    if (stateNameHash == m_protectHighStateHash)
      return AnimationState.ProtectHigh;
    if (stateNameHash == m_protectLowStateHash)
      return AnimationState.ProtectLow;

    return AnimationState.Idle;
  }

  public void SwingHigh ()    {m_animator.SetTrigger (m_swingHighHash);}
  public void SwingLow  ()    {m_animator.SetTrigger (m_swingLowHash);}
  public void Die       ()    {}
  public void HitHigh   ()    {}
  public void HitLow    ()    {}

  public void SetProtectHigh (bool protect)       {m_animator.SetBool   (m_protectHighHash, protect);}
  public void SetProtectLow  (bool protect)       {m_animator.SetBool   (m_protectLowHash,  protect);}
  public void SetGrounded    (bool grounded)      {m_animator.SetBool   (m_groundedHash,    grounded);}
  public void SetSpeed       (float speed)        {m_animator.SetFloat  (m_speedHash, Mathf.Abs (speed));}

  void Update() 
	{
  }
}
