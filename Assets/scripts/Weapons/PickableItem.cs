using UnityEngine;
using System.Collections;

public class PickableItem : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private void MenuDone (string context, bool pressedOk)
  {
    if (!pressedOk)
    {
      logger.Debug ("Player discarded " + gameObject.name + "...");
      return;
    }

    logger.Debug ("Player picked up " + gameObject.name + "!");

    Player.instance.m_equipmentManager.AddItem (gameObject.GetComponent<Item> ());

    Destroy (gameObject);
  }

  void OnTriggerEnter2D (Collider2D other) 
  {
    logger.Debug (gameObject.name + ": " +other.gameObject.name + " entered (OnTriggerEnter2D)");

    if (other.gameObject != Player.instance.gameObject)
      return;

    GuiManager.instance.CreateGlobalPopup ("Pick up " + name +  "?", "", MenuDone);
  }

  void OnTriggerExit2D (Collider2D other) 
  {
    logger.Debug (gameObject.name + ": " +other.gameObject.name + " exited (OnTriggerEnter2D)");
  }


}
