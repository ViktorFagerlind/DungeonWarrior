using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentPanelManager : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
  public  Transform       m_buttonPrefab = null;

  public float            m_listItemStartY;
  public float            m_listItemSpaceY;
  public Vector2          m_listItemSize;

  public RectTransform    m_weaponItemList;
  public RectTransform    m_shieldItemList;

  private RectTransform   m_weaponPanel;
  private RectTransform   m_shieldPanel;

  private GameObject      m_savedSelection;

  delegate void OnItemPressedDelegate (string name);

  // -------------------------------------------------------------------------------------------------------------------
  
  void Start ()
  {
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  public void OnWeaponSelectionConfirm (string context, bool pressedOk)
  {
    SetGroupInteractable (true);

    if (pressedOk)
      Player.instance.m_equipmentManager.SetWeapon (context);
  }
        
  // -------------------------------------------------------------------------------------------------------------------
  
  public void OnWeaponItemPressed (string name)
  {
    SetGroupInteractable (false);
    GuiManager.instance.CreateSubPopup ("Equip " + name + " as weapon?", name, OnWeaponSelectionConfirm);
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  public void OnShieldSelectionConfirm (string context, bool pressedOk)
  {
    SetGroupInteractable (true);

    if (pressedOk)
      Player.instance.m_equipmentManager.SetShield (context);
  }
  
  // -------------------------------------------------------------------------------------------------------------------
  
  public void OnShieldItemPressed (string name)
  {
    SetGroupInteractable (false);
    GuiManager.instance.CreateSubPopup ("Equip " + name + " as shield?", name, OnShieldSelectionConfirm);
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  private void SetGroupInteractable (bool interactable)
  {
    if (interactable)
    {
      GetComponent<CanvasGroup> ().interactable = true;
      GuiManager.instance.m_eventSystem.SetSelectedGameObject (m_savedSelection, new BaseEventData (GuiManager.instance.m_eventSystem));
    }
    else
    {
      GetComponent<CanvasGroup> ().interactable = false;
      m_savedSelection = GuiManager.instance.m_eventSystem.currentSelectedGameObject;
    }
  }
    
  // -------------------------------------------------------------------------------------------------------------------
  
  public void SetActive (bool active)
  {
    gameObject.SetActive (active);

    Misc.DeleteAllChildren (m_weaponItemList);
    Misc.DeleteAllChildren (m_shieldItemList);

    if (active)
    {
      UpdateItemList (m_weaponItemList, 
                      Player.instance.m_equipmentManager.m_attackWeapons.GetItemArray (),
                      OnWeaponItemPressed);

      UpdateItemList (m_shieldItemList, 
                      Player.instance.m_equipmentManager.m_shilds.GetItemArray (),
                      OnShieldItemPressed);

      // Select the WeaponsButton upon start
      Transform objectToSelect = transform.Find ("ChoisePanel/WeaponButton");
      GuiManager.instance.m_eventSystem.SetSelectedGameObject (objectToSelect.gameObject, new BaseEventData (GuiManager.instance.m_eventSystem));
    }
  }
  
  // -------------------------------------------------------------------------------------------------------------------
  
  public bool IsActive ()
  {
    return gameObject.activeSelf;
  }
  
  // -------------------------------------------------------------------------------------------------------------------
  
  void UpdateItemList (RectTransform itemListRect, string[] itemNames, OnItemPressedDelegate onItemPressedDelegate)
  {
    for (int i=0; i < itemNames.Length; i++)
    {
      string name = itemNames[i];

      RectTransform buttonTransform = (RectTransform) Instantiate (m_buttonPrefab);
      buttonTransform.parent = itemListRect;
      buttonTransform.anchoredPosition = new Vector2 (0, -m_listItemStartY - i*m_listItemSpaceY);
      buttonTransform.sizeDelta        = new Vector2 (m_listItemSize.x, m_listItemSize.y);

      buttonTransform.Find("Text").GetComponent<Text>   ().text   = name;
      buttonTransform.Find("Image").GetComponent<Image> ().sprite = EquipmentDatabase.instance.FindItem (name).GetSprite ();

      Button button = buttonTransform.GetComponent<Button> ();

      button.onClick.AddListener (delegate {onItemPressedDelegate (name);});
    }
    
    float totalSizeY = (itemNames.Length-1) * m_listItemSpaceY + 2 * m_listItemStartY + m_listItemSize.y;
    
    logger.Debug ("totalSizeY: " + totalSizeY + ", itemList.sizeDelta.y: " + itemListRect.sizeDelta.y);

    // Set list size
    if (totalSizeY > itemListRect.sizeDelta.y)
      itemListRect.sizeDelta = new Vector2 (itemListRect.sizeDelta.x, totalSizeY);
    //itemList.anchoredPosition = new Vector2 (itemList.anchoredPosition.x, 0);
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  void Update ()
  {
  }
}

