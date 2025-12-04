using System;
using System.Collections;
using DG.Tweening;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public enum TypeCollectBooster
{
    NONE,
    FREEZE,
    BOMB,
    HAMMER
}

public class CollectBooster : MonoBehaviour
{
    [Header("Collect Booster Config")]
    [SerializeField] TypeCollectBooster typeCollectBooster;
    [SerializeField] Button ClaimButton;
    [Header("Image")]
    [SerializeField] Image Freeze;
    [SerializeField] Image Bomb;
    [SerializeField] Image Hammer;
    [Header("Image SRC")]
    [SerializeField] Sprite FreezeImage;
    [SerializeField] Sprite BombImage;
    [SerializeField] Sprite HammerImage;
    [Header("Spanwer")]
    [SerializeField] Transform ParentSpawner;
    [SerializeField] Transform TargetIcon;
    [SerializeField] Transform TargetFreeze;
    [SerializeField] Transform TargetBomb;
    [SerializeField] Transform TargetHammer;
    [SerializeField] public Action<bool> DoneAction;

    public TypeCollectBooster TypeCollectBooster { get => typeCollectBooster; set => typeCollectBooster = value; }

    private void AddEventListener()
    {
        ClaimButton.onClick.AddListener(ClaimClick);
    }

    private void ClaimClick()
    {
        StartCoroutine(ShowAnimClaim());
    }

    private IEnumerator ShowAnimClaim()
    {

        Sprite Choice = ChoiceSprite();
        Transform TargetChoice = ChoiceTarget();
        if (TargetChoice == null) yield break;
        GameObject booster = new GameObject("Booster");
        booster.transform.SetParent(ParentSpawner, false);
        booster.transform.position = TargetIcon.position;
        Image img = booster.AddComponent<Image>();
        img.sprite = Choice;
        img.SetNativeSize();

        booster.transform.DOMove(TargetChoice.position, 0.4f).OnComplete(() =>
        {
            Destroy(booster);
            DoneAction.Invoke(true);
            AudioManager.Instance.PlayOneShot("CollectBooster", 1f);
        });
        transform.DOScale(Vector3.zero, 0.4f);
    }

    private Transform ChoiceTarget()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                return TargetBomb;
            case TypeCollectBooster.FREEZE:
                return TargetFreeze;
            case TypeCollectBooster.HAMMER:
                return TargetHammer;
            default:
                return null;

        }
    }

    private Sprite ChoiceSprite()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                return BombImage;
            case TypeCollectBooster.FREEZE:
                return FreezeImage;
            case TypeCollectBooster.HAMMER:
                return HammerImage;
            default:
                return BombImage;

        }
    }

    public void setTypeCollect(TypeCollectBooster typeCollectBooster)
    {
        this.typeCollectBooster = typeCollectBooster;
        ChangeIcon();
        AddEventListener();
    }

    private void ChangeIcon()
    {
        switch (typeCollectBooster)
        {

            case TypeCollectBooster.BOMB:
                Bomb.gameObject.SetActive(true);
                break;
            case TypeCollectBooster.FREEZE:
                Freeze.gameObject.SetActive(true);
                break;
            case TypeCollectBooster.HAMMER:
                Hammer.gameObject.SetActive(true);
                break;

        }
    }
}
