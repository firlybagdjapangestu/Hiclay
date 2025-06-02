using System.Collections;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private ObjectPoolManager objectPoolManager;

    private void Awake()
    {
        // Bikin override controller dari animator runtime  
        objectPoolManager = Object.FindAnyObjectByType<ObjectPoolManager>();
    }


    public void PlayAnimation(string animationName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator tidak ditemukan!");
            return;
        }
        animator.Play(animationName);
        StartCoroutine(DeactivateAfterAnimation(1f));
    }

    private IEnumerator DeactivateAfterAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        objectPoolManager.DeactivateObject(gameObject);
    }
}
