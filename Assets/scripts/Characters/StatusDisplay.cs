using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AttackableCharacter))]
public class StatusDisplay : MonoBehaviour 
{
  public GUISkin m_barSkin;

  public Vector2    m_position;
  public Vector2    m_size;

  private float     m_gap;
  private Vector2   m_totalSize;

  private Color     m_backgroundColor = Misc.createColor256 (50,50,50);
  private Color     m_lifeColor       = Misc.createColor256 (122,19,19);
  private Color     m_staminaColor    = Misc.createColor256 (22,133,56);

  private AttackableCharacter m_attackableCharacter;
  

	void Start () 
  {
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

    drawBar (m_gap, 2f * m_gap + m_size.y, m_attackableCharacter.m_stamina / m_attackableCharacter.m_maxStamina, m_staminaColor);

    GUI.EndGroup ();
  }
}
