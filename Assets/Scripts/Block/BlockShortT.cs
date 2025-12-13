using System.Collections.Generic;
using UnityEngine;

public class BlockShortT : BaseBlock
{
    private void Awake()
    {
        blockType = BlockType.SHORT_T;
        maxCapacity = 4;
        blockID = 8;
        currentCapacity = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    protected override Vector2 SnapToGrid(Vector2 pos)
    {
        float cellSize = 1f;
        Vector2 origin = Vector2.zero;

        // --- Tạo danh sách các điểm snap có thể ---
        List<Vector2> candidates = new List<Vector2>();

        // Snap thô theo lưới
        float xBase = Mathf.Round((pos.x - origin.x) / cellSize) * cellSize + origin.x;
        float yBase = Mathf.Round((pos.y - origin.y) / cellSize) * cellSize + origin.y;

        // 4 vị trí có thể snap gần nhất
        candidates.Add(new Vector2(xBase, yBase));
        candidates.Add(new Vector2(xBase + cellSize, yBase));
        candidates.Add(new Vector2(xBase - cellSize, yBase));
        candidates.Add(new Vector2(xBase, yBase + cellSize));
        candidates.Add(new Vector2(xBase, yBase - cellSize));

        // --- Lọc theo hướng block ---
        List<Vector2> filtered = new List<Vector2>();

        foreach (var c in candidates)
        {
            if (blockDirection == Direction.HORIZONTAL)
            {
                // X phải chẵn, Y phải lẻ
                float xFix = SnapEven(c.x);
                float yFix = SnapOdd(c.y);

                filtered.Add(new Vector2(xFix, yFix));
            }
            else
            {
                // X phải lẻ , Y phải chẵn
                float xFix = SnapOdd(c.x);
                float yFix = SnapEven(c.y);

                filtered.Add(new Vector2(xFix, yFix));
            }
        }

        // --- Chọn điểm gần nhất ---
        Vector2 best = filtered[0];
        float bestDist = Vector2.Distance(pos, best);

        for (int i = 1; i < filtered.Count; i++)
        {
            float d = Vector2.Distance(pos, filtered[i]);
            if (d < bestDist)
            {
                bestDist = d;
                best = filtered[i];
            }
        }

        return best;
    }

    // override init visual water
    public override void AddVisualWater(BlockColor blockColor)
    {
        base.AddVisualWater(blockColor);
        Vector3 direction = DirectionWater(blockDirection, transform.rotation);
        BlockVisual.blockTypeVariant.InitWater(blockColor, direction);
    }

    private Vector3 DirectionWater(Direction direction, Quaternion rotation)
    {
        if (blockDirection == Direction.HORIZONTAL)
        {
            if (transform.rotation.z == 0)
            {
                return new Vector3(0, 0, -1);
            }
            else
            {
                return new Vector3(0, 0, 1);
            }
        }
        else
        {
            if (transform.rotation.z == 90f || transform.rotation.z == -270f)
            {
                return new Vector3(1, 0, 0);
            }
            else
            {
                return new Vector3(-1, 0, 0);
            }
        }
    }


    // Override Play BlockParticle
    protected override void PlayParticleBlock()
    {
        base.PlayParticleBlock();
        if (currentCapacity == 1)
        {
            StartCoroutine(blockParticles[0].PlayParticle());
        }
        else if (currentCapacity > 1 && currentCapacity < 5)
        {
            StartCoroutine(blockParticles[0].PlayParticle());
            StartCoroutine(blockParticles[1].PlayParticle());
            StartCoroutine(blockParticles[2].PlayParticle());
            StartCoroutine(blockParticles[3].PlayParticle());
        }
        else if (currentCapacity >= 5)
        {
            StartCoroutine(blockParticles[0].PlayParticle());
            StartCoroutine(blockParticles[1].PlayParticle());
            StartCoroutine(blockParticles[2].PlayParticle());
            StartCoroutine(blockParticles[3].PlayParticle());
            StartCoroutine(blockParticles[4].PlayParticle());
        }

    }

    // override Set Height Water Fall
    protected override float SetHeightWaterFall(DirectionPipe directionPipe, Vector3 pipeTransform)
    {
        if (blockDirection == Direction.HORIZONTAL)
        {
            if (transform.rotation.eulerAngles.z == 0)
            {
                if (directionPipe == DirectionPipe.Down)
                {
                    if (transform.position.x < pipeTransform.x)
                    {
                        return 0.25f;
                    }
                    else if (transform.position.x == pipeTransform.x)
                    {
                        return 0.65f;
                    }
                    else
                    {
                        return 0.25f;
                    }
                }
                else if (directionPipe == DirectionPipe.Left || directionPipe == DirectionPipe.Right)
                {
                    return 0.25f;
                }
            }
            else
            {
                if (directionPipe != DirectionPipe.Up)
                {
                    return 0.25f;
                }
            }
        }
        else
        {
            if (transform.rotation.eulerAngles.z == 90f || transform.rotation.eulerAngles.z == -270f)
            {
                if (directionPipe == DirectionPipe.Down)
                {
                    return 1f;
                }
                else if (directionPipe == DirectionPipe.Right)
                {
                    return 0.25f;
                }
                else if (directionPipe == DirectionPipe.Left)
                {
                    if (transform.position.y < pipeTransform.y)
                    {
                        return 1f;
                    }
                    else if (transform.position.y == pipeTransform.y)
                    {
                        return 0.65f;
                    }
                    else
                    {
                        return 0.25f;
                    }
                }
            }
            else if (transform.rotation.eulerAngles.z == -90f || transform.rotation.eulerAngles.z == 270f)
            {
                if (directionPipe == DirectionPipe.Down)
                {
                    return 1f;
                }
                else if (directionPipe == DirectionPipe.Right)
                {
                    return 0.25f;
                }
                else if (directionPipe == DirectionPipe.Left)
                {
                    if (transform.position.y < pipeTransform.y)
                    {
                        return 1f;
                    }
                    else if (transform.position.y == pipeTransform.y)
                    {
                        return 0.65f;
                    }
                    else
                    {
                        return 0.25f;
                    }
                }
            }
        }
        return 1f;
    }

    //override Set SnapToPipe
    protected override Vector3 SnapToPipe(Vector3 pos, WaterPipe waterPipe)
    {
        Vector3 posSnap = pos;
        if (blockDirection == Direction.HORIZONTAL)
        {
            if (transform.rotation.eulerAngles.z == 0)
            {
                if (waterPipe.DirectionPipe == DirectionPipe.Up)
                {
                    posSnap.x = waterPipe.transform.position.x;
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Down)
                {
                    if (pos.x < waterPipe.transform.position.x)
                    {
                        posSnap.x = waterPipe.transform.position.x - 2;
                    }
                    else if (pos.x == waterPipe.transform.position.x)
                    {
                        posSnap.x = waterPipe.transform.position.x;
                    }
                    else
                    {
                        posSnap.x = waterPipe.transform.position.x + 2;
                    }
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Left || waterPipe.DirectionPipe == DirectionPipe.Right)
                {
                    posSnap.y = waterPipe.transform.position.y - 1;
                }
            }
            else
            {
                if (waterPipe.DirectionPipe == DirectionPipe.Down)
                {
                    posSnap.x = waterPipe.transform.position.x;
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Up)
                {
                    if (pos.x < waterPipe.transform.position.x)
                    {
                        posSnap.x = waterPipe.transform.position.x - 2;
                    }
                    else if (pos.x == waterPipe.transform.position.x)
                    {
                        posSnap.x = waterPipe.transform.position.x;
                    }
                    else
                    {
                        posSnap.x = waterPipe.transform.position.x + 2;
                    }
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Left || waterPipe.DirectionPipe == DirectionPipe.Right)
                {
                    posSnap.y = waterPipe.transform.position.y + 1;
                }
            }
        }
        else
        {
            if (transform.rotation.z == 90f || transform.rotation.z == -270f)
            {
                if (waterPipe.DirectionPipe == DirectionPipe.Down)
                {
                    posSnap.x = waterPipe.transform.position.x + 1;
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Up)
                {
                    posSnap.x = waterPipe.transform.position.x + 1;
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Right)
                {
                    posSnap.y = waterPipe.transform.position.y;
                }
                else
                {
                    if (pos.y < waterPipe.transform.position.y)
                    {
                        posSnap.y = waterPipe.transform.position.y - 2;
                    }
                    else if (pos.y == waterPipe.transform.position.y)
                    {
                        posSnap.y = waterPipe.transform.position.y;
                    }
                    else
                    {
                        posSnap.y = waterPipe.transform.position.y + 2;
                    }
                }
            }
            else if (transform.rotation.z == -90f || transform.rotation.z == 270f)
            {
                if (waterPipe.DirectionPipe == DirectionPipe.Down)
                {
                    posSnap.x = waterPipe.transform.position.x - 1;
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Up)
                {
                    posSnap.x = waterPipe.transform.position.x - 1;
                }
                else if (waterPipe.DirectionPipe == DirectionPipe.Right)
                {
                    if (pos.y < waterPipe.transform.position.y)
                    {
                        posSnap.y = waterPipe.transform.position.y - 2;
                    }
                    else if (pos.y == waterPipe.transform.position.y)
                    {
                        posSnap.y = waterPipe.transform.position.y;
                    }
                    else
                    {
                        posSnap.y = waterPipe.transform.position.y + 2;
                    }
                }
                else
                {
                    posSnap.y = waterPipe.transform.position.y;
                }
            }
        }
        return posSnap;
    }
}
