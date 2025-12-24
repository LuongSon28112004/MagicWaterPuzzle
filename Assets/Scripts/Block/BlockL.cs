using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockL : BaseBlock
{
    private void Awake()
    {
        blockType = BlockType.L;
        maxCapacity = 4;
        blockID = 12;
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
                // Y phải chẵn, X phải lẻ
                float yFix = Mathf.Round(c.y / 2f) * 2f + 1;
                float xFix = Mathf.Round(c.x / 2f) * 2f;

                filtered.Add(new Vector2(xFix, yFix));
            }
            else
            {
                // X phải chẵn, Y phải lẻ
                float xFix = Mathf.Round(c.x / 2f) * 2f + 1;
                float yFix = Mathf.Round(c.y / 2f) * 2f;

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
        float z = rotation.eulerAngles.z;

        if (direction == Direction.VERTICAL)
        {
            if (Mathf.Abs(z - 180f) < 1f)
                return new Vector3(0, 0, 1);

            if (Mathf.Abs(z - 0f) < 1f || Mathf.Abs(z - 360f) < 1f)
                return new Vector3(0, 0, -1);

            return new Vector3(0, 0, -1);
        }

        if (direction == Direction.HORIZONTAL)
        {
            // 90° → (-1,0,0)
            if (Mathf.Abs(z - 90f) < 1f)
                return new Vector3(-1, 0, 0);

            // 270° → (1,0,0)
            if (Mathf.Abs(z - 270f) < 1f)
                return new Vector3(1, 0, 0);

            // fallback
            return new Vector3(1, 0, 0);
        }

        return Vector3.zero;
    }

    protected override IEnumerator PlayParticleBlock()
    {
        StartCoroutine(blockParticles[0].PlayParticle());
        StartCoroutine(blockParticles[1].PlayParticle());
        StartCoroutine(blockParticles[2].PlayParticle());
        StartCoroutine(blockParticles[3].PlayParticle());
        // if (blockDirection == Direction.VERTICAL)
        // {
        //     if (transform.rotation.eulerAngles.x == 0 || transform.rotation.eulerAngles.x == 360)
        //     {
        //         if (currentCapacity > 0)
        //         {
        //             StartCoroutine(blockParticles[0].PlayParticle());
        //         }

        //         if (currentCapacity > 1)
        //         {
        //             StartCoroutine(blockParticles[0].PlayParticle());
        //             StartCoroutine(blockParticles[1].PlayParticle());
        //         }

        //         if (currentCapacity > 2)
        //         {
        //             StartCoroutine(blockParticles[0].PlayParticle());
        //             StartCoroutine(blockParticles[1].PlayParticle());
        //             StartCoroutine(blockParticles[2].PlayParticle());
        //             StartCoroutine(blockParticles[3].PlayParticle());
        //         }
        //     }
        // }
        // else
        // {
        //     if (transform.rotation.eulerAngles.z == -90f || transform.rotation.eulerAngles.z == 270f)
        //     {
        //         if (currentCapacity > 0)
        //         {
        //             StartCoroutine(blockParticles[1].PlayParticle());
        //             StartCoroutine(blockParticles[2].PlayParticle());
        //             StartCoroutine(blockParticles[3].PlayParticle());
        //         }
        //         if (currentCapacity > 3)
        //         {
        //             StartCoroutine(blockParticles[0].PlayParticle());
        //             StartCoroutine(blockParticles[1].PlayParticle());
        //             StartCoroutine(blockParticles[2].PlayParticle());
        //             StartCoroutine(blockParticles[3].PlayParticle());
        //         }
        //     }
        //     else if (transform.rotation.eulerAngles.z == 90f || transform.rotation.eulerAngles.z == -270f)
        //     {
        //         if (currentCapacity > 0)
        //         {
        //             StartCoroutine(blockParticles[0].PlayParticle());
        //         }
        //         if (currentCapacity > 1)
        //         {
        //             StartCoroutine(blockParticles[0].PlayParticle());
        //             StartCoroutine(blockParticles[1].PlayParticle());
        //             StartCoroutine(blockParticles[2].PlayParticle());
        //             StartCoroutine(blockParticles[3].PlayParticle());
        //         }
        //     }
        // }

        yield break;

    }


    // Set Height Water
    // override Set Height Water
    protected override float SetHeightWaterFall(DirectionPipe directionPipe, Vector3 pipeTransform)
    {
        float z = transform.rotation.eulerAngles.z;


        // =============== VERTICAL ===============

        if (blockDirection == Direction.VERTICAL)
        {
            // Góc 0° hoặc 360° (thẳng đứng)
            if (ApproxAngle(0f, 1, z))
            {
                if (directionPipe == DirectionPipe.Left)
                {
                    return 0.25f;
                }
                else if (directionPipe == DirectionPipe.Right)
                {
                    if (transform.position.y < pipeTransform.y)
                    {
                        return 1f;
                    }
                    else if (transform.position.y == pipeTransform.y)
                    {
                        return 0.62f;
                    }
                    else
                    {
                        return 0.25f;
                    }
                }
                else if (directionPipe == DirectionPipe.Down)
                {
                    return transform.position.x < pipeTransform.x ? 1f : 0.2f;
                }
            }

            if (ApproxAngle(180, 1, z))
            {
                if (directionPipe == DirectionPipe.Left)
                {
                    if (transform.position.y < pipeTransform.y)
                    {
                        return 1f;
                    }
                    else if (transform.position.y == pipeTransform.y)
                    {
                        return 0.67f;
                    }
                    else
                    {
                        return 0.25f;
                    }
                }
                else if (directionPipe == DirectionPipe.Right)
                {
                    return 0.25f;
                }
                else if (directionPipe == DirectionPipe.Down)
                {
                    return 1f;
                }
            }
        }

        // =============== HORIZONTAL ===============

        else if (blockDirection == Direction.HORIZONTAL)
        {
            // Góc 90° hoặc -270°
            if (ApproxAngle(90f, 1, z))
            {
                if (directionPipe == DirectionPipe.Left)
                {
                    return transform.position.y < pipeTransform.y ? 0.6f : 0.2f;
                }
                else if (directionPipe == DirectionPipe.Right)
                {
                    return 0.25f;
                }
                else if (directionPipe == DirectionPipe.Down)
                {
                    if (transform.position.x < pipeTransform.x)
                    {
                        return 0.25f;
                    }
                    else if (transform.position.x == pipeTransform.x)
                    {
                        return 0.25f;
                    }
                    else
                    {
                        return 0.67f;
                    }
                }
            }
            // Góc -90° hoặc 270°
            else if (ApproxAngle(-90f, 1, z) || ApproxAngle(270f, 1, z))
            {
                if (directionPipe == DirectionPipe.Left)
                {
                    return 0.25f;
                }
                else if (directionPipe == DirectionPipe.Right)
                {
                    return transform.position.y < pipeTransform.y ? 0.67f : 0.25f;
                }
                else if (directionPipe == DirectionPipe.Down)
                {
                    return 0.67f;
                }
            }
        }

        // Default fallback
        return 1f;
    }

    //override Set SnapToPipe
    protected override Vector3 SnapToPipe(Vector3 pos, WaterPipe waterPipe)
    {
        Vector3 posSnap = pos;
        float z = transform.rotation.eulerAngles.z;
        if (waterPipe.DirectionPipe == DirectionPipe.Down)
        {
            if (blockDirection == Direction.VERTICAL)
            {
                if (ApproxAngle(0, 1, z))
                {
                    if (pos.x < waterPipe.transform.position.x)
                    {
                        posSnap.x = waterPipe.transform.position.x - 1;
                    }
                    else
                    {
                        posSnap.x = waterPipe.transform.position.x + 1;
                    }
                }
                if (ApproxAngle(180, 1, 0))
                {
                    posSnap.x = waterPipe.transform.position.x + 1;
                }
            }
            else
            {
                if (ApproxAngle(90, 1, z) || ApproxAngle(-270, 1, z))
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

                if (ApproxAngle(-90, 1, z) || ApproxAngle(270, 1, z))
                {
                    posSnap.x = waterPipe.transform.position.x - 2;
                }

            }
        }
        else if (waterPipe.DirectionPipe == DirectionPipe.Up)
        {
            if (blockDirection == Direction.VERTICAL)
            {
                if (ApproxAngle(0, 1, z))
                {
                    posSnap.x = waterPipe.transform.position.x - 2;
                }
                if (ApproxAngle(180, 1, 0))
                {
                    if (pos.x < waterPipe.transform.position.x)
                    {
                        posSnap.x = waterPipe.transform.position.x - 2;
                    }
                    else
                    {
                        posSnap.x = waterPipe.transform.position.x + 2;
                    }
                }
            }
            else
            {
                if (ApproxAngle(90, 1, z) || ApproxAngle(-270, 1, z))
                {
                    posSnap.x = waterPipe.transform.position.x + 2;
                }

                if (ApproxAngle(-90, 1, z) || ApproxAngle(270, 1, z))
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

            }
        }
        else if (waterPipe.DirectionPipe == DirectionPipe.Left)
        {
            if (blockDirection == Direction.VERTICAL)
            {
                if (ApproxAngle(0, 1, z))
                {
                    posSnap.y = waterPipe.transform.position.y - 2;
                }
                if (ApproxAngle(180, 1, 0))
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
            else
            {
                if (ApproxAngle(90, 1, z) || ApproxAngle(-270, 1, z))
                {
                    if (pos.y < waterPipe.transform.position.y)
                    {
                        posSnap.y = waterPipe.transform.position.y - 1;
                    }
                    else
                    {
                        posSnap.y = waterPipe.transform.position.y + 1;
                    }
                }

                if (ApproxAngle(-90, 1, z) || ApproxAngle(270, 1, z))
                {
                    posSnap.y = waterPipe.transform.position.y + 1;
                }

            }
        }
        else if (waterPipe.DirectionPipe == DirectionPipe.Right)
        {
            if (blockDirection == Direction.VERTICAL)
            {
                if (ApproxAngle(0, 1, z))
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
                if (ApproxAngle(180, 1, z))
                {
                    posSnap.y = waterPipe.transform.position.y + 2;
                }
            }
            else
            {
                if (ApproxAngle(90, 1, z) || ApproxAngle(-270, 1, z))
                {
                    posSnap.y = waterPipe.transform.position.y - 1;
                }

                if (ApproxAngle(-90, 1, z) || ApproxAngle(270, 1, z))
                {
                    if (pos.y < waterPipe.transform.position.y)
                    {
                        posSnap.y = waterPipe.transform.position.y - 1;
                    }
                    else
                    {
                        posSnap.y = waterPipe.transform.position.y + 1;
                    }
                }

            }
        }
        return posSnap;
    }




}
