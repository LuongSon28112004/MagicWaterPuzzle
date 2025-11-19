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
    // Lists to keep track of instantiated objects
    private List<Transform> gridSlotInstances = new List<Transform>();
    private List<Transform> gateInstances = new List<Transform>();
    private List<Transform> blockInstances = new List<Transform>();


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
            if(tile.name.Contains("GridSlot"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "GridSlot"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            else if(tile.name.Contains("InnerCornerGridBorder"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "InnerCornerGridBorder"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            else if(tile.name.Contains("OuterCornerGridBorder"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "OuterCornerGridBorder"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            else if(tile.name.Contains("StraightGridBorder"))
            {
                gameObject = Instantiate(gridSlotList.Find(x => x.name == "StraightGridBorder"), tile.position, Quaternion.Euler(tile.rotation), GridSlotParent.transform);
            }
            gameObject.transform.localScale = Vector3.zero;
            gridSlotInstances.Add(gameObject.transform);
        }

        foreach (var block in levelData.blocks)
        {
            GameObject gameObject = null;
            if(block.name.Contains("Two"))
            {
                gameObject = Instantiate(blockList.Find(x => x.name == "Two"), block.position, Quaternion.Euler(block.rotation), BlockParent.transform);
                // Block blockComponent = gameObject.GetComponent<Block>();
                // blockComponent.SetColor(block.color);
            }
            // else if(block.name.Contains("Three"))
            // {
            //     GameObject gameObject = Instantiate(blockList.Find(x => x.name == "ThreeBlock"), block.position, Quaternion.Euler(block.rotation));
            //     Block blockComponent = gameObject.GetComponent<Block>();
            //     blockComponent.SetColor(block.color);
            // }
            gameObject.transform.localScale = Vector3.zero;
            blockInstances.Add(gameObject.transform);
        }

        foreach (var gate in levelData.gates)
        {
            GameObject gameObject = null;
            if(gate.name.Contains("GridGate1"))
            {
                gameObject = Instantiate(gateList.Find(x => x.name == "GridGate1"), gate.position, Quaternion.Euler(gate.rotation), GateParent.transform);
                // Gate gateComponent = gameObject.GetComponent<Gate>();
                // gateComponent.SetGateColors(gate.gateColorInfos);
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
    }
}
