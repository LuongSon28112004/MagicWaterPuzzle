using System.Collections.Generic;
using UnityEngine;

public class BlockTwoSquare : BaseBlock
{
    private void Awake()
    {
        blockType = BlockType.TWO_SQUARE;
        maxCapacity = 4;
        blockID = 11;
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
            float yFix = SnapOdd(c.y);
            float xFix = SnapOdd(c.x);

            filtered.Add(new Vector2(xFix, yFix));
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
        return new Vector3(0, 0, 1);
    }


    // Override Play Particle
    protected override void PlayParticleBlock()
    {
        base.PlayParticleBlock();
        if (blockDirection == Direction.HORIZONTAL)
        {
            if (currentCapacity > 0)
            {
                StartCoroutine(blockParticles[2].PlayParticle());
                StartCoroutine(blockParticles[3].PlayParticle());
            }

            if (currentCapacity > 2)
            {
                StartCoroutine(blockParticles[0].PlayParticle());
                StartCoroutine(blockParticles[1].PlayParticle());
            }
        }

    }


    // override Set Height Water
    protected override float SetHeightWaterFall(DirectionPipe directionPipe, Vector3 pipeTransform)
    {
        if (directionPipe == DirectionPipe.Down)
        {
            return 0.6f;
        }
        else if (directionPipe == DirectionPipe.Left || directionPipe == DirectionPipe.Right)
        {
            if (transform.position.y < pipeTransform.y)
            {
                return 0.6f;
            }
            else if (transform.position.y == pipeTransform.y)
            {
                return 0.6f;
            }
            else return 0.2f;
        }


        return 0.6f;
    }




    //override Set SnapToPipe
    protected override Vector3 SnapToPipe(Vector3 pos, WaterPipe waterPipe)
    {
        Vector3 posSnap = pos;
        if (waterPipe.DirectionPipe == DirectionPipe.Up || waterPipe.DirectionPipe == DirectionPipe.Down)
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
        else if (waterPipe.DirectionPipe == DirectionPipe.Left || waterPipe.DirectionPipe == DirectionPipe.Right)
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
        return posSnap;
    }
}
