using UnityEngine;
using System.Collections;

public class PickableItem : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  private void Pickup ()
  {
    logger.Debug ("Player picked up " + gameObject.name + "!");

    Player.instance.m_equipmentManager.AddItem (gameObject.GetComponent<Item> ());

    Destroy (gameObject);
  }

  public void OnMenuDone (StickMenu.SelectionState state, int item, string itemName)
  {
    if (itemName == "Yes")
      Pickup ();
    else
      logger.Debug ("Player discarded " + gameObject.name + "...");
  }

  void OnTriggerEnter2D (Collider2D other) 
  {
    logger.Debug (gameObject.name + ": " +other.gameObject.name + " entered (OnTriggerEnter2D)");

    GuiManager.instance.CreatePopupMenuHorizontal ("Pick?", "Pick up " + name +  "?", new string[] {"Yes", "No"}, 0, OnMenuDone);
  }

  void OnTriggerExit2D (Collider2D other) 
  {
    logger.Debug (gameObject.name + ": " +other.gameObject.name + " exited (OnTriggerEnter2D)");
  }


}
