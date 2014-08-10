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
  
  private IEnumerator IdleMode ()
  {
    logger.Debug ("Enter IdleMode");

    while (!m_playerIsWithinSight)
    {
      float r = Random.value;

      if (r < 0.3)
        Move (0f);
      else if (r < 0.6)
        Move (-1f);
      else
        Move (1f);

      yield return new WaitForSeconds (1f);
    }

    m_state = State.Attack;

    logger.Debug ("Exit IdleMode");
    yield break;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private IEnumerator AttackMode ()
  {
    logger.Debug ("Enter AttackMode");
    if (m_playerIsWithinRange)
    {
      if (Random.value < 0.3)
        SwingLow ();
      else
        SwingHigh ();

      yield return new WaitForSeconds (1f);
    }
    else if (m_playerIsWithinSight)
    {
      MoveTowardsPlayer ();
      yield return new WaitForSeconds (1f);
    }
    else
    {
      m_state = State.Idle;
      logger.Debug ("Exit AttackMode");
      yield break;
    }
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

    StartCoroutine (UpdateStateMachine ());
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
  }

  // ---------------------------------------------------------------------------------------------------------------------------------

  public IEnumerator UpdateStateMachine ()
  {
    while (true)
    {
      logger.Debug ("Main State Machine");

      switch (m_state)
      {
        case State.Idle:
          yield return StartCoroutine (IdleMode ());
          break;

        case State.Attack:
          yield return StartCoroutine (AttackMode ());
          break;
      }
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}
}
