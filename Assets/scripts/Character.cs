using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody2D), typeof (CircleCollider2D))]
public class Character : MonoBehaviour 
{
	// edit these to tune character movement	
	public float                          m_walkForce         = 3f;
  public float                          m_jumpForce         = 20000f;
  public float                          m_maxVelocityChange = 10f;

  // raycast stuff
  private CircleCollider2D              m_collider;
  private Vector2                       m_leftRayOrigin;
  private Vector2                       m_rightRayOrigin;
  private float                         m_rayLength;

  [HideInInspector] private bool        m_grounded = false;
  public bool  grounded         {get {return m_grounded;}}

  public float horizontalSpeed  {get {return rigidbody2D.velocity.x;}}

  private readonly int                  m_groundMask = 1 << 8; // Ground layer mask

  public enum InputHorizontal 
  { 
    None,
    Left,
    Right
  }

  protected InputHorizontal             m_inputHorizontal = InputHorizontal.None;
  protected bool                        m_inputJump       = false;

	public virtual void Awake()
	{
    m_collider        = GetComponentInChildren<CircleCollider2D> ();
    m_leftRayOrigin   = new Vector2 (m_collider.center.x - m_collider.radius * 0.7f, m_collider.center.y);
    m_rightRayOrigin  = new Vector2 (m_collider.center.x + m_collider.radius * 0.7f, m_collider.center.y);
    m_rayLength       = m_collider.radius * 1.1f;
  }
	
	// Use this for initialization
	public virtual void Start () 
	{
	}
	
  public virtual void FixedUpdate ()
	{
    Vector2 force;

    // use raycasts to determine if the player is standing on the ground or not
    Vector2 pos2d = new Vector2 (m_collider.transform.position.x, m_collider.transform.position.y);
    
    m_grounded = Physics2D.Raycast (pos2d + m_leftRayOrigin,  -Vector2.up, m_rayLength, m_groundMask) || 
      Physics2D.Raycast (pos2d + m_rightRayOrigin, -Vector2.up, m_rayLength, m_groundMask);

    // Let the physics to the work if we don't have ground contact.
    if (!m_grounded)
      return;

    // Calculate force along ground plane
    Vector2 targetVelocity  = new Vector2 ();
    
    targetVelocity.x        = m_walkForce * Input.GetAxis("Horizontal");
    Vector2 velocity        = rigidbody2D.velocity;
    Vector2 velocityChange  = targetVelocity - velocity;
    velocityChange.x = Mathf.Clamp(velocityChange.x, -m_maxVelocityChange, m_maxVelocityChange);
    velocityChange.y = 0;

    // f = m * a; dv = dt * a => dv = dt * f/m => f = m * dv / dt
    force = rigidbody2D.mass * velocityChange / Time.fixedDeltaTime;

    if (m_inputJump)
      force.y += m_jumpForce;

    rigidbody2D.AddForce (force);
	}
}

