using System.IO;
using UnityEngine;
using System.Collections;

public class Weapons : MonoBehaviour
{
  public string rightHandWeapon;
  public string leftHandWeapon;

  // Use this for initialization
  void Start()
  {
    LoadAndSetWeapon ("Graphics and prefabs/Weapons/" + rightHandWeapon, "Right-hand-weapon");
    LoadAndSetWeapon ("Graphics and prefabs/Weapons/" + leftHandWeapon, "Left-hand-weapon");
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  // Update is called once per frame
  void Update()
  {
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
    
    attachWeapon.GetComponent<SpriteRenderer> ().enabled = false;
    
    newWeapon.parent = attachWeapon;
    Misc.resetTransform (newWeapon);  
  }
  

}
