using UnityEngine;

[CreateAssetMenu(menuName = "EnemyBehaviour/Patrol/Default Patrol")]
public class EnemyDefaultPatrol : BasePatrolData
{
    public Vector2 patrolOffset = new Vector2(3f, 0f); // Jarak kanan-kiri
    public float moveSpeed = 2f;

    private Transform origin;
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 targetPoint;

    public override void Initialize(Transform origin)
    {
        this.origin = origin;
        pointA = origin.position;
        pointB = origin.position + (Vector3)patrolOffset;
        targetPoint = pointB;
    }

    public override void Patrol()
    {
        if (origin == null) return;

        origin.position = Vector3.MoveTowards(origin.position, targetPoint, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(origin.position, targetPoint) < 0.1f)
        {
            // Ubah target jika sudah sampai
            targetPoint = targetPoint == pointA ? pointB : pointA;

            // Kalau mau flip visual enemy juga bisa di sini
            Vector3 scale = origin.localScale;
            scale.x = targetPoint == pointB ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            origin.localScale = scale;
        }
    }
}
