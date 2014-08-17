using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour 
{
  public bool m_droppable = true;

  public bool Drop ()
  {
    if (!m_droppable)
      return false;

    transform.parent = null;

    // Disable scripts
    MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour> ();
    foreach (MonoBehaviour s in scripts)
      s.enabled = false;
    
    // Destroy Audio
    AudioSource[] audios = GetComponentsInChildren<AudioSource> ();
    foreach (AudioSource a in audios)
      a.enabled = false;

    // Destroy colliders
    Collider2D[] colliders = GetComponentsInChildren<Collider2D> ();
    foreach (Collider2D c in colliders)
      c.enabled = false;

    // Make it a pickable item
    gameObject.AddComponent<PickableItem> ();

    CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D> ();
    collider.isTrigger  = true;
    collider.radius     = 1.4f;

    return true;
  }
}
