using UnityEngine;
using System.Collections;

public class EnemyKnightAI : AttackableCharacter
{	
  public enum State
  { 
    Idle,
    Fight
  };

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  private static readonly float m_fightTimeResolution = 0.05f;

  public float    m_range = 4f;
  public float    m_sightRange = 10f;

  private bool    m_isAggressive = true;
  
  private bool    m_playerIsLeft            = false;
  private float   m_playerDistance          = 0f;
  private float   m_previousPlayerDistance  = 0f;
  private bool    m_facingPlayer            = false;
  private bool    m_playerFacingUs          = false;
  private bool    m_playerIsWithinRange     = false;
  private bool    m_playerIsWithinSight     = false;

  private float   m_moveSpeed = 0f;

  private State   m_state = State.Idle;

  Player          m_player;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnStateChange (AnimationState oldState, AnimationState newState)
  {
    switch (newState)
    {
      case AnimationState.DamageHigh:
      case AnimationState.DamageLow:
        m_isAggressive = false;
        break;
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void MoveTowardsPlayer ()
  {
    if (m_playerIsWithinRange)
      m_moveSpeed = 0f;
    else if (m_playerIsLeft)
      m_moveSpeed -= 0.5f * m_fightTimeResolution; // Add 0.5 to speed every second
    else 
      m_moveSpeed += 0.5f * m_fightTimeResolution;

    m_moveSpeed = Mathf.Clamp (m_moveSpeed, -1f, 1f);    

    //logger.Debug ("moveSpeed: " + m_moveSpeed);
    Move (m_moveSpeed);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private IEnumerator IdleMode ()
  {
    logger.Debug ("Enter IdleMode");

    while (!m_playerIsWithinSight)
    {
      m_moveSpeed = Random.Range (-1f, 1f);
      if (Mathf.Abs (m_moveSpeed) < 0.6f)
        m_moveSpeed = 0f;

      Move (m_moveSpeed);

      yield return new WaitForSeconds (2f);
    }

    m_state = State.Fight;

    logger.Debug ("Exit IdleMode");
    yield break;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void Protect ()
  {
    if (m_player.m_characterAnims.GetState () == AnimationState.SwingHigh)
    {
      //logger.Debug ("protect high");
      Protect (ProtectionType.High);
    }
    else if (m_player.m_characterAnims.GetState () == AnimationState.SwingLow)
    {
      //logger.Debug ("protect low");
      Protect (ProtectionType.Low);
    }
    else
      Protect (ProtectionType.None);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void FightAggressively ()
  {
    Protect (ProtectionType.None);

    if (Random.value < 0.0f)
      SwingLow ();
    else
      SwingHigh ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void FightPassively ()
  {
    Protect ();
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private IEnumerator FightMode ()
  {
    logger.Debug ("Enter AttackMode");

    while (m_playerIsWithinSight)
    {
      if (m_playerIsWithinRange)
      {
        if (!m_playerFacingUs) //  Aggressive if player not turned away)
          m_isAggressive = true;
        else if (Random.Range (0f, 3f) < m_fightTimeResolution) // Switch ransomly about every 3s
        {
          m_isAggressive = !m_isAggressive;
          logger.Debug ("Switched to " + (m_isAggressive?"aggressive":"passive"));
        }
        
        //logger.Debug ("Fighting...");

        m_moveSpeed = 0f;
        Move (m_moveSpeed);

        if (m_isAggressive)
          FightAggressively ();
        else
          FightPassively ();

        yield return new WaitForSeconds (m_fightTimeResolution);
      }
      else
      {
        //logger.Debug ("Moving...");
        
        MoveTowardsPlayer ();
        yield return new WaitForSeconds (m_fightTimeResolution);
      }
    }

    m_isAggressive = true;
    m_state = State.Idle;
    logger.Debug ("Exit AttackMode");
    yield break;
  }
  

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Awake ()
  {
    base.Awake ();

    m_player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  // Use this for initialization
	public override void Start () 
	{
    base.Start();

    // logger.LogEnabled = false;

    m_characterAnims.m_onStateChangeDelegate += OnStateChange;

    StartCoroutine (UpdateStateMachine ());
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnDestroy ()
  {
    m_characterAnims.m_onStateChangeDelegate -= OnStateChange;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Update ()
  {
    base.Update ();

    m_playerIsLeft    = m_player.transform.position.x < transform.position.x;
    m_playerDistance  = Mathf.Abs(transform.position.x - m_player.transform.position.x);
    m_facingPlayer    = (m_facingLeft && m_playerIsLeft) || (!m_facingLeft && !m_playerIsLeft);
    m_playerFacingUs  = (m_player.m_facingLeft && !m_playerIsLeft) || (!m_player.m_facingLeft && m_playerIsLeft);

    float speed = (m_playerDistance - m_previousPlayerDistance) / Time.deltaTime;
    float predictedHitDistance = m_playerDistance + speed * m_fightTimeResolution + 
                                 speed / 2f * 0.5f; // Assume 0.5s from start to hit of swing and that the speed during swing will be 1/2 of the current speed

    //logger.Debug ("speed: " +speed + ", " + "predictedHitDistance: " + predictedHitDistance);

    m_playerIsWithinRange = m_facingPlayer && (predictedHitDistance < m_range) && !m_player.isDead;
    m_playerIsWithinSight = m_facingPlayer && (m_playerDistance < m_sightRange) && !m_player.isDead;

    m_previousPlayerDistance = m_playerDistance;
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

        case State.Fight:
          yield return StartCoroutine (FightMode ());
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
