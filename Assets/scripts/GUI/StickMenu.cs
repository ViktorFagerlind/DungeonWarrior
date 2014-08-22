/*
 I had Alex Hackl's menu code ( http://wiki.unity3d.com/index.php?title=JoystickButtonMenu ) 
 up as a reference when writing this. But the code in this file is a complete rewrite, 
 so I'll release this code as PUBLIC DOMAIN instead of CC license. Much thanks to Alex 
 for posting his code which made mine much easier to write.
 Enjoy! You can contact me, Erik Hermansen, at info@seespacelabs.com if you like.
*/
using UnityEngine;

public class StickMenu
{
  public enum SelectionState
  { 
    SelectedItem,
    Cancelled,
    Choosing
  }

  public int    SelectedItem     {get {return m_selectedItem;}}
  public string SelectedItemName {get {return m_listItems[m_selectedItem].text;}}

  //Change these to match what you've defined in InputManager.
  private const   string        SELECT_AXIS    = "Vertical";
  private const   string        SELECT_BUTTON  = "Select";
  private const   string        BACK_BUTTON    = "Jump";

  //Input freeze intervals to help the menu control work intuitively.
  private const   float         AXIS_FREEZE_DELAY   = .2f;
  private         float         m_noInputUntil      = -1f;

  private         string        m_title;
  private         string        m_description;

  private         int           m_selectedItem;
  private         GUIContent[]  m_listItems;
  private         Vector2       m_buttonSize;
  private         int           m_xCount;
  private         int           m_yCount;

  //Pass in names to be displayed in menu options.
  public StickMenu (string title, string description, GUIContent[] listItems, int selectedItem, Vector2 buttonSize, int xCount)
  {
    m_listItems   = listItems;
    m_title       = title;
    m_description = description;

    m_buttonSize  = buttonSize;
    m_xCount      = xCount;
    m_yCount      = (listItems.Length + (xCount-1)) / xCount;

    m_selectedItem = selectedItem;
  }

  //Must be called from OnGUI of a MonoBehavior class. 
  public void Display ()
  {
    float labelHeight   = 50;
    float width         = m_xCount * m_buttonSize.x;
    float height        = m_yCount * m_buttonSize.y + labelHeight + GUI.skin.box.border.bottom;

    GUI.BeginGroup (new Rect (Screen.width/2f - width/2f, Screen.height/2f - height/2f, width, height), m_title);

    GUI.Box (new Rect (0, 0, width, height), m_description);
    /*
    GUI.Toolbar (new Rect (GUI.skin.box.border.left, 
                           labelHeight, 
                           width - GUI.skin.box.border.left - GUI.skin.box.border.right, 
                           height - labelHeight - GUI.skin.box.border.bottom), 
                           m_selectedItem, m_itemNames);
                           */
    GUI.SelectionGrid (new Rect (GUI.skin.box.border.left, 
                                 labelHeight, 
                                 width - GUI.skin.box.border.left - GUI.skin.box.border.right, 
                                 height - labelHeight - GUI.skin.box.border.bottom),
                       m_selectedItem, m_listItems, m_xCount);

    GUI.EndGroup ();
  }

  // Call from Update() method of a MonoBehavior object.
  public SelectionState CheckForSelection ()
  {
    float now = Time.realtimeSinceStartup; //Using .realtime instead of .time so that menus are immune to pausing via Time.timeScale = 0.

    if (now < m_noInputUntil)
      return SelectionState.Choosing; //Check for previously clicked option that was animated in down state. 

    if (Input.GetButtonDown(SELECT_BUTTON))
      return SelectionState.SelectedItem; 

    if (Input.GetButtonDown(BACK_BUTTON))
      return SelectionState.Cancelled; 

    //Check for up/down menu movement. 
    float axisValue = Input.GetAxis(SELECT_AXIS); 
    if (axisValue < -.1f)
    {
      if (m_selectedItem == m_listItems.Length - 1)
        m_selectedItem = 0;
      else
        m_selectedItem++;

      m_noInputUntil = now + AXIS_FREEZE_DELAY;
    } 
    else if (axisValue > .1f)
    {
      if (m_selectedItem == 0)
        m_selectedItem = m_listItems.Length-1;
      else
        m_selectedItem--;

      m_noInputUntil = now + AXIS_FREEZE_DELAY;
    }

    return SelectionState.Choosing;
  }

  //Returns current selection of menu. In lower case for use in 
  //case-insensitive comparisons.
  public string Selection()
  {
    return m_listItems [m_selectedItem].text.ToLower();
  }

}
