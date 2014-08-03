using UnityEngine;
using System.Collections;

public class EnemyKnightAI : Character 
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

    GameObject player = GameObject.FindGameObjectWithTag ("Player");

    if (player == null)
      return;

  }

  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}
}
