using System;
using TMPro;
using Unity.Android.Gradle;
using UnityEngine;

public class BlockIce : MonoBehaviour
{
    [Header("BlockIce Component")]
    [SerializeField] private bool isActive;
    [SerializeField] private TextMeshProUGUI textCountIce;

    public bool IsActive { get => isActive; set => isActive = value; }

    public void ActiveIce(int count, Direction blockDirection)
    {
        this.isActive = true;
        this.transform.gameObject.SetActive(true);
        textCountIce.text = count.ToString();
        SetDirText(blockDirection);
    }
    public void InActiveIce()
    {
        this.isActive = false;
        this.transform.gameObject.SetActive(false);
    }

    private void SetDirText(Direction blockDirection)
    {
        if (blockDirection == Direction.VERTICAL)
        {
            // Vector3 newPosition = textCountIce.transform.position;
            // newPosition.y -= 0.5f;
            //textCountIce.transform.position = newPosition;
        }
        else if (blockDirection == Direction.HORIZONTAL)
        {
            // Vector3 newPosition = textCountIce.transform.position;
            // newPosition.x -= 0.5f;
            // textCountIce.transform.position = newPosition;
            textCountIce.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        Vector3 pos = textCountIce.transform.position;
        pos.z = -1f;
        textCountIce.transform.position = pos;
    }



}
