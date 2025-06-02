public static class GameEvents
{
    public static event System.Action OnHit;

    public static void Hit()
    {
        OnHit?.Invoke();
    }
}