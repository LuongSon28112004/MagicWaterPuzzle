using System;
using System.Collections;
using UnityEngine;

public class PipeKeyLock : MonoBehaviour
{
    [Header("Key Lock Settings")]
    [SerializeField] bool isLocked = false;
    [SerializeField] GameObject keyObject;
    [SerializeField] private GameObject GateObject;
    [SerializeField] private GameObject lockObject;
    [SerializeField] private GameObject LockIcon;
    [SerializeField] MeshRenderer LockBase;

    [SerializeField] KeyInfor keyInfor;
    [SerializeField] MeshRenderer keyMeshRenderer;

    public PipeKeyLock(bool isLocked)
    {
        this.isLocked = isLocked;
    }

    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public KeyInfor KeyInfor { get => keyInfor; set => keyInfor = value; }
    public GameObject LockObject { get => lockObject; set => lockObject = value; }

    public void InitKeyLock(KeyInfor colorKey)
    {
        if (!colorKey.IsKey)
            return;
        transform.gameObject.SetActive(true);
        isLocked = true;
        this.KeyInfor = colorKey;
        ColorMaterialCongig colorMat = Contacts.GetColorMat(colorKey.ColorKey);
        if (colorMat != null && keyMeshRenderer != null)
        {
            keyMeshRenderer.material = colorMat.Frame_01;
            LockBase.material = colorMat.Frame_01;
        }
    }

    public void UnlockKeyLock()
    {
        if (isLocked)
        {
            isLocked = false;
            // play unlock animation or effect here
            // ...
            // disable key object
            keyObject.SetActive(false);
            StartCoroutine(ShowAnimClearPipe());

        }
    }

    private IEnumerator ShowAnimClearPipe()
    {
        yield return new WaitForSeconds(0.5f);
        // play Sound
        AudioManager.Instance.PlayOneShot("Metal Gate Open", 1f);
        GameObject effectPrefab = Resources.Load<GameObject>("Particles/PipeClearEffect");
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Vector3 posEffect = effect.transform.position;
            posEffect.z -= 3f;
            effect.transform.position = posEffect;
            PipeClearEffect pipeClearEffect = effect.GetComponent<PipeClearEffect>();
            if (pipeClearEffect != null)
            {
                pipeClearEffect.PlayParticle();
            }
            //yield return new WaitForSeconds(0.1f);
            GateObject.SetActive(false);
            LockObject.SetActive(false);
            LockIcon.SetActive(false);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}
