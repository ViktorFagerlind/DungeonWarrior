using UnityEngine;
using System.Collections;

public class EnemyStatusDisplayer : MonoBehaviour 
{
  private GameObject m_currentEnemyWithStatus = null;

  // Singleton
  private static EnemyStatusDisplayer m_instance;
  public  static EnemyStatusDisplayer instance {get { return m_instance;}}

  void Awake ()
  {
    m_instance = this;
  }

  public void OnEnemyStateChange (AnimationState oldState, AnimationState newState)
  {
    if (newState == AnimationState.Death || newState == AnimationState.LieDead)
      DeRegisterEnemy ();
  }

  private void RegisterEnemy (GameObject enemyGameObj)
  {
    m_currentEnemyWithStatus = enemyGameObj;
    m_currentEnemyWithStatus.AddComponent<StatusDisplay>();
    m_currentEnemyWithStatus.GetComponent<Character> ().m_characterAnims.m_onStateChangeDelegate += OnEnemyStateChange;
  }

  private void DeRegisterEnemy ()
  {
    if (m_currentEnemyWithStatus == null)
      return;

    m_currentEnemyWithStatus.GetComponent<Character> ().m_characterAnims.m_onStateChangeDelegate -= OnEnemyStateChange;

    Destroy (m_currentEnemyWithStatus.GetComponent<StatusDisplay> ());
  }
  
  public void SetEnemyToDisplay (GameObject enemyGameObj)
  {
    if (m_currentEnemyWithStatus == enemyGameObj)
      return;
    
    DeRegisterEnemy ();
    RegisterEnemy (enemyGameObj);
  }


}
