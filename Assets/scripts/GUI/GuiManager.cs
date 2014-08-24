using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiManager : MonoBehaviour 
{
  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

  // -------------------------------------------------------------------------------------------------------------------

  public delegate void OnButtonPressedDelegate (string context, bool pressedOk);

  OnButtonPressedDelegate m_currentUserDelegate = null;

  // -------------------------------------------------------------------------------------------------------------------
  
  // Singleton
  private static GuiManager m_instance;
  public  static GuiManager instance {get { return m_instance;}}


  public  Transform     m_popupGuiPrefab = null;
  private RectTransform m_popupGui;
  private bool          m_popupGuiIsSub = false; // True if user input etc should not be restored
    
  [HideInInspector] public Canvas        m_canvas;
  [HideInInspector] public EventSystem   m_eventSystem;


  private EquipmentPanelManager m_equipmentPanelManager;

  private bool m_guiUsed;


    // -------------------------------------------------------------------------------------------------------------------
  
  void Awake()
  {
    m_instance = this;

    m_canvas = GetComponent<Canvas> ();

    m_eventSystem = GetComponent<EventSystem> ();

    m_equipmentPanelManager = GetComponentInChildren<EquipmentPanelManager> ();
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  void Start () 
  {
    m_equipmentPanelManager.SetActive (false);
    SetGuiUsed (false);
  }

  // -------------------------------------------------------------------------------------------------------------------

  void Update ()
  {
    bool menuButtonPressed = Input.GetButtonDown ("Menu");

    if (menuButtonPressed && !m_equipmentPanelManager.IsActive () && !m_guiUsed)
    {
      m_equipmentPanelManager.SetActive (true);
      SetGuiUsed (true);
    }
    else if (menuButtonPressed && m_equipmentPanelManager.IsActive ())
    {
      m_equipmentPanelManager.SetActive (false);
      SetGuiUsed (false);
    }

  }

  // -------------------------------------------------------------------------------------------------------------------
  
  void SetGuiUsed (bool guiUsed)
  {
    m_guiUsed                    = guiUsed;
    Player.instance.InputEnabled = !m_guiUsed;
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  void OnButtonPressed (string context, bool pressedOk)
  {
    if (m_currentUserDelegate != null)
      m_currentUserDelegate (context, pressedOk);

    Destroy (m_popupGui.gameObject);

    if (!m_popupGuiIsSub)
      SetGuiUsed (false);
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  private void CreatePopup (bool popupGuiIsSub, string textString, string context, OnButtonPressedDelegate userDelegate)
  {
    m_popupGuiIsSub = popupGuiIsSub;
       
    if (!popupGuiIsSub)
    {
      if (m_guiUsed)
        return;
      
      SetGuiUsed (true);
    }

    m_currentUserDelegate = userDelegate;

    m_popupGui = (RectTransform) Instantiate (m_popupGuiPrefab);
    m_popupGui.parent = m_canvas.transform;
    m_popupGui.anchoredPosition = new Vector2 (0, 0);
    
    Button okButton = m_popupGui.transform.Find ("OK").GetComponent<Button> ();
    okButton.onClick.AddListener (delegate {OnButtonPressed (context, true);});
    
    Button cancelButton = m_popupGui.transform.Find ("Cancel").GetComponent<Button> ();
    cancelButton.onClick.AddListener (delegate {OnButtonPressed (context, false);});
    
    Text text = m_popupGui.transform.Find ("Text").GetComponent<Text> ();
    text.text = textString;

    m_eventSystem.SetSelectedGameObject (okButton.gameObject, new BaseEventData (m_eventSystem));
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  public void CreateSubPopup (string textString, string context, OnButtonPressedDelegate userDelegate)
  {
    CreatePopup (true, textString, context, userDelegate);
  }    

  // -------------------------------------------------------------------------------------------------------------------
  
  public void CreateGlobalPopup (string textString, string context, OnButtonPressedDelegate userDelegate)
  {
    CreatePopup (false, textString, context, userDelegate);
  }
}




