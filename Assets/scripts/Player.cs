using UnityEngine;
using System.Collections;

public class Player : Character 
{	
	// Use this for initialization
	public override void Start () 
	{
		base.Start();
	}
	
  public void Update()
  {
    // jump
    m_inputJump = Input.GetKeyDown(KeyCode.W);

    // move left
    if(Input.GetKey(KeyCode.A))
      m_inputHorizontal = InputHorizontal.Left;
    // move right
    else if (Input.GetKey(KeyCode.D)) 
      m_inputHorizontal = InputHorizontal.Right;
    // none
    else
      m_inputHorizontal = InputHorizontal.None;
  }

  public override void FixedUpdate()
	{
    base.FixedUpdate ();
	}

}
