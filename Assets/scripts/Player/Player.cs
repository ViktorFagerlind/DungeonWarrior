using UnityEngine;
using System.Collections;

public class Player : AttackableCharacter 
{	
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	// Use this for initialization
	public override void Start () 
	{
		base.Start();
	}
	
  public override void Update()
  {
    base.Update ();

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

        
    //m_inputHorizontal = Input.GetAxis ("Horizontal");
    Move (Input.GetAxis ("Horizontal"));
  }

  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}


}
