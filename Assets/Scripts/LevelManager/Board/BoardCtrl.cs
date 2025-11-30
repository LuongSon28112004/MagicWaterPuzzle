using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardCtrl : MonoBehaviour
{
    [Header("BoardCtrl Components")]
    [SerializeField] private LevelData levelData;
    // Parents for instantiated objects
    [SerializeField] private GameObject GridSlotParent;
    [SerializeField] private GameObject BlockParent;
    [SerializeField] private GameObject GateParent;
    //data Board
    [SerializeField] private List<Transform> gridSlotInstances = new List<Transform>();
    [SerializeField] private List<Transform> gateInstances = new List<Transform>();
    [SerializeField] private List<Transform> blockInstances = new List<Transform>();

    public List<Transform> BlockInstances { get => blockInstances; set => blockInstances = value; }


    // Method to load level based on levelData
    public void LoadLevel(LevelData levelData)
    {
        this.levelData = levelData;

        CreateObjects();
        StartCoroutine(ScaleObjects(gridSlotInstances, gateInstances, blockInstances));

    }

    private void CreateObjects()
    {
        List<GameObject> gridSlotList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Grids"));
        List<GameObject> blockList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Blocks"));
        List<GameObject> gateList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Gates"));
        // Load tiles
        foreach (var tile in levelData.slotHolders)
        {
            GameObject gameObject = null;
            if (tile.name.Contains("GridSlot"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "GridSlot"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            else if (tile.name.Contains("InnerCornerGridBorder"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "InnerCornerGridBorder"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            else if (tile.name.Contains("OuterCornerGridBorder"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "OuterCornerGridBorder"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            else if (tile.name.Contains("StraightGridBorder"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "StraightGridBorder"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            gameObject.transform.localScale = Vector3.zero;
            gridSlotInstances.Add(gameObject.transform);
        }

        //Load blocks

        foreach (var block in levelData.blocks)
        {
            GameObject gameObject = null;
            if (block.name.Contains("Two"))
            {
                gameObject = Instantiate(blockList.Find(x => x.name == "Two"), block.position, Quaternion.Euler(block.rotation), BlockParent.transform);

                BlockTwo blockTwo = gameObject.GetComponent<BlockTwo>();
                blockTwo.AddVisualColor(block.color);
                if (block.rotation != new Vector3(0, 0, 0) && block.rotation != new Vector3(0, 0, 180) && block.rotation != new Vector3(0, 0, -180) && block.rotation != new Vector3(0, 0, 360))
                {
                    blockTwo.BlockDirection = Direction.HORIZONTAL;
                }
                else
                {
                    blockTwo.BlockDirection = Direction.VERTICAL;
                }
                blockTwo.AddVisualWater(block.color);
                blockTwo.AddMoveDirection(block.moveDir);
            }
            else if (block.name.Contains("One"))
            {
                gameObject = Instantiate(blockList.Find(x => x.name == "One"), block.position, Quaternion.Euler(block.rotation), BlockParent.transform);

                BlockOne blockOne = gameObject.GetComponent<BlockOne>();
                blockOne.BlockDirection = Direction.NORMAL;
                blockOne.AddVisualColor(block.color);
                blockOne.AddMoveDirection(block.moveDir);
            }
            else if (block.name.Contains("Plus"))
            {
                gameObject = Instantiate(blockList.Find(x => x.name == "Plus"), block.position, Quaternion.Euler(block.rotation), BlockParent.transform);

                BlockPlus blockPlus = gameObject.GetComponent<BlockPlus>();
                blockPlus.AddVisualColor(block.color);
                blockPlus.BlockDirection = Direction.NORMAL;
                blockPlus.AddVisualWater(block.color);
                blockPlus.AddMoveDirection(block.moveDir);
            }
            else if (block.name.Contains("L"))
            {
                gameObject = Instantiate(blockList.Find(x => x.name == "L"), block.position, Quaternion.Euler(block.rotation), BlockParent.transform);

                BlockL blockL = gameObject.GetComponent<BlockL>();
                blockL.AddVisualColor(block.color);
                if (block.rotation != new Vector3(0, 0, 0) && block.rotation != new Vector3(0, 0, 180) && block.rotation != new Vector3(0, 0, -180) && block.rotation != new Vector3(0, 0, 360))
                {
                    blockL.BlockDirection = Direction.HORIZONTAL;
                }
                else
                {
                    blockL.BlockDirection = Direction.VERTICAL;
                }
                blockL.AddVisualWater(block.color);
                blockL.AddMoveDirection(block.moveDir);
            }
            else if (block.name.Contains("ThreeSquare"))
            {
                gameObject = Instantiate(blockList.Find(x => x.name == "ThreeSquare"), block.position, Quaternion.Euler(block.rotation), BlockParent.transform);

                BlockThreeSquare blockThreeSquare = gameObject.GetComponent<BlockThreeSquare>();
                blockThreeSquare.AddVisualColor(block.color);
                if (block.rotation != new Vector3(0, 0, 0) || block.rotation != new Vector3(0, 0, 360))
                {
                    blockThreeSquare.BlockDirection = Direction.HORIZONTAL;
                }
                else
                {
                    blockThreeSquare.BlockDirection = Direction.VERTICAL;
                }
                blockThreeSquare.AddVisualWater(block.color);
                blockThreeSquare.AddMoveDirection(block.moveDir);
            }
            gameObject.transform.localScale = Vector3.zero;
            blockInstances.Add(gameObject.transform);
        }

        foreach (var gate in levelData.gates)
        {
            GameObject gameObject = null;
            if (gate.name.Contains("GridGate1"))
            {
                gameObject = Instantiate(gateList.Find(x => x.name == "GridGate1"), gate.position, Quaternion.Euler(gate.rotation), GateParent.transform);
                WaterPipe waterPipe = gameObject.GetComponent<WaterPipe>();
                if (gate.rotation == new Vector3(0, 0, 0))
                {
                    waterPipe.DirectionPipe = DirectionPipe.Up;
                    waterPipe.PipeLineHeadCtrl.SetDirectionWaterFall(DirectionWaterSplash.UP);
                }
                else if (gate.rotation == new Vector3(0, 0, 90))
                {
                    waterPipe.DirectionPipe = DirectionPipe.Right;
                    waterPipe.PipeLineHeadCtrl.SetDirectionWaterFall(DirectionWaterSplash.RIGHT);
                }
                else if (gate.rotation == new Vector3(0, 0, 180))
                {
                    waterPipe.DirectionPipe = DirectionPipe.Down;
                    waterPipe.PipeLineHeadCtrl.SetDirectionWaterFall(DirectionWaterSplash.DOWN);
                }
                else if (gate.rotation == new Vector3(0, 0, 270))
                {
                    waterPipe.DirectionPipe = DirectionPipe.Left;
                    waterPipe.PipeLineHeadCtrl.SetDirectionWaterFall(DirectionWaterSplash.LEFT);
                }
                //init color
                waterPipe.InitColorPipe(gate.colorOutputs);
                // udpate ice
                waterPipe.UpdateIce();
                // update splash
                waterPipe.UpdateColorSplash();
                // play particle bubble
                waterPipe.PlayParticleIdleBubble();
                waterPipe.PipeLineCtrl.HideWater();
            }
            gameObject.transform.localScale = Vector3.zero;
            gateInstances.Add(gameObject.transform);
        }
    }

    private IEnumerator ScaleObjects(List<Transform> gridSlotInstances, List<Transform> gateInstances, List<Transform> blockInstances)
    {
        Sequence sequence = DOTween.Sequence();
        foreach (var g in gridSlotInstances)
        {
            sequence.Join(g.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        }
        foreach (var g in gateInstances)
        {
            sequence.Join(g.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        }
        sequence.AppendInterval(0.25f);
        foreach (var b in blockInstances)
        {
            sequence.Join(b.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        }
        yield return sequence.WaitForCompletion();
        yield return new WaitForSeconds(0.1f);

        //show water
        AudioManager.Instance.PlayOneShot("intro", 1f);
        foreach (var g in gateInstances)
        {
            WaterPipe waterPipe = g.GetComponent<WaterPipe>();
            waterPipe.PipeLineCtrl.ShowWater();
        }
    }
}
