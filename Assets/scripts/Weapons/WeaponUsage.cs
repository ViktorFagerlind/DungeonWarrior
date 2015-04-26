using System.IO;
using UnityEngine;
using System.Collections;

public class WeaponUsage : MonoBehaviour
{
  public string m_weaponName;
  public string m_parentName;

  private SpriteRenderer m_weaponSprite;
  private SpriteRenderer m_parentWeaponSprite;

  void Start ()
  {
    LoadAndSetWeapon (m_weaponName);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void LateUpdate ()
  {
    if (m_weaponSprite == null)
      return;

    m_weaponSprite.GetComponent<Renderer>().sortingOrder = m_parentWeaponSprite.GetComponent<Renderer>().sortingOrder;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void ClearWeapon ()
  {
    ClearWeaponsFromParent (m_parentName);
    m_weaponSprite        = null;
    m_parentWeaponSprite  = null;
    m_weaponName = "";
    m_parentName = "";
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void LoadAndSetWeapon (string weaponName)
  {
    Transform newWeapon = (Transform)Instantiate (Resources.Load ("Graphics and prefabs/Weapons/" + weaponName, typeof (Transform)));

    if (newWeapon == null)
    {
      ClearWeapon ();
      return;
    }

    m_weaponName = weaponName;

    newWeapon.gameObject.name = Path.GetFileName (weaponName);
    
    SetWeapon (newWeapon, m_parentName);
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void SetWeapon (Transform weapon, string weaponParentName)
  {
    Transform parentWeaponTransform = Misc.GetChildByName (transform, weaponParentName);
    
    ClearWeaponsFromParent (weaponParentName);
    
    m_weaponSprite        = weapon.GetComponentInChildren<SpriteRenderer> ();
    m_parentWeaponSprite  = parentWeaponTransform.GetComponent<SpriteRenderer> ();
    m_parentWeaponSprite.enabled = false;
    
    weapon.parent = parentWeaponTransform;
    Misc.ResetTransform (weapon);
    
    // Update shield in potential Attackable character
    AttackableCharacter ac = GetComponent<AttackableCharacter> ();
    if (ac != null)
      ac.updateCurrentShield ();
  }
  
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void ClearWeaponsFromParent (string weaponParentName)
  {
    Transform weaponParent = Misc.GetChildByName (transform, weaponParentName);
    
    // Destroy other potential weapons
    for (int i = 0; i<weaponParent.childCount; i++) 
      Destroy (weaponParent.GetChild (i).gameObject);
  }
}
