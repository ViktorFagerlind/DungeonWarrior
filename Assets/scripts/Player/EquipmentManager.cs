using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class EquipmentManager : MonoBehaviour
{
  // ---------------------------------------------------------------------------------------------------------------------------------
  
  [System.Serializable]
  public class EquipmentCollection
  {
    private string        m_selectedItem = "";

    public  List<string>  m_items;
    public  string        m_parentName;

    public  bool      IsEmpty            {get {return m_items.Count == 0;}}
    public  int       GetSelectedItemIndex () {return m_items.IndexOf (m_selectedItem);}
    public  string    GetSelectedItem ()      {return m_selectedItem;}

    public static GUIContent NameToGuiItem (string itemName)
    {
      Sprite sprite = (Sprite)Resources.Load ("Graphics and prefabs/Weapons/" + itemName, typeof (Sprite));

      return new GUIContent (itemName, sprite.texture);
    }

    public GUIContent[] GetGuiItemArray ()      
    {
      return Array.ConvertAll (m_items.ToArray (), new Converter<string, GUIContent>(NameToGuiItem));
    }

    public void RemoveItem (string item)
    {
      m_items.Remove (item);

      if (item == m_selectedItem)
        m_selectedItem    = "";  
    }

    public void AddItem (string item)
    {
      if (m_items.Contains (item))
        return;

      m_items.Add (item);
    }
    
    public void SelectFirst ()
    {
      if (m_items.Count == 0)
        return;

      m_selectedItem = m_items[0];
    }

    public void SelectItem (string item)
    {
      if (!m_items.Contains (item))
        return;

      m_selectedItem = item;
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------

  private static readonly GameLogger logger = GameLogger.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
  
  public EquipmentCollection m_attackWeapons;
  public EquipmentCollection m_shilds;

  private WeaponUsage m_primaryWeaponUsage;
  private WeaponUsage m_secondaryWeaponUsage;

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void AddItem (Item item)
  {
    if (item is AttackWeapon)
      m_attackWeapons.m_items.Add (item.gameObject.name);
    else if (item is Shield)
      m_shilds.m_items.Add (item.gameObject.name);
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  private void UpdateWeapon (EquipmentCollection collection, WeaponUsage weaponUsage)
  {
    if (collection.IsEmpty)
      weaponUsage.ClearWeapon ();
    else
      weaponUsage.LoadAndSetWeapon (collection.GetSelectedItem ());
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Awake ()
  {
    m_attackWeapons.SelectFirst ();
    m_shilds.SelectFirst ();
    
    m_primaryWeaponUsage = gameObject.AddComponent<WeaponUsage> ();
    m_primaryWeaponUsage.m_parentName = "Right-hand-weapon";
    m_primaryWeaponUsage.m_weaponName = m_attackWeapons.GetSelectedItem ();

    m_secondaryWeaponUsage = gameObject.AddComponent<WeaponUsage> ();
    m_secondaryWeaponUsage.m_parentName = "Left-hand-weapon";
    m_secondaryWeaponUsage.m_weaponName = m_shilds.GetSelectedItem ();
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Start ()
  {
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  public void OnMenuDone (StickMenu.SelectionState state, int item, string itemName)
  {
    if (state == StickMenu.SelectionState.SelectedItem)
    {
      if (itemName == m_attackWeapons.GetSelectedItem ())
        return;

      m_attackWeapons.SelectItem (itemName);
      UpdateWeapon (m_attackWeapons, m_primaryWeaponUsage);
    }
  }

  // ---------------------------------------------------------------------------------------------------------------------------------
  
  void Update ()
  {
    if (Input.GetButtonDown ("Menu"))
    {
      logger.Debug ("Create gui...");
      GuiManager.instance.CreatePopupMenuVertical ("Weapon selection", "Select weapon", 
                                                   m_attackWeapons.GetGuiItemArray (), m_attackWeapons.GetSelectedItemIndex (), 
                                                   OnMenuDone);
    }
  }
}
