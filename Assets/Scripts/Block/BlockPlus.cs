using System.Collections.Generic;
using UnityEngine;

public class BlockPlus : BaseBlock
{
    private void Awake()
    {
        blockType = BlockType.PLUS;
        maxCapacity = 5;
        blockID = 3;
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
                // Y phải chẵn, X phải chăn
                float yFix = Mathf.Round(c.y / 2f) * 2f;
                float xFix = Mathf.Round(c.x / 2f) * 2f;

                filtered.Add(new Vector2(xFix, yFix));
            }
            else
            {
                // X phải chẵn, Y phải lẻ
                float xFix = Mathf.Round(c.x / 2f) * 2f;
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
        blockVisual.blockTypeVariant.InitWater(blockColor, direction);
    }

    private Vector3 DirectionWater(Direction direction, Quaternion rotation)
    {
        return new Vector3(1, 0, 0);
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
        if (directionPipe == DirectionPipe.Down)
        {
            return 1f;
        }
        else
        {
            return 0.25f;
        }
    }

    //override Set SnapToPipe
    protected override Vector3 SnapToPipe(Vector3 pos, WaterPipe waterPipe)
    {
        Vector3 posSnap = pos;
        if (waterPipe.DirectionPipe == DirectionPipe.Up || waterPipe.DirectionPipe == DirectionPipe.Down)
        {
            posSnap.x = waterPipe.transform.position.x;
        }
        else if (waterPipe.DirectionPipe == DirectionPipe.Left || waterPipe.DirectionPipe == DirectionPipe.Right)
        {
            posSnap.y = waterPipe.transform.position.y;
        }
        return posSnap;
    }
}
