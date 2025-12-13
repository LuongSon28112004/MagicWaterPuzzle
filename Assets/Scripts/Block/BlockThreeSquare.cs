using System.Collections.Generic;
using UnityEngine;

public class BlockThreeSquare : BaseBlock
{
    private void Awake()
    {
        blockType = BlockType.THREE_SQUARE;
        maxCapacity = 9;
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
            float yFix = Mathf.Round(c.y / 2f) * 2f;
            float xFix = Mathf.Round(c.x / 2f) * 2f;

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
        BlockVisual.blockTypeVariant.InitWater(blockColor, direction);
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
                StartCoroutine(blockParticles[0].PlayParticle());
                StartCoroutine(blockParticles[1].PlayParticle());
                StartCoroutine(blockParticles[2].PlayParticle());
            }

            if (currentCapacity > 3)
            {
                StartCoroutine(blockParticles[3].PlayParticle());
                StartCoroutine(blockParticles[4].PlayParticle());
                StartCoroutine(blockParticles[5].PlayParticle());
            }
            if (currentCapacity > 6)
            {
                StartCoroutine(blockParticles[6].PlayParticle());
                StartCoroutine(blockParticles[7].PlayParticle());
                StartCoroutine(blockParticles[8].PlayParticle());
            }
        }

    }


    // override Set Height Water
    protected override float SetHeightWaterFall(DirectionPipe directionPipe, Vector3 pipeTransform)
    {
        if (directionPipe == DirectionPipe.Down)
        {
            return 1f;
        }
        else if (directionPipe == DirectionPipe.Left || directionPipe == DirectionPipe.Right)
        {
            if (transform.position.y < pipeTransform.y)
            {
                return 1f;
            }
            else if (transform.position.y == pipeTransform.y)
            {
                return 0.65f;
            }
            else return 0.25f;
        }


        return 1f;
    }




    //override Set SnapToPipe
    protected override Vector3 SnapToPipe(Vector3 pos, WaterPipe waterPipe)
    {
        Vector3 posSnap = pos;
        if (waterPipe.DirectionPipe == DirectionPipe.Up || waterPipe.DirectionPipe == DirectionPipe.Down)
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
        return posSnap;
    }
}
