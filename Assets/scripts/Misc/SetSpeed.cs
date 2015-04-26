using UnityEngine;
using System.Collections;

public class SetSpeed : MonoBehaviour 
{
  public Vector2 m_speed;


  void OnMouseUp ()
  {
    GetComponent<Rigidbody2D>().velocity = m_speed;
  }
}
