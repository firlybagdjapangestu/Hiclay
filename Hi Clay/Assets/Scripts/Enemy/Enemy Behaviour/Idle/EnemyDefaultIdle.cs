using UnityEngine;


[CreateAssetMenu(menuName = "EnemyBehaviour/Idle/New Idle")]
public class EnemyDefaultIdle : BaseIdleData
{
    public override void Idle()
    {
        Debug.Log("Enemy is idling.");
    }

    public override void Initialize(Transform origin)
    {
        throw new System.NotImplementedException();
    }
   
}
