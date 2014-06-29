using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

  public Transform m_objectToFollow;

	// Update is called once per frame
	void Update () 
  {
    transform.position = new Vector3 (m_objectToFollow.position.x, transform.position.y, transform.position.z);
	}
}
