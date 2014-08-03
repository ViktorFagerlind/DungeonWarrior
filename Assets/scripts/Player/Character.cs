using UnityEngine;
using System.Collections;

public enum AnimationState
{ 
  Idle,
  Walk,
  Fall,
  SwingHigh,
  SwingLow
}

[RequireComponent (typeof (Rigidbody2D), typeof (CircleCollider2D))]
public class Character : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	// edit these to tune character movement	
	public float                          m_walkForce         = 3f;
  public float                          m_jumpSpeed         = 4f;
  public float                          m_maxVelocityChange = 10f;

  protected bool                        m_facingLeft = true;

  // raycast stuff
  private CircleCollider2D              m_collider;
  private Vector2                       m_leftRayOrigin;
  private Vector2                       m_rightRayOrigin;
  private float                         m_rayLength;
  private Vector3                       m_initialScale;

  private float                         m_flipSpeed = 0.05f;


  [HideInInspector] private bool        m_grounded = false;
  public bool  grounded         {get {return m_grounded;}}

  public float m_horizontalSpeed  {get {return rigidbody2D.velocity.x;}}

  private readonly int                  m_groundMask = 1 << 8; // Ground layer mask

  private float                       m_inputHorizontal     = 0.0f;
  private bool                        m_inputJump           = false;

  private   CharacterAnims              m_characterAnims;

	public virtual void Awake()
	{
    m_collider        = GetComponentInChildren<CircleCollider2D> ();
    m_leftRayOrigin   = new Vector2 (m_collider.center.x - m_collider.radius * 0.7f, m_collider.center.y);
    m_rightRayOrigin  = new Vector2 (m_collider.center.x + m_collider.radius * 0.7f, m_collider.center.y);
    m_rayLength       = m_collider.radius * 1.1f;

    m_characterAnims = GetComponentInChildren<CharacterAnims> ();

    m_initialScale = transform.localScale;
  }
	
  private bool IsSwingAllowed ()
  {
    return m_characterAnims.GetState () == AnimationState.Idle || 
           m_characterAnims.GetState () == AnimationState.Walk;
  }

  private bool IsMoveAllowed ()
  {
    return m_characterAnims.GetState () == AnimationState.Idle || 
           m_characterAnims.GetState () == AnimationState.Walk;
  }
  
  private bool IsJumpAllowed ()
  {
    return m_characterAnims.GetState () == AnimationState.Idle || 
           m_characterAnims.GetState () == AnimationState.Walk;
  }
  
  public void Jump ()
  {
    m_inputJump = true;
  }

  public void Move (float horizontalFactor)
  {
    m_inputHorizontal = horizontalFactor;
  }
  
  public void SwingHigh ()
  {
    if (!IsSwingAllowed ())
      return;

    m_characterAnims.SwingHigh ();
  }

  public void SwingLow ()
  {
    if (!IsSwingAllowed ())
      return;
    
    m_characterAnims.SwingLow ();
  }
  
  // Use this for initialization
	public virtual void Start () 
	{
	}
	
  public virtual void FixedUpdate ()
	{
//    Debug.Log ("FixedUpdate");

    Vector2 force;

    // use raycasts to determine if the player is standing on the ground or not
    Vector2 pos2d = new Vector2 (m_collider.transform.position.x, m_collider.transform.position.y);
    
    m_grounded = Physics2D.Raycast (pos2d + m_leftRayOrigin,  -Vector2.up, m_rayLength, m_groundMask) || 
      Physics2D.Raycast (pos2d + m_rightRayOrigin, -Vector2.up, m_rayLength, m_groundMask);

    //Debug.Log ("Grounded: " + m_grounded);

    // Let the physics to the work if we don't have ground contact.
//    if (!m_grounded)
//      return;

    if (!IsMoveAllowed ())
      return;

    // Calculate force along ground plane
    Vector2 targetVelocity  = new Vector2 ();
    
    targetVelocity.x        = m_walkForce * m_inputHorizontal;
    Vector2 velocity        = rigidbody2D.velocity;
    Vector2 velocityChange  = targetVelocity - velocity;
    velocityChange.x = Mathf.Clamp(velocityChange.x, -m_maxVelocityChange, m_maxVelocityChange);
    velocityChange.y = 0;

    // f = m * a; dv = dt * a => dv = dt * f/m => f = m * dv / dt
    force = rigidbody2D.mass * velocityChange / Time.fixedDeltaTime;

    if (m_inputJump)
    {
      m_inputJump = false;
      logger.Debug ("No more jump input");

      if (IsJumpAllowed ())
      {
        logger.Debug ("Jump!");

        //force.y += m_jumpForce;
        rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, m_jumpSpeed);
      }

    }

    rigidbody2D.AddForce (force);
	}

  public virtual void Update ()
  {
    // Update animation
    m_characterAnims.SetGrounded (m_grounded);
    m_characterAnims.SetSpeed (m_horizontalSpeed);

    // change direction
//    if (m_characterAnims.GetState () == AnimationState.Walk)
    {
      if (m_horizontalSpeed > m_flipSpeed)
      {
        m_facingLeft = true;
        transform.localScale = new Vector3(-m_initialScale.x, m_initialScale.y, m_initialScale.z);
      }
      else if (m_horizontalSpeed < -m_flipSpeed)
      {
        m_facingLeft = true;
        transform.localScale = new Vector3(m_initialScale.x, m_initialScale.y, m_initialScale.z);
      }
    }

  }
}

