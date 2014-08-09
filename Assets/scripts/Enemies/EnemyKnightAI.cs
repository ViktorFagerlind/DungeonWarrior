using UnityEngine;
using System.Collections;

public class EnemyKnightAI : AttackableCharacter
{	
  public enum State
  { 
    Idle,
    Attack
  };

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
  public float m_range = 4f;
  public float m_sightRange = 10f;

  private bool m_playerIsLeft;
  private bool m_playerIsWithinRange;
  private bool m_playerIsWithinSight;

  private State m_state = State.Idle;

  Player m_player;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void MoveTowardsPlayer ()
  {
    float dir;

    if (m_playerIsWithinRange)
      dir = 0f;
    else if (m_playerIsLeft)
      dir = -1f;
    else 
      dir = 1f;

    Move (dir);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void IdleMode ()
  {
    Move (0f);

    m_state = State.Attack;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void AttackMode ()
  {
    if (m_playerIsWithinRange)
    {
      Move (0f);
      if (Random.value < 0.3)
        SwingLow ();
      else
        SwingHigh ();
    }
    else if (m_playerIsWithinSight)
      MoveTowardsPlayer ();
    else
      m_state = State.Idle;
  }
  

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Awake ()
  {
    base.Awake ();

    m_player = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<Player> ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  // Use this for initialization
	public override void Start () 
	{
		base.Start();
	}
	
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Update ()
  {
    base.Update ();

    m_playerIsLeft = m_player.transform.position.x < transform.position.x;

    m_playerIsWithinRange = ((m_facingLeft && m_playerIsLeft) || (!m_facingLeft && !m_playerIsLeft)) &&
      (Mathf.Abs(transform.position.x - m_player.transform.position.x) < m_range);

    m_playerIsWithinSight = ((m_facingLeft && m_playerIsLeft) || (!m_facingLeft && !m_playerIsLeft)) &&
      (Mathf.Abs(transform.position.x - m_player.transform.position.x) < m_sightRange);

    switch (m_state)
    {
      case State.Idle:
        IdleMode ();
        break;

      case State.Attack:
        AttackMode ();
        break;
    }



  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}
}
