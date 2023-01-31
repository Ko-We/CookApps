using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    MyObject myChar;

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int BlockType;       //블록 종류는 10개가 넘지 않을걸 생가하여 10이넘으면 특수 블럭으로 처리하기위함
    public int offSet;          //위에서 블럭 떯어지게하는 높이 추가용으로 사용할려고함

    private float xOffset = 1;
    private float yOffset = 1;

    public Transform BlockParent;
    public Text SpecialCnt;

    public bool MapType;        //맵의 시작 블록이 정해져있어야하면 MapType를 false로 받아서 필요위치에 블록을넣어주고 true라면 랜덤으로 블록이 들어가게 제작
    private bool specialCheck;

    public GameObject[] Blocks;
    public GameObject[] SpecialBlocks;
    public GameObject titlePrefab;
    public GameObject cleantitlePrefab;

    public GameObject ClearPopUp;

    [SerializeField]
    public GameObject[,] allBlocks;
    private FindMatches findMatches;
    [SerializeField]
    private List<GameObject> SpecialBlocksCheck = new List<GameObject>();

    public int[,] Stage;        //맵을 그리기위해 2차원 배열로 보기 편하게 제작하기위함    

    public bool StageCheck = false;
    // Start is called before the first frame update
    private void Awake()
    {
        myChar = MyObject.MyChar;
        findMatches = FindObjectOfType<FindMatches>();
        ClearPopUp.SetActive(false);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!StageCheck)
        {
            allBlocks = new GameObject[width, height];
            SetUp();
            StageCheck = true;
        }

        SpecialCnt.text = myChar.DestroyScore.ToString();
        if (myChar.DestroyScore <= 0)
        {
            ClearPopUp.SetActive(true);
        }

    }

    //맵 만들어주는 함수
    public void SetUp()
    {
        int[,] Stage = new int[7, 6] { { 0,0,1,2,6,0},
        {0,10,4,4,10,0 },
        {0,10,6,5,2,10 },
        {10,4,10,5,2,1 },
        {0,10,5,2,1,10 },
        {0,10,4,4,10,0 },
        {0,0,1,2,6,0} };

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float yPos = y * yOffset;
                if (x % 2 == 1)
                {
                    yPos += yOffset / 2;
                }
                if (Stage[x, y] != 0)
                {
                    GameObject Board = Instantiate(titlePrefab, new Vector2(x * xOffset, yPos), Quaternion.identity);
                    Board.transform.parent = transform;
                    Board.name = "(" + x + ", " + y + ")";
                    //Board.GetComponent<BackgroundTile>().BlockType = Stage[x, y];
                }
                else
                {
                    GameObject Board = Instantiate(cleantitlePrefab, new Vector2(x * xOffset, yPos), Quaternion.identity);
                    Board.transform.parent = transform;
                    Board.name = "(" + x + ", " + y + ")";
                }
                if (Stage[x, y] != 0)
                {
                    if (MapType)
                    {
                        switch (Stage[x, y])
                        {
                            case 1:
                                int BlockToUse = Random.Range(1, Blocks.Length);
                                GameObject Block = Instantiate(Blocks[BlockToUse], new Vector2(x * xOffset, yPos), Quaternion.identity);
                                Block.transform.parent = BlockParent;
                                Block.name = "(" + x + ", " + y + ")";
                                Block.GetComponent<Block>().startBlock = true;
                                allBlocks[x, y] = Block;
                                break;
                            case 2:
                                GameObject SpecialBlock = Instantiate(SpecialBlocks[0], new Vector2(x * xOffset, yPos), Quaternion.identity);
                                SpecialBlock.transform.parent = BlockParent;
                                SpecialBlock.name = "(" + x + ", " + y + ")";
                                SpecialBlock.GetComponent<Block>().startBlock = true;
                                allBlocks[x, y] = SpecialBlock;
                                break;
                        }
                    }
                    else
                    {
                        GameObject Block;
                        if (Stage[x, y] < 10)
                        {
                            Block = Instantiate(Blocks[Stage[x, y]], new Vector2(x * xOffset, yPos), Quaternion.identity);
                            Block.transform.parent = BlockParent;
                            Block.name = "(" + x + ", " + y + ")";
                            Block.GetComponent<Block>().blockType = 0;
                            Block.GetComponent<Block>().startBlock = true;
                            allBlocks[x, y] = Block;
                        }
                        else
                        {
                            Block = Instantiate(SpecialBlocks[Stage[x, y] - 10], new Vector2(x * xOffset, yPos), Quaternion.identity);
                            Block.transform.parent = BlockParent;
                            Block.name = "(" + x + ", " + y + ")";
                            Block.GetComponent<Block>().blockType = 10;
                            Block.GetComponent<Block>().startBlock = true;
                            Block.GetComponent<Block>().blockDamage = 2;
                            allBlocks[x, y] = Block;
                            myChar.DestroyScore++;
                        }
                    }
                }
                else
                {
                    GameObject Block;
                    if (Stage[x, y] < 10)
                    {
                        Block = Instantiate(Blocks[Stage[x, y]], new Vector2(x * xOffset, yPos), Quaternion.identity);
                        Block.transform.parent = BlockParent;
                        Block.name = "(" + x + ", " + y + ")";
                        //Block.GetComponent<Block>().blockType = 0;
                        allBlocks[x, y] = Block;
                    }
                }
            }
        }
    }


    
    private void Deduplication(GameObject SpecialBlock)
    {

        for (int i = 0; i < SpecialBlocksCheck.Count; i++)
        {
            if (SpecialBlocksCheck[i].name == SpecialBlock.name)
            {
                return;
            }
        }
        SpecialBlocksCheck.Add(SpecialBlock);
    }
    private void DestroyMatchesAt(int column, int row)
    {       
        if (allBlocks[column, row].GetComponent<Block>() != null && allBlocks[column, row].GetComponent<Block>().isMatched) //일반블럭 파괴
        {
            StartCoroutine(SpecialBlockCheck(column, row));
            findMatches.currentMatches.Remove(allBlocks[column, row]);
            Destroy(allBlocks[column, row]);
            allBlocks[column, row] = null;
        }
        //else if(allBlocks[column, row].GetComponent<Block>() != null && allBlocks[column, row].GetComponent<Block>())   //특수블럭 파괴
        //{
        //    Debug.Log(111);
        //    if (allBlocks[column, row].GetComponent<Block>().blockType == 10 && allBlocks[column, row].GetComponent<Block>().blockDamage <= 0)
        //    {
        //        myChar.DestroyScore--;
        //        Destroy(allBlocks[column, row]);
        //        allBlocks[column, row] = null;
        //    }
        //}
    }
    public void DestroyMatches()
    {        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBlocks[x,y] != null)
                {
                    DestroyMatchesAt(x, y);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    public IEnumerator SpecialBlockCheck(int column, int row)
    {
        if (column > 0 && column < width - 1 && yOffset > 0 && yOffset < height - 1)
        {
            if (column % 2 == 0)
            {

                GameObject leftBlock1 = allBlocks[column - 1, row];
                GameObject rightBlock1 = allBlocks[column + 1, row - 1];

                GameObject leftBlock2 = allBlocks[column - 1, row - 1];
                GameObject rightBlock2 = allBlocks[column + 1, row];
                if (leftBlock1 != null && leftBlock1.CompareTag("SpecialBlock"))
                {
                    Deduplication(leftBlock1);
                    //SpecialBlocksCheck.Add(leftBlock1);
                }
                else if (rightBlock1 != null && rightBlock1.CompareTag("SpecialBlock"))
                {
                    Deduplication(rightBlock1);
                    //SpecialBlocksCheck.Add(rightBlock1);
                }
                else if (leftBlock2 != null && leftBlock2.CompareTag("SpecialBlock"))
                {
                    Deduplication(leftBlock2);
                    //SpecialBlocksCheck.Add(leftBlock2);
                }
                else if (rightBlock2 != null && rightBlock2.CompareTag("SpecialBlock"))
                {
                    Deduplication(rightBlock2);
                    //SpecialBlocksCheck.Add(rightBlock2);
                }

            }
            else
            {
                GameObject leftBlock1 = allBlocks[column - 1, row + 1];
                GameObject rightBlock1 = allBlocks[column + 1, row];

                GameObject leftBlock2 = allBlocks[column - 1, row];
                GameObject rightBlock2 = allBlocks[column + 1, row + 1];

                if (leftBlock1 != null && leftBlock1.CompareTag("SpecialBlock"))
                {
                    Deduplication(leftBlock1);
                    //SpecialBlocksCheck.Add(leftBlock1);
                }
                else if (rightBlock1 != null && rightBlock1.CompareTag("SpecialBlock"))
                {
                    Deduplication(rightBlock1);
                    //SpecialBlocksCheck.Add(rightBlock1);
                }
                else if (leftBlock2 != null && leftBlock2.CompareTag("SpecialBlock"))
                {
                    Deduplication(leftBlock2);
                    //SpecialBlocksCheck.Add(leftBlock2);
                }
                else if (rightBlock2 != null && rightBlock2.CompareTag("SpecialBlock"))
                {
                    Deduplication(rightBlock2);
                    //SpecialBlocksCheck.Add(rightBlock2);
                }
            }
        }
        if (row > 0 && row < height - 1)
        {
            GameObject upBlock = allBlocks[column, row + 1];
            GameObject downBlock = allBlocks[column, row - 1];
            if (upBlock != null && upBlock.CompareTag("SpecialBlock"))
            {
                Deduplication(upBlock);
                //SpecialBlocksCheck.Add(upBlock);
            }
            else if (downBlock != null && downBlock.CompareTag("SpecialBlock"))
            {
                Deduplication(downBlock);
                //SpecialBlocksCheck.Add(downBlock);
            }
        }

        yield return new WaitForSeconds(0.1f);
        if (!specialCheck)
        {
            specialCheck = true;
            for (int i = 0; i < SpecialBlocksCheck.Count; i++)
            {
                SpecialBlocksCheck[i].GetComponent<Block>().blockDamage--;
            }
            SpecialBlocksCheck.Clear();
        }
        yield return new WaitForSeconds(0.1f);
        specialCheck = false;
    }

    public void restart()
    {
        for (int i = 0; i < BlockParent.childCount; i++)
        {
            Destroy(BlockParent.GetChild(i).gameObject);
        }
        myChar.DestroyScore = 0;
        StageCheck = false;

        if (ClearPopUp.activeSelf)
        {
            ClearPopUp.SetActive(false);
        }
    }
    private IEnumerator RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBlocks[x, y] == null)
                {
                    yield return new WaitForSeconds(0.2f);
                    Vector2 temPosition = new Vector2(width / 2, height - 0.5f);
                    int BlockToUse = Random.Range(1, Blocks.Length);
                    GameObject Block = Instantiate(Blocks[BlockToUse], temPosition, Quaternion.identity);
                    Block.name = Random.Range(0, 100).ToString();
                    Block.transform.parent = BlockParent;
                    //allBlocks[x, y] = Block;
                }
            }
        }
        //if (allBlocks[(int)(width / 2), (int)(height - 0.5f)] == null)
        //{
        //    Vector2 temPosition = new Vector2(width / 2, height - 0.5f);
        //    int BlockToUse = Random.Range(1, Blocks.Length);
        //    GameObject Block = Instantiate(Blocks[BlockToUse], temPosition, Quaternion.identity);
        //    Block.name = Random.Range(0, 100).ToString();
        //    Block.transform.parent = BlockParent;
        //}
    }
    private bool MatchesOnBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBlocks[x,y] != null)
                {
                    if (allBlocks[x, y].GetComponent<Block>() != null && allBlocks[x, y].GetComponent<Block>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        StartCoroutine( RefillBoard());
        yield return new WaitForSeconds(0.2f);
        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
    }
    //블록 삭제하고 떨어지게하기
    public IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allBlocks[x,y] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    if (allBlocks[x, y].GetComponent<Block>() != null)
                    {
                        allBlocks[x, y].GetComponent<Block>().row -= nullCount;
                        allBlocks[x, y].GetComponent<Block>().previousRow -= nullCount;
                        allBlocks[x, y] = null;
                        allBlocks[x, y - nullCount] = gameObject;
                    }
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(FillBoard());
    }
    //private bool MatchesAt(int column, int row, GameObject piece)
    //{
    //    if (column > 1 && row > 1)
    //    {
    //        if (allBlocks[column - 1, row].tag == piece.tag && allBlocks[column - 2, row])
    //        {
    //            return true;
    //        }
    //        if (allBlocks[column, row-1].tag == piece.tag && allBlocks[column, row-2])
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}
}
