using UnityEngine;

public class BlockLock : MonoBehaviour
{
    [Header("BlockLock Infor")]
    [SerializeField] bool IsActive = false;

    [SerializeField] BlockLockMoveDir blockLockMoveDir;
    public void ActiveLockLock(MoveDir moveDir, Direction blockMoveDirection, bool isNormal)
    {
        transform.gameObject.SetActive(true);
        IsActive = true;
        blockLockMoveDir.InitMoveDirection(moveDir, blockMoveDirection, isNormal);
    }
}
