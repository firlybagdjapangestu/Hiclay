using System.Collections;
using UnityEngine;


public class MechaInteractable : MonoBehaviour, IInteractable
{
    private ChangeForm changeForm;
    [SerializeField] private GameObject VFX;
    [SerializeField] private AnimationClip enterAnimation;
    private ObjectPoolManager objectPoolManager;

    private void Start()
    {
        objectPoolManager = Object.FindFirstObjectByType<ObjectPoolManager>();
        changeForm = Object.FindFirstObjectByType<ChangeForm>();
    }

    public string GetInteractPrompt()
    {
        return "Press E to Enter Mecha!";
    }

    public void Interact()
    {
        if (changeForm != null)
        {
            GameObject vfxGO = objectPoolManager.ActiveObject(VFX, transform.position, Quaternion.identity);

            // 🔥 Aktifkan animasinya (kalau pakai VFXController)
            var vfx = vfxGO.GetComponent<VFXController>();
            if (vfx != null)
            {
                vfx.PlayAnimation(enterAnimation.name); // Ganti nama animasi sesuai kebutuhan kamu
                StartCoroutine(WaitForAnimationAndTransform(vfx));
            }
            else
            {
                changeForm.TransformToMecha(); // Langsung transformasi tanpa animasi
                gameObject.SetActive(false); // Nonaktifkan MechaInteractable setelah interaksi
            }
        }
        else
        {
            Debug.LogWarning("PlayerStatus belum diset di MechaInteractable!");
        }
    }

    private IEnumerator WaitForAnimationAndTransform(VFXController vfx)
    {
        yield return new WaitForSeconds(enterAnimation.length); // Tunggu durasi animasi selesai
        changeForm.TransformToMecha();
        gameObject.SetActive(false); // Nonaktifkan MechaInteractable setelah interaksi
    }
}
