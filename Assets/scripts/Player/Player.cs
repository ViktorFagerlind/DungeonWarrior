using UnityEngine;
using System.Collections;

public class Player : AttackableCharacter 
{	
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private AudioSource m_audioSteps;
  public  AudioClip   m_stepSound;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();

    m_audioSteps = GetComponents<AudioSource> ()[1];
    m_audioSteps.clip = m_stepSound;
    m_audioSteps.loop = true;
	}
	
  public override void Update ()
  {
    base.Update ();

    if (!m_audioSteps.isPlaying &&
        Mathf.Abs (m_horizontalSpeed) > 0.3f && m_characterAnims.GetState () == AnimationState.IdleToRun)
      m_audioSteps.Play ();
    else if (m_audioSteps.isPlaying &&
      Mathf.Abs (m_horizontalSpeed) < 0.3f || m_characterAnims.GetState () != AnimationState.IdleToRun)
      m_audioSteps.Pause ();

    if (Input.GetButtonDown ("Jump"))
    {
      // logger.Debug ("Jump button pressed");
      Jump ();
    }

    if (Input.GetButtonDown ("Swing"))
    {
      // logger.Debug ("Fire button pressed");
      if (Input.GetAxis ("Vertical") < -0.2f)
        SwingLow ();
      else
        SwingHigh ();
    }

    if (Input.GetAxis ("Protect") > 0.2f)
    {
      if (Input.GetAxis ("Vertical") >= 0f)
        Protect (Character.ProtectionType.High);
      else
        Protect (Character.ProtectionType.Low);
    }
    else
      Protect (Character.ProtectionType.None);

    /* if (Input.GetMouseButton (1)) 
    {
      float mouseX = Input.GetAxis ("Mouse X");
      logger.Debug (mouseX.ToString ());
      Move (mouseX);
    }
    else */
    Move (Input.GetAxis ("Horizontal"));
  }

  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}


}
