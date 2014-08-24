using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiManager_depricated : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

//  public float          m_buttonWidth        = 80;
//  public float          m_buttonHeight       = 60;
//  private Texture2D     m_OK;
//  private Texture2D     m_NOK;

  public delegate void OnMenuDone (StickMenu.SelectionState state, int item, string itemName);
  OnMenuDone   m_onMenuDoneDelegate;
  // -------------------------------------------------------------------------------------------------------------------

  private GUISkin       m_menuSkin = null;

  StickMenu             m_menu = null;
  

  // -------------------------------------------------------------------------------------------------------------------
  
//  public Texture2D buttonOkTexture  {get { return m_OK;}}
//  public Texture2D buttonNokTexture {get { return m_NOK;}}

  // Singleton
  private static GuiManager_depricated m_instance;
  public  static GuiManager_depricated instance {get { return m_instance;}}
  
  // -------------------------------------------------------------------------------------------------------------------
  
  void Awake()
  {
    m_menuSkin    = (GUISkin)Resources.Load ("Graphics and prefabs/GUI/MenuSkin");
    
    m_instance = this;
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  void Start () 
  {
    // m_OK  = Resources.Load ("Graphics and prefabs/GUI/GreenOK")  as Texture2D;
    // m_NOK = Resources.Load ("Graphics and prefabs/GUI/RedNOK")   as Texture2D;
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  public void CreatePopupMenuHorizontal (string title, string description, GUIContent[] listItems, int selectedItem, OnMenuDone onItemSelectedDelegate)
  {
    // Do not create multiple menues
    if (m_menu != null)
      return;

    m_onMenuDoneDelegate = onItemSelectedDelegate;

    m_menu = new StickMenu (title, description, listItems, selectedItem, new Vector2 (100, 40), listItems.Length);

    Player.instance.InputEnabled = false;
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  public void CreatePopupMenuVertical (string title, string description, GUIContent[] listItems, int selectedItem, OnMenuDone onItemSelectedDelegate)
  {
    // Do not create multiple menues
    if (m_menu != null)
      return;
    
    m_onMenuDoneDelegate = onItemSelectedDelegate;
    
    m_menu = new StickMenu (title, description, listItems, selectedItem, new Vector2 (200, 40), 1);
    
    Player.instance.InputEnabled = false;
  }
  
  // -------------------------------------------------------------------------------------------------------------------
  
  void Update ()
  {
    if (m_menu == null)
    {
      Player.instance.InputEnabled = true;
      return;
    }

    StickMenu.SelectionState selectState = m_menu.CheckForSelection ();
    if (selectState != StickMenu.SelectionState.Choosing) 
    {
      if (selectState == StickMenu.SelectionState.SelectedItem) 
        m_onMenuDoneDelegate (selectState, m_menu.SelectedItem, m_menu.SelectedItemName);
      else
        m_onMenuDoneDelegate (selectState, 0, "");

      m_menu = null;
    }
  }
  
  // -------------------------------------------------------------------------------------------------------------------

  void OnGUI () 
  {
    if (m_menu != null)
    {
      GUI.skin = m_menuSkin;
      m_menu.Display ();
    }
  }

  // -------------------------------------------------------------------------------------------------------------------

}
