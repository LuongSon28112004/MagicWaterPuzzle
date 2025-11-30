using System;
using UnityEngine;

public class BlockMoveDir : MonoBehaviour
{
    [SerializeField] private BlockMoveDirection blockMoveDirection;
    [SerializeField] Transform horizontal;
    [SerializeField] Transform vertical;

    public BlockMoveDirection BlockMoveDirection { get => blockMoveDirection; set => blockMoveDirection = value; }

    public void InitMoveDirection(MoveDir moveDir, Direction blockDirection)
    {
        AsignMoveDir(moveDir);
        SetDirection(blockDirection);

    }

    private void AsignMoveDir(MoveDir moveDir)
    {
        if (moveDir == MoveDir.NORMAL) blockMoveDirection = BlockMoveDirection.NORMAL;
        if (moveDir == MoveDir.HORIZONTAL) blockMoveDirection = BlockMoveDirection.HORIZONTAL;
        if (moveDir == MoveDir.VERTICAL) blockMoveDirection = BlockMoveDirection.VERTICAL;
    }

    private void SetDirection(Direction blockDirection)
    {
        if (blockDirection == Direction.NORMAL || blockDirection == Direction.VERTICAL)
        {
            if (blockMoveDirection == BlockMoveDirection.NORMAL) return;
            if (blockMoveDirection == BlockMoveDirection.HORIZONTAL)
            {
                horizontal.gameObject.SetActive(true);
                vertical.gameObject.SetActive(false);
            }

            if (blockMoveDirection == BlockMoveDirection.VERTICAL)
            {
                horizontal.gameObject.SetActive(false);
                vertical.gameObject.SetActive(true);
            }
        }
        else
        {
            if (blockMoveDirection == BlockMoveDirection.NORMAL) return;
            if (blockMoveDirection == BlockMoveDirection.HORIZONTAL)
            {
                horizontal.gameObject.SetActive(false);
                vertical.gameObject.SetActive(true);
            }

            if (blockMoveDirection == BlockMoveDirection.VERTICAL)
            {
                horizontal.gameObject.SetActive(true);
                vertical.gameObject.SetActive(false);
            }
        }
    }
}
