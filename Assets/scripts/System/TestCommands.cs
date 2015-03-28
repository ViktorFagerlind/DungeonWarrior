using UnityEngine;
using System.Collections;

public class TestCommands : MonoBehaviour
{
  public Transform m_createObj;


  void Update()
  {
    if (Input.GetKeyDown ("space"))
    {
      Instantiate (m_createObj, new Vector3 (-25f, -3.5f, 0f), Quaternion.identity);
    }

  }
}
