using UnityEngine;
using System.Collections;

public class ForestManAI : FighterBaseAI
{	

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  protected override void ProtectAction ()
  {
    if (m_player.m_characterAnims.State == AnimationState.AttackHigh ||
        m_player.m_characterAnims.State == AnimationState.AttackLow)
      Protect (ProtectionType.Low);
    else
      Protect (ProtectionType.None);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  protected override void AttackAction ()
  {
    if (Random.value < 0.5f)
      AttackLow ();
    else
      AttackHigh ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Awake ()
  {
    base.Awake ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
	public override void Start () 
	{
    base.Start();
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void Update ()
  {
    base.Update ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}

  // ---------------------------------------------------------------------------------------------------------------------------------
}
