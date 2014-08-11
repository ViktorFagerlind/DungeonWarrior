using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameLogger
{
  Stack<string>   m_contextStack;
  private string  m_className;

  public bool LogEnabled {get; set;}

  public static GameLogger GetLogger (System.Type t)
  {
    return new GameLogger (t);
  }

  private GameLogger (System.Type t)
  {
    m_contextStack  = new Stack<string> ();
    m_className     = t.ToString ();

    LogEnabled = true;
  }

  private string getContext ()
  {
    string context = "";

    foreach (string ctx in m_contextStack)
      context += ctx;

    return context;
  }

  private string getTimeString ()
  {
    return "(" + Time.time + ")";
  }

  public void Debug (string message)
  {
    if (!LogEnabled)
      return;

    UnityEngine.Debug.Log (getTimeString () + m_className + "   [" + getContext () + "] - " + message);
  }

  public void Warning (string message)
  {
    if (!LogEnabled)
      return;
    
    UnityEngine.Debug.LogWarning (getTimeString () + m_className + "   [" + getContext () + "] - " + message);
  }

  public void Error (string message)
  {
    if (!LogEnabled)
      return;
    
    UnityEngine.Debug.LogError (getTimeString () + m_className + "   [" + getContext () + "] - " + message);
  }
  public void PushContext (string context)
  {
    m_contextStack.Push (context);
  }
  public void PopContext ()
  {
    m_contextStack.Pop ();
  }
}
