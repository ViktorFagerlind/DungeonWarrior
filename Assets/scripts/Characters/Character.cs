using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------------------------------------------------------------------

[RequireComponent (typeof (AudioSource))]
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
	public float                            m_walkVelocity        = 3f;
  public float                            m_jumpSpeed           = 4f;
  public float                            m_maxVelocityChange   = 10f;

  public StaminaManager                   m_stamina;

  // Sounds
  public AudioClip                        m_attackSound;
  public AudioClip                        m_damagedSound;
  public AudioClip                        m_deathSound;
    
  [HideInInspector] public bool           m_facingLeft = true;
  
  [HideInInspector] public CharacterAnims m_characterAnims;
  
  protected   Vector2                     m_hitForceToApply = new Vector2 (0f, 0f);
  protected   AudioSource                 m_audioFx;

  // raycast stuff
  private CircleCollider2D                m_collider;
  private Vector2                         m_leftRayOrigin;
  private Vector2                         m_rightRayOrigin;
  private float                           m_rayLength;
  private Vector3                         m_initialScale;

  private float                           m_flipSpeed = 0.05f;

  private readonly int                    m_groundMask = 1 << 8; // Ground layer mask

  private bool                            m_grounded            = false;

  private float                           m_inputHorizontal     = 0.0f;
  private bool                            m_inputJump           = false;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public bool  grounded                   {get {return m_grounded;}}
  public float m_horizontalSpeed          {get {return GetComponent<Rigidbody2D>().velocity.x;}}
    
  // ---------------------------------------------------------------------------------------------------------------------------------

  void PlaySound (AudioClip clip)
  {
    if (clip)
    {
      m_audioFx.clip = clip;
      m_audioFx.Play ();
    }
  }
    
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void DeathActions ()
  {
    // Drop items
    Item[] items = GetComponentsInChildren<Item> ();
    foreach (Item i in items)
      i.Drop ();

    // Destroy scripts
    MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour> ();
    foreach (MonoBehaviour s in scripts)
      Destroy (s);

    Destroy (GetComponent<Rigidbody2D> ());
    
    // Destroy Audio
    AudioSource[] audios = GetComponentsInChildren<AudioSource> ();
    foreach (AudioSource a in audios)
      Destroy (a);
    
    // Destroy colliders
    Collider2D[] colliders = GetComponentsInChildren<Collider2D> ();
    foreach (Collider2D c in colliders)
      Destroy (c);

  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnStateChange (AnimationState oldState, AnimationState newState)
  {
    switch (oldState)
    {
      case AnimationState.AttackHigh:
      case AnimationState.AttackLow:
        m_characterAnims.AnimationSpeed = 1f; // Restore animation speed from potentially low stamina
        m_stamina.ReduceAttack ();
        break;
    }

    switch (newState)
    {
      case AnimationState.AttackHigh:
      case AnimationState.AttackLow:
        m_characterAnims.AnimationSpeed = Mathf.Clamp (m_stamina.LimitPercentage, 0.5f, 1f); // Slow down attack according to stamina
        PlaySound (m_attackSound);
        break;

      case AnimationState.Death:
        logger.Debug (gameObject.name + ": Entered death state");
        PlaySound (m_deathSound);
        break;
                
      case AnimationState.DamageHigh:
      case AnimationState.DamageLow:
        PlaySound (m_damagedSound);
        break;
                
      case AnimationState.LieDead:
        logger.Debug ("============ " + gameObject.name + " lies dead ============");
        DeathActions ();
        break;
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public virtual void Awake ()
	{
    m_collider        = GetComponentInChildren<CircleCollider2D> ();
    m_leftRayOrigin   = new Vector2 (m_collider.offset.x - m_collider.radius * 0.7f, m_collider.offset.y);
    m_rightRayOrigin  = new Vector2 (m_collider.offset.x + m_collider.radius * 0.7f, m_collider.offset.y);
    m_rayLength       = m_collider.radius * 1.1f;

    m_characterAnims = GetComponentInChildren<CharacterAnims> ();

    m_initialScale = transform.localScale;
  }

  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  // Use this for initialization
  public virtual void Start () 
  {
    m_audioFx = GetComponents<AudioSource> ()[0];

    logger.LogEnabled = false;

    m_characterAnims.m_onStateChangeDelegate += OnStateChange;
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void OnDestroy ()
  {
    m_characterAnims.m_onStateChangeDelegate -= OnStateChange;
  }
    
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  protected bool IsAttack ()
  {
    return m_characterAnims.State == AnimationState.AttackHigh ||
           m_characterAnims.State == AnimationState.AttackLow;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  protected bool IsActionAllowed ()
  {
    return m_characterAnims.State == AnimationState.IdleToRun;
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private bool IsUserFlipAllowed ()
  {
    return  m_characterAnims.State == AnimationState.IdleToRun    || 
            m_characterAnims.State == AnimationState.Fall         || 
            m_characterAnims.State == AnimationState.ProtectHigh  || 
            m_characterAnims.State == AnimationState.ProtectLow;
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
  
  public void AttackHigh ()
  {
    if (!IsActionAllowed ())
      return;

    m_characterAnims.AttackHigh ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void AttackLow ()
  {
    if (!IsActionAllowed ())
      return;
    
    m_characterAnims.AttackLow ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void SetFacingDirection (bool faceLeft)
  {
    if (faceLeft == m_facingLeft)
      return;

    m_facingLeft = faceLeft;
    //logger.Debug (gameObject.name + (faceLeft ? ": left" : ":right"));

    transform.localScale = new Vector3(faceLeft ? m_initialScale.x : -m_initialScale.x, 
                                       m_initialScale.y, 
                                       m_initialScale.z);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public virtual void FixedUpdate ()
	{
    // use raycasts to determine if the player is standing on the ground or not
    Vector2 pos2d = new Vector2 (m_collider.transform.position.x, m_collider.transform.position.y);
    
    m_grounded = Physics2D.Raycast (pos2d + m_leftRayOrigin,  -Vector2.up, m_rayLength, m_groundMask) || 
      Physics2D.Raycast (pos2d + m_rightRayOrigin, -Vector2.up, m_rayLength, m_groundMask);

    //Debug.Log ("Grounded: " + m_grounded);

    // Apply force from hit?
    if (Mathf.Abs (m_hitForceToApply.x) > 0f)
    {
      //rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);

      GetComponent<Rigidbody2D>().AddForce (m_hitForceToApply);
      SetFacingDirection (m_hitForceToApply.x > 0f);

      m_hitForceToApply = new Vector2 (0f, 0f);

      return;
    }

    // Let the physics to the work if we don't have ground contact.
//    if (!m_grounded)
//      return;

    if (!IsActionAllowed ())
    {
      m_inputJump = false;
      return;
    }

    if (m_inputJump)
    {
      logger.Debug (gameObject.name + ": Jump");
      GetComponent<Rigidbody2D>().velocity = new Vector2 (GetComponent<Rigidbody2D>().velocity.x, m_jumpSpeed);
    }

    // Calculate force along ground plane
    Vector2 targetVelocity  = new Vector2 ();
    
    targetVelocity.x        = m_walkVelocity * m_inputHorizontal;
    Vector2 velocity        = GetComponent<Rigidbody2D>().velocity;
    Vector2 velocityChange  = targetVelocity - velocity;
    velocityChange.x = Mathf.Clamp(velocityChange.x, -m_maxVelocityChange, m_maxVelocityChange);
    velocityChange.y = 0;

    // f = m * a; dv = dt * a => dv = dt * f/m => f = m * dv / dt
    Vector2 force = GetComponent<Rigidbody2D>().mass * velocityChange / Time.fixedDeltaTime;

    //logger.Debug (gameObject.name +  ": Apply move force");
    GetComponent<Rigidbody2D>().AddForce (force);
	}

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public virtual void Update ()
  {
    // Re-generate stamina
    m_stamina.Regenerate (Time.deltaTime);

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

