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
                Vector3 targetPos = pipeKeyLock.LockObject.transform.position;
                targetPos.z -= 1f;
                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.Append(keyObject.transform.DOMove(targetPos, 0.8f).SetEase(Ease.InOutQuad));
                sequence.Join(keyObject.transform.DORotate(new Vector3(keyObject.transform.rotation.eulerAngles.x, keyObject.transform.rotation.eulerAngles.y, keyObject.transform.rotation.eulerAngles.z + 90f), 1f).SetEase(Ease.InOutQuad));
                sequence.OnComplete(() =>
                {
                    AudioManager.Instance.PlayOneShot("unlock", 1f);
                    keyObject.transform.DORotate(new Vector3(keyObject.transform.rotation.eulerAngles.x + 90f, keyObject.transform.rotation.eulerAngles.y, keyObject.transform.rotation.eulerAngles.z), 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        pipeKeyLock.UnlockKeyLock();
                        keyObject.SetActive(false);
                    });
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