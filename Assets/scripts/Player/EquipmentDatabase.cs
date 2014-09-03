using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EquipmentDatabase : MonoBehaviour
{
  static readonly string EquipmentPath = "Graphics and prefabs/Weapons/";

  // -------------------------------------------------------------------------------------------------------------------
  
  public class Item
  {
    public string m_name;
    public string m_description;

    public Item (string name, string description)
    {
      m_name          = name;
      m_description   = description;
    }

    public Sprite GetSprite ()
    {
      return (Sprite)Resources.Load (EquipmentPath + m_name, typeof (Sprite));
    }
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  // Singleton
  private static EquipmentDatabase m_instance;
  public  static EquipmentDatabase instance {get { return m_instance;}}

  List<Item> m_items;

  // -------------------------------------------------------------------------------------------------------------------
  
  void Awake()
  {
    m_instance = this;
  }

  // -------------------------------------------------------------------------------------------------------------------
  
  void Start()
  {
    m_items = new List<Item> ();

    m_items.Add (new Item ("Grey sword", ""));
    m_items.Add (new Item ("Rusty sword", ""));
    m_items.Add (new Item ("Grey shield", ""));
    m_items.Add (new Item ("Rusty shield", ""));
    m_items.Add (new Item ("Forest man stone", ""));
  }
  
  // -------------------------------------------------------------------------------------------------------------------
  
  public Item FindItem (string name)
  {
    return m_items.Find (item => item.m_name == name);
  }
}
