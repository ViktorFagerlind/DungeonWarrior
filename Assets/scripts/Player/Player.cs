using UnityEngine;
using System.Collections;

public class Player : Character 
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

    if (/*Input.GetAxis ("Vertical") > 0.5f || */ Input.GetKeyDown (KeyCode.W))
    {
      logger.Debug ("Jump button pressed");
      Jump ();
    }

    if (/*Input.GetButton ("Fire1") ||*/ Input.GetKeyDown (KeyCode.G))
    {
      logger.Debug ("Fire button pressed");

      if (Input.GetAxis ("Vertical") < -0.2f)
        SwingLow ();
      else
        SwingHigh ();
    }

    //m_inputHorizontal = Input.GetAxis ("Horizontal");
    Move (Input.GetAxis ("Horizontal"));
  }

  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}
}
