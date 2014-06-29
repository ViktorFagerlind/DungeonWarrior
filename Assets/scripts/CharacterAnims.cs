using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator), typeof (Character))]
public class CharacterAnims : MonoBehaviour 
{
	private Transform m_transform;
	private Animator 	m_animator;
  private Character m_character;
  private Vector3   m_scale;

	public enum AnimationState 
	{ 
		Stand = 0,
		Walk  = 1,
		Fall  = 2
	}

  private readonly float m_limitSpeed = 0.005f;

	// hash the animation state string to save performance
	private int m_animState;

	void Awake()
	{
		// cache components to save on performance
		m_transform = transform;
    m_animator  = GetComponentInChildren<Animator>  ();
    m_character = GetComponentInChildren<Character> ();

		m_animState = Animator.StringToHash ("AnimationState");

    m_scale = m_transform.localScale;
	}
	
	void Update() 
	{
    if (m_character.horizontalSpeed < -m_limitSpeed)
      m_transform.localScale = new Vector3(-m_scale.x, m_scale.y, m_scale.z);
    else if (m_character.horizontalSpeed > m_limitSpeed)
      m_transform.localScale = new Vector3(m_scale.x, m_scale.y, m_scale.z);

    if(!m_character.grounded)
      m_animator.SetInteger (m_animState, (int)AnimationState.Fall);
    else if(m_character.horizontalSpeed > -m_limitSpeed && m_character.horizontalSpeed < m_limitSpeed)
      m_animator.SetInteger (m_animState, (int)AnimationState.Stand);
    else
      m_animator.SetInteger (m_animState, (int)AnimationState.Walk);
	}
}
