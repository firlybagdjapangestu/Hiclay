using Unity.Cinemachine;
using UnityEngine;

public class ChangeForm : MonoBehaviour
{
    public GameObject mechaForm;
    public GameObject humanForm;
    public CinemachineCamera cinemachineCamera;

    public void TransformToMecha()
    {
        if (mechaForm != null && humanForm != null)
        {
            mechaForm.transform.position = humanForm.transform.position;

            mechaForm.SetActive(true);
            humanForm.SetActive(false);

            // Hanya ubah kamera jika zone tidak aktif
            if (!ZoneManager.IsZoneActive)
            {
                cinemachineCamera.Follow = mechaForm.transform;
                cinemachineCamera.LookAt = mechaForm.transform;
            }

            Debug.Log("Beralih ke Mecha 💥");
        }
        else
        {
            Debug.LogWarning("Form belum diset di inspector!");
        }
    }

    public void TransformToHuman()
    {
        if (mechaForm != null && humanForm != null)
        {
            humanForm.transform.position = mechaForm.transform.position;

            mechaForm.SetActive(false);
            humanForm.SetActive(true);

            if (!ZoneManager.IsZoneActive)
            {
                cinemachineCamera.Follow = humanForm.transform;
                cinemachineCamera.LookAt = humanForm.transform;
            }

            Debug.Log("Beralih ke Human 🧍");
        }
        else
        {
            Debug.LogWarning("Form belum diset di inspector!");
        }
    }
}
