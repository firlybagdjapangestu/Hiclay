using UnityEngine;

[CreateAssetMenu(menuName = "EnemyBehaviour/Chase/New Chase")]
public class EnemyDefaultChase : BaseChaseData
{
    [SerializeField] private float speed;
    [SerializeField] private float minDistance;
    [SerializeField] private float retreatSpeedMultiplier; // Bisa lebih lambat mundur

    public override void Chase(Transform origin, Transform player)
    {
        if (origin == null || player == null) return;

        float distanceX = player.position.x - origin.position.x;
        float absDistanceX = Mathf.Abs(distanceX);
        float direction = Mathf.Sign(distanceX);

        Vector3 move;
        if (absDistanceX < minDistance)
        {
            // Mundur dari player
            move = new Vector3(-direction, 0f, 0f);
            origin.position += move * speed * retreatSpeedMultiplier * Time.deltaTime;
        }
        else
        {
            // Kejar player
            move = new Vector3(direction, 0f, 0f);
            origin.position += move * speed * Time.deltaTime;
        }
    }
}
