using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------------------------------------------------------------------

public class Character : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  public enum ProtectionType
  { 
    None,
    High,
    Low
  }

  // ---------------------------------------------------------------------------------------------------------------------------------

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

  private readonly int                  m_groundMask = 1 << 8; // Ground layer mask

  private bool                          m_grounded        = false;

  private float                         m_inputHorizontal     = 0.0f;
  private bool                          m_inputJump           = false;

  protected   CharacterAnims            m_characterAnims;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public bool  grounded           {get {return m_grounded;}}
  public float m_horizontalSpeed  {get {return rigidbody2D.velocity.x;}}
    
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public virtual void Awake ()
	{
    m_collider        = GetComponentInChildren<CircleCollider2D> ();
    m_leftRayOrigin   = new Vector2 (m_collider.center.x - m_collider.radius * 0.7f, m_collider.center.y);
    m_rightRayOrigin  = new Vector2 (m_collider.center.x + m_collider.radius * 0.7f, m_collider.center.y);
    m_rayLength       = m_collider.radius * 1.1f;

    m_characterAnims = GetComponentInChildren<CharacterAnims> ();

    m_initialScale = transform.localScale;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private bool IsActionAllowed ()
  {
    return m_characterAnims.GetState () == AnimationState.Idle || 
           m_characterAnims.GetState () == AnimationState.Walk;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private bool IsUserFlipAllowed ()
  {
    return  m_characterAnims.GetState () == AnimationState.Idle         || 
            m_characterAnims.GetState () == AnimationState.Walk         || 
            m_characterAnims.GetState () == AnimationState.Fall         || 
            m_characterAnims.GetState () == AnimationState.ProtectHigh  || 
            m_characterAnims.GetState () == AnimationState.ProtectLow;
    }
    
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void Jump ()
  {
    m_inputJump = true;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void Move (float horizontalFactor)
  {
    m_inputHorizontal = horizontalFactor;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void Protect (ProtectionType type)
  {
/*
    AnimationState state = m_characterAnims.GetState ();
    if (((type == ProtectionType.High)  && (state == AnimationState.ProtectHigh)) ||
        ((type == ProtectionType.Low)   && (state == AnimationState.ProtectLow))  ||
        ((type == ProtectionType.None)  && (state != AnimationState.ProtectHigh) && (state != AnimationState.ProtectLow)))
        return;
*/

    switch (type)
    {
      case ProtectionType.High:
        m_characterAnims.SetProtectHigh (true);
        m_characterAnims.SetProtectLow  (false);
        break;

      case ProtectionType.Low:
        m_characterAnims.SetProtectHigh (false);
        m_characterAnims.SetProtectLow  (true);
        break;

      case ProtectionType.None:
        m_characterAnims.SetProtectHigh (false);
        m_characterAnims.SetProtectLow  (false);
        break;
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void SwingHigh ()
  {
    if (!IsActionAllowed ())
      return;

    m_characterAnims.SwingHigh ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void SwingLow ()
  {
    if (!IsActionAllowed ())
      return;
    
    m_characterAnims.SwingLow ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void SetFacingDirection (bool faceLeft)
  {
    if (faceLeft == m_facingLeft)
      return;

    m_facingLeft = faceLeft;
    logger.Debug (gameObject.name + (faceLeft ? ": left" : ":right"));

    transform.localScale = new Vector3(faceLeft ? m_initialScale.x : -m_initialScale.x, 
                                       m_initialScale.y, 
                                       m_initialScale.z);
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  // Use this for initialization
	public virtual void Start () 
	{
	}
	
  // ---------------------------------------------------------------------------------------------------------------------------------
  
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

    if (!IsActionAllowed ())
    {
      //logger.Debug ("Not Allowed");
      return;
    }

    // logger.Debug ("Allowed");

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

      if (IsActionAllowed ())
      {
        //logger.Debug ("Jump!");

        //force.y += m_jumpForce;
        rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, m_jumpSpeed);
      }

    }

    rigidbody2D.AddForce (force);
	}

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public virtual void Update ()
  {
    // Update animation
    m_characterAnims.SetGrounded (m_grounded);
    m_characterAnims.SetSpeed (m_horizontalSpeed);

//    m_characterAnims.SetProtectHigh (m_inputHighProtection);

    // change direction
    if (IsUserFlipAllowed ())
    {
      if (m_inputHorizontal > m_flipSpeed)
        SetFacingDirection (false);
      else if (m_inputHorizontal < -m_flipSpeed)
        SetFacingDirection (true);
    }

  }
}

