using System.IO;
using UnityEngine;
using System.Collections;

public class WeaponUser : MonoBehaviour
{
  public string PrefabName;
  public string WeaponAttachName;

  private SpriteRenderer m_attachWeaponSprite;
  private SpriteRenderer m_newWeaponSprite;


  // Use this for initialization
  void Start()
  {
    LoadAndSetWeapon ("Graphics and prefabs/Weapons/" + PrefabName, WeaponAttachName);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  // Update is called once per frame
  void LateUpdate()
  {
    m_newWeaponSprite.renderer.sortingOrder = m_attachWeaponSprite.renderer.sortingOrder;
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void LoadAndSetWeapon (string weaponResource, string attachWeaponName)
  {
    Transform newWeapon = (Transform)Instantiate (Resources.Load (weaponResource, typeof (Transform)));
    newWeapon.gameObject.name = Path.GetFileName (weaponResource);
    
    SetWeapon (newWeapon, attachWeaponName);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void SetWeapon (Transform newWeapon, string attachWeaponName)
  {
    Transform attachWeapon = Misc.getChildByName (transform, attachWeaponName);
    
    m_attachWeaponSprite = attachWeapon.GetComponent<SpriteRenderer> ();
    m_newWeaponSprite    = newWeapon.GetComponent<SpriteRenderer> ();

    m_attachWeaponSprite.enabled = false;
    
    newWeapon.parent = attachWeapon;
    Misc.resetTransform (newWeapon);  
  }
  

}
