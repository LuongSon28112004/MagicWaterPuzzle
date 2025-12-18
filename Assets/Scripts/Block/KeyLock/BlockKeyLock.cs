using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlockKeyLock : MonoBehaviour
{
    [Header("Key Lock Settings")]
    [SerializeField] bool isLocked = false;
    [SerializeField] BlockColor colorKey;
    [SerializeField] GameObject keyObject;
    [SerializeField] MeshRenderer keyMeshRenderer;

    [Header("ref")]
    [SerializeField] ParticleSystem center;
    [SerializeField] ParticleSystem MagicDust;

    public BlockColor ColorKey { get => colorKey; set => colorKey = value; }

    public void InitKeyLock(KeyInfor colorKey)
    {
        if (!colorKey.IsKey)
            return;
        transform.gameObject.SetActive(true);
        isLocked = true;
        this.ColorKey = colorKey.ColorKey;
        ColorMaterialCongig colorMat = Contacts.GetColorMat(colorKey.ColorKey);
        if (colorMat != null && keyMeshRenderer != null)
        {
            keyMeshRenderer.material = colorMat.Frame_01;
        }
        center.Play();
        MagicDust.Play();
    }

    public void UnlockKeyLock(GameObject pipeLockObj)
    {
        if (isLocked)
        {
            PipeKeyLock pipeKeyLock = pipeLockObj.GetComponentInChildren<PipeKeyLock>();
            if (pipeKeyLock != null)
            {
                keyObject.transform.SetParent(null);
                Vector3 targetPos = keyObject.transform.position;
                targetPos.z -= 5f;
                keyObject.transform.position = targetPos;
                keyObject.transform.DOMove(pipeKeyLock.transform.position, 0.8f).SetEase(Ease.InOutQuad).OnComplete(() =>
                {
                    pipeKeyLock.UnlockKeyLock();
                    keyObject.SetActive(false);
                });
            }
            // isLocked = false;
            // // play unlock animation or effect here
            // // ...
            // // disable key object
            // keyObject.SetActive(false);
        }
    }
}