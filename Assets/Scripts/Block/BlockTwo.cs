using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BlockTwo : BaseBlock
{
    private void Awake()
    {
        blockType = BlockType.TWO;
        maxCapacity = 2;
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
                float yFix = SnapEven(c.y);
                float xFix = SnapOdd(c.x);

                filtered.Add(new Vector2(xFix, yFix));
            }
            else
            {
                // X phải chẵn, Y phải lẻ
                float xFix = SnapEven(c.x);
                float yFix = SnapOdd(c.y);

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
        blockVisual.blockTypeVariant.InitWater(blockColor, direction);
    }

    private Vector3 DirectionWater(Direction direction, Quaternion rotation)
    {
        float z = rotation.eulerAngles.z;

        if (direction == Direction.VERTICAL)
        {
            if (Mathf.Abs(z - 180f) < 1f)
                return new Vector3(0, 0, -1);

            if (Mathf.Abs(z - 0f) < 1f || Mathf.Abs(z - 360f) < 1f)
                return new Vector3(0, 0, 1);

            return new Vector3(0, 0, -1);
        }

        if (direction == Direction.HORIZONTAL)
        {
            // 90° → (-1,0,0)
            if (Mathf.Abs(z - 90f) < 1f)
                return new Vector3(1, 0, 0);

            // 270° → (1,0,0)
            if (Mathf.Abs(z - 270f) < 1f)
                return new Vector3(-1, 0, 0);

            // fallback
            return new Vector3(1, 0, 0);
        }

        return new Vector3(1, 0, 0);
    }
}
