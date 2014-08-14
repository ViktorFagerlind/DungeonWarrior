using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AttackableCharacter))]
public class StatusDisplay : MonoBehaviour 
{
  private GUISkin   m_barSkin;

  private Vector2   m_position;
  private Vector2   m_size;

  private float     m_gap;
  private Vector2   m_totalSize;

  private Color     m_backgroundColor = Misc.createColor256 (50,50,50);
  private Color     m_lifeColor       = Misc.createColor256 (122,19,19);
  private Color     m_staminaColorOk  = Misc.createColor256 (22,133,56);
  private Color     m_staminaColorNok = Misc.createColor256 (133,133,36);

  private AttackableCharacter m_attackableCharacter;


  void Awake ()
  {
    m_size = new Vector2 (400, 8);

    if (gameObject == Player.instance.gameObject)
      m_position = new Vector2 (30, 20);
    else
      m_position = new Vector2 (Screen.width - m_size.x - 30, 20);
  }

	void Start () 
  {
    m_barSkin = (GUISkin)Resources.Load ("Graphics and prefabs/Characters/StatusBar/StatusBarSkin");

    m_attackableCharacter = GetComponent<AttackableCharacter> ();

    m_gap       = m_size.y / 3f;
    m_totalSize = new Vector2 (m_size.x + 2f * m_gap, 2f * m_size.y + 3f * m_gap);
	}
	
  void drawBar (float px, float py, float percent, Color color)
  {
    GUI.backgroundColor = m_backgroundColor;
    GUI.Box (new Rect (px, py, m_size.x, m_size.y), "");

    if (percent > 0f)
    {
      GUI.backgroundColor = color;
      GUI.Box (new Rect (px, py, m_size.x * percent, m_size.y), "");
    }
  }

	void OnGUI () 
  {
    GUI.skin = m_barSkin;

    GUI.BeginGroup (new Rect (m_position.x, m_position.y, m_totalSize.x, m_totalSize.y));

    drawBar (m_gap, m_gap, m_attackableCharacter.m_health / m_attackableCharacter.m_maxHealth, m_lifeColor);

    Color m_staminaColor = (m_attackableCharacter.m_stamina.LimitPercentage == 1f) ? m_staminaColorOk : m_staminaColorNok;
    drawBar (m_gap, 2f * m_gap + m_size.y, m_attackableCharacter.m_stamina.Percentage, m_staminaColor);

    GUI.EndGroup ();
  }
}
