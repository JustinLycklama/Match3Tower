using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Match3View : MonoBehaviour
{

    private struct GridSizeInfo
    {
        //public readonly float gridWidth;
        //public readonly float gridHeight;

        public readonly Vector3 startPosOffset;

        //public readonly Vector2 cellSize;
        public readonly Vector3 widthPositionVector;
        public readonly Vector3 heightPositionVector;


        public GridSizeInfo(int numRows, int numCols, float cellWidth, float cellHeight)
        {
            float gridWidth = numCols * cellWidth;
            float gridHeight = numRows * cellHeight;

            startPosOffset = new Vector3(gridWidth / 2, gridHeight / 2, 0);

            //cellSize = new Vector2(cellWidth, cellHeight);
            widthPositionVector = new Vector3(cellWidth, 0, 0);
            heightPositionVector = new Vector3(0, cellHeight, 0);
        }

        public Vector3 getLocalPosition(int x, int y)
        {
            return -startPosOffset + (widthPositionVector * x) + (heightPositionVector * y);
        }
    }

    public float cellWidth;
    public float cellHeight;

    //public RectTransform rectTransform;

    public SpriteRenderer template;
    //private ItemRow[] rowViewList;

    public Match3Model model;

    private void Awake()
    {
        template.transform.SetParent(null);
        template.gameObject.SetActive(false);
    }

    void Start()
    {
        //StartCoroutine(WaitForChange());

        SetGridSize(model.numberOfRows, model.numberOfColumns);

        model.subscribe((dataMap) =>
        {
            print("Display New Datamap");

            int numAnimations = dataMap.Keys.Count;
            print($"SET numAnimations {numAnimations}");

            foreach (Item key in dataMap.Keys)
            {
                Coordinate coordinate = dataMap[key];
                setPosition(key, coordinate.x, coordinate.y, () =>
                {
                    print($"numAnimations {numAnimations}");
                    numAnimations -= 1;
                    if (numAnimations <= 0) {
                        model.dataUpdated = true;
                    }
                });
            }

            List<Item> itemsInGrid = gameObjectMap.Keys.ToList();
            var itemsLeftInGrid = itemsInGrid.Where(item => !dataMap.Keys.Contains(item));

            foreach (var item in itemsLeftInGrid)
            {                
                delete(item, () =>
                {
                    //numAnimations -= 1;
                });
            }

        });
    }

    //public int numAnimations = 100;
    //IEnumerator checkAnimationsCompleted()
    //{
    //    yield return new WaitUntil(() => { return numAnimations == 0; });
    //    //numAnimations = 50;
    //    print($"IENUM numAnimations {numAnimations}");


    //    //model.dataUpdated = true;
    //}

    // Update is called once per frame
    void Update()
    {

    }

    //public int numberOfRows;
    //public int numberOfColumns;

    private RowAnimationState[] rowAnimationStates;

    //private HashSet<int> activeItemsIds = new HashSet<int>();

    private bool dataUpdated = false;
    //private List<MovieItem> newDataCollection = new List<MovieItem>();
    //private List<MovieItem> currentAnimationCycleCollection = new List<MovieItem>();

    GridSizeInfo gridSizeInfo;

    Dictionary<Item, GameObject> gameObjectMap = new Dictionary<Item, GameObject>();

    //HashSet<ItemView> itemSet = new HashSet<ItemView>();



    //struct ItemView: IEquatable<Item>//, IComparable<Item>
    //{
    //    public Item item;

    //    public GameObject gameObject;

    //    public ItemView(Item item, GameObject gameObject)
    //    {
    //        this.item = item;
    //        this.gameObject = gameObject;
    //    }

    //    public bool Equals(Item other)
    //    {
    //        return item.id == other.id;
    //    }

    //    //public int CompareTo(Item other)
    //    //{
    //    //    return id.CompareTo(other.id);
    //    //}
    //}


    public void SetGridSize(int numberOfRows, int numberOfColumns)
    {

        //for (int i = 0; i < numberOfRows; i++)
        //{
        //    if (rowViewList != null && rowViewList[i] != null)
        //    {
        //        rowViewList[i].transform.SetParent(null);
        //        Destroy(rowViewList[i].gameObject);
        //    }
        //}

        gridSizeInfo = new GridSizeInfo(numberOfRows, numberOfColumns, cellWidth, cellHeight);

        //float gridWidth = numberOfColumns * cellWidth;
        //float gridHeight = numberOfRows * cellHeight;

        //Vector3 startPos = transform.position - new Vector3(gridWidth / 2, gridHeight / 2, 0);

        //Vector2 cellSize = new Vector2(cellWidth, cellHeight);
        //Vector3 widthPositionVector = new Vector3(cellWidth, 0, 0);
        //Vector3 heightPositionVector = new Vector3(0, cellHeight, 0);


    }

    private void setPosition(Item item, int row, int col, Action onComplete)
    {

        if (!gameObjectMap.ContainsKey(item))
        {
            SpriteRenderer newObject = Instantiate(template);
            newObject.transform.SetParent(transform);
            newObject.gameObject.SetActive(true);

            gameObjectMap[item] = newObject.gameObject;
        }

        GameObject obj = gameObjectMap[item];
        //obj.transform.position = transform.position + gridSizeInfo.getLocalPosition(row, col);

        Vector3 destination = transform.position + gridSizeInfo.getLocalPosition(row, col);


        var distance = Vector3.Distance(obj.transform.position, destination); // or whatever distance you calculate.
        var desiredSpeed = 5; // that's in pixels per second, more or less.
        var time = distance / desiredSpeed; // this number is the number of seconds.


        Tween t = obj.transform.DOMove(destination, time);
        //t.onComplete(() => onComplete.Invoke());


        //// Don't bother saving the new state if it is transitioning off screen
        //if (toIndex >= 0 && toIndex < columns)
        //{
        //    nextTransitionCells[toIndex] = cellAtPos;
        //}

        t.OnComplete(() =>
        {
            onComplete();
        });

    }

    private void delete(Item item, Action onComplete)
    {
        if (!gameObjectMap.ContainsKey(item))
        {
            return;
        }

        GameObject obj = gameObjectMap[item];


        Tween t = obj.transform.DOScale(Vector3.zero, 0.5f);
        //t.onComplete(() => onComplete.Invoke());


        //// Don't bother saving the new state if it is transitioning off screen
        //if (toIndex >= 0 && toIndex < columns)
        //{
        //    nextTransitionCells[toIndex] = cellAtPos;
        //}

        t.OnComplete(() =>
        {
            gameObjectMap.Remove(item);
            Destroy(obj);
            onComplete();
        });

    }

    private void UpdateRowStateGivenCollection(List<MovieItem> collection)
    {
        //for (int row = 0; row < numberOfRows; row++)
        //{
        //    List<MovieItem> rowItems = collection.Skip(row * numberOfColumns).Take(numberOfColumns).ToList();

        //    rowAnimationStates[row].SetRowItems(rowItems, activeItemsIds, filteredItemsIds);
        //}
    }

    private void PrintStateChange()
    {
        print("State Change");
        print("");

        int row = 0;
        foreach (RowAnimationState state in rowAnimationStates)
        {
            print("Row " + row + ":");
            state.Print();
            row++;
        }

        print("");
    }

    /*
     * Movie Update Notifiable Interface
     * */

    public void MovieCollectionUpdated(List<MovieItem> collection)
    {
        //newDataCollection = collection;
        //dataUpdated = true;
    }

    /*
     * Animation States
     * */

    //IEnumerator WaitForChange()
    //{

    //    yield return new WaitUntil(() => { return dataUpdated; });
    //    dataUpdated = false;

    //    currentAnimationCycleCollection = newDataCollection.Where(item => !filteredItemsIds.Contains(item.id)).ToList();

    //    // Update State
    //    UpdateRowStateGivenCollection(currentAnimationCycleCollection);
    //    //PrintStateChange();

    //    // Perform Animations
    //    StartCoroutine(PerformAnimateUpdate(new List<CellPhase> { CellPhase.Delete, CellPhase.Transpose, CellPhase.Create }));
    //}

    //IEnumerator PerformAnimateUpdate(List<CellPhase> phases)
    //{

    //    if (phases.Count == 0)
    //    {
    //        CompleteAnimateUpdate();
    //        yield break;
    //    }

    //    CellPhase phase = phases[0];
    //    phases.RemoveAt(0);

    //    int animations = 0;
    //    for (int row = 0; row < numberOfRows; row++)
    //    {

    //        RowAnimationState state = rowAnimationStates[row];
    //        ItemRow rowView = rowViewList[row];

    //        animations += state.EnactModificationsOnObject(rowView, phase, () => { animations--; });
    //    }

    //    yield return new WaitUntil(() => animations == 0);

    //    StartCoroutine(PerformAnimateUpdate(phases));
    //}

    //private void CompleteAnimateUpdate()
    //{

    //    // Cleanup and reset
    //    activeItemsIds = new HashSet<int>(currentAnimationCycleCollection.Select(x => x.id));

    //    for (int row = 0; row < numberOfRows; row++)
    //    {
    //        rowViewList[row].Consolidate();
    //    }

    //    StartCoroutine(WaitForChange());
    //}

    void OnDrawGizmos()
    {
        //RectTransform rt = (RectTransform)transform;

        //Vector3 width = new Vector3(rectTransform.size.width, 0, 0);
        //Vector3 height = new Vector3(0, rectTransform.rect.height, 0);

        //Vector3 startX = transform.position - (width / 2);
        //Vector3 startY = transform.position - (height / 2);

        //// Draws a blue line from this transform to the target
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(startX, startX + width);

        //Gizmos.DrawCube(transform.position, spriteRenderer.bounds.size);
        //Gizmos.DrawSphere(transform.position, 5f);
    }
}
