using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    MyObject myChar;

    public Animator _anim;

    [Header("Board Variables")]
    public float column;
    public float row;
    public float previousColumn;
    public float previousRow;
    public float targetX;
    public float targetY;
    
    public int blockType;
    public int blockDamage;

    public bool isMatched = false;
    public bool isMove = false;
    public bool startBlock;
    private FindMatches findMatches;
    private Board board;
    [SerializeField]
    private GameObject otherBlock;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    [SerializeField]
    private Vector2 tempPosition;
    

    public float swipeAngle = 0;
    private float swipeResist = 0.5f;

    [Header("Power Up")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public GameObject columnArrow;
    public GameObject rowArrow;


    // Start is called before the first frame update
    void Start()
    {
        myChar = MyObject.MyChar;

        findMatches = FindObjectOfType<FindMatches>();
        board = FindObjectOfType<Board>();
        targetX = transform.position.x;
        targetY = transform.position.y;
        
        column = targetX;
        row = targetY;

        previousColumn = column;
        previousRow = row;

        if (blockType == 10)
        {
           _anim = GetComponent<Animator>();
        }

        if (!startBlock)
        {
            StartCoroutine(NewCheckMove());
        }

        isColumnBomb = false;
        isRowBomb = false;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;

        //좌우 이동
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f && Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            tempPosition = new Vector2(targetX, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.05f);
            if (board.allBlocks[(int)column, (int)row] != gameObject)
            {
                board.allBlocks[(int)column, (int)row] = gameObject;
            }
            findMatches.FindallMatch();
        }
        else if(Mathf.Abs(targetY - transform.position.y) > 0.1f)       //상하 이동
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.05f);
            if (board.allBlocks[(int)column, (int)row] != gameObject)
            {
                board.allBlocks[(int)column, (int)row] = gameObject;
            }
            findMatches.FindallMatch();
        }
        else            //블록 최종 이동시켜주기위함
        {
            tempPosition = new Vector2(targetX, targetY);
            transform.position = tempPosition;            
        }

        if (blockType == 10)
        {
            if (blockDamage <= 1)
            {
                _anim.SetBool("Active", true);
            }
            if (blockDamage <= 0)
            {
                myChar.DestroyScore--;
                Destroy(board.allBlocks[(int)column, (int)row]);
                board.allBlocks[(int)column, (int)row] = null;

                StartCoroutine(board.DecreaseRow());
            }
        }
        
    }

    
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }        
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngel();
        }
    }
    //마우스 클릭 각도 구하는법 
    void CalculateAngel()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }
        else
        {
            board.currentState = GameState.move;
        }
        
    }

    void MovePieces()
    {
        isMove = true;
        if (swipeAngle > 0 && swipeAngle <= 60 && column < board.width -1)   //우측 상단
        {
            if (column % 2 == 0)
            {
                otherBlock = board.allBlocks[(int)column + 1, (int)row];
            }
            else
            {
                otherBlock = board.allBlocks[(int)column + 1, (int)row + 1];
            }
            otherBlock.GetComponent<Block>().column -= 1;
            otherBlock.GetComponent<Block>().row -= 0.5f;

            column += 1;
            row += 0.5f;

           
        }
        else if (swipeAngle > -60 && swipeAngle <= 0 && column < board.width)   //우측 하단
        {
            if (column % 2 == 0)
            {
                otherBlock = board.allBlocks[(int)column + 1, (int)row - 1];
            }
            else
            {
                otherBlock = board.allBlocks[(int)column + 1, (int)row];
            }
            otherBlock.GetComponent<Block>().column -= 1;
            otherBlock.GetComponent<Block>().row += 0.5f;

            column += 1;
            row -= 0.5f;
            //if (blockType < 10)
            //{
            //    FindMatches();
            //}
        }
        if (swipeAngle > 120 && swipeAngle <= 180 && column < board.width)   //좌측 상단
        {
            if (column % 2 == 0)
            {
                otherBlock = board.allBlocks[(int)column - 1, (int)row];
            }
            else
            {
                otherBlock = board.allBlocks[(int)column - 1, (int)row + 1];
            }
            otherBlock.GetComponent<Block>().column += 1;
            otherBlock.GetComponent<Block>().row -= 0.5f;

            column -= 1;
            row += 0.5f;
            //if (blockType < 10)
            //{
            //    FindMatches();
            //}
        }
        else if (swipeAngle > -180 && swipeAngle <= -120 && column < board.width)   //좌측 하단
        {
            if (column % 2 == 0)
            {
                otherBlock = board.allBlocks[(int)column - 1, (int)row - 1];
            }
            else
            {
                otherBlock = board.allBlocks[(int)column - 1, (int)row];
            }
            otherBlock.GetComponent<Block>().column += 1;
            otherBlock.GetComponent<Block>().row += 0.5f;

            column -= 1;
            row -= 0.5f;
        }
        else if (swipeAngle > 60 && swipeAngle <= 120 && row < board.height - 1)   //상단
        {
            if (board.allBlocks[(int)column, (int)row + 1] != null)
            {
                otherBlock = board.allBlocks[(int)column, (int)row + 1];

                //otherBlock.GetComponent<Block>().column += 1;
                otherBlock.GetComponent<Block>().row -= 1f;

                //column -= 1;
                row += 1f;
            }
            
        }
        else if (swipeAngle > -120 && swipeAngle <= -60 && row < board.height)   //하단
        {
            if (board.allBlocks[(int)column, (int)row - 1] != null)
            {
                otherBlock = board.allBlocks[(int)column, (int)row - 1];

                //otherBlock.GetComponent<Block>().column += 1;
                otherBlock.GetComponent<Block>().row += 1f;

                //column -= 1;
                row -= 1f;
            }
            
        }
        StartCoroutine(CheckMove());
    }
    //void FindMatches()
    //{
    //    if (column > 0 && column < board.width - 1 && row > 0 && row < board.height - 1 && blockType < 10)
    //    {

    //        if (column % 2 == 0)
    //        {
    //            //좌상단 + 우하단
    //            GameObject leftUpBlock1 = board.allBlocks[(int)column - 1, (int)row];
    //            GameObject rightDownBlock1 = board.allBlocks[(int)column + 1, (int)row - 1];
    //            if (leftUpBlock1 != null && rightDownBlock1 != null)
    //            {
    //                if (gameObject.CompareTag(leftUpBlock1.tag) && gameObject.CompareTag(rightDownBlock1.tag))
    //                {
    //                    //Debug.Log(gameObject.name + "(" + gameObject.tag + ")" + " // left : " + leftUpBlock1.name + "(" + leftUpBlock1.tag + ") , right :" + rightDownBlock1.name + "(" + rightDownBlock1.tag + ")");
    //                    leftUpBlock1.GetComponent<Block>().isMatched = true;
    //                    rightDownBlock1.GetComponent<Block>().isMatched = true;
    //                    isMatched = true;
    //                }
    //            }
    //            //좌하단 + 우상단
    //            GameObject leftDownBlock1 = board.allBlocks[(int)column - 1, (int)row - 1];
    //            GameObject rightUpBlock1 = board.allBlocks[(int)column + 1, (int)row];
    //            if (leftDownBlock1 != null && rightUpBlock1 != null)
    //            {
    //                if (gameObject.CompareTag(leftDownBlock1.tag) && gameObject.CompareTag(rightUpBlock1.tag))
    //                {
    //                    //Debug.Log(gameObject.name + "(" + gameObject.tag + ")" + " // left : " + leftDownBlock1.name + "(" + leftDownBlock1.tag + ") , right :" + rightUpBlock1.name + "(" + rightUpBlock1.tag + ")");
    //                    leftDownBlock1.GetComponent<Block>().isMatched = true;
    //                    rightUpBlock1.GetComponent<Block>().isMatched = true;
    //                    isMatched = true;
    //                }
    //            }
    //            //우상상
    //            if (column - 1 < board.width && column - 2 >= 0 && row + 1 < board.height - 1)
    //            {
    //                GameObject rightUpBlock1_1 = board.allBlocks[(int)column + 1, (int)row];
    //                GameObject rightUpBlock1_2 = board.allBlocks[(int)column + 2, (int)row + 1];
    //                if (rightUpBlock1_1 != null && rightUpBlock1_1 != null)
    //                {
    //                    if (gameObject.CompareTag(rightUpBlock1_1.tag) && gameObject.CompareTag(rightUpBlock1_2.tag))
    //                    {
    //                        rightUpBlock1_1.GetComponent<Block>().isMatched = true;
    //                        rightUpBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //            //우하하
    //            if (column - 1 >= 0 && column - 2 >= 0 && row + 1 < board.height - 1)
    //            {
    //                GameObject rightDownBlock1_1 = board.allBlocks[(int)column + 1, (int)row - 1];
    //                GameObject rightDownBlock1_2 = board.allBlocks[(int)column + 2, (int)row - 1];

    //                if (rightDownBlock1_1 != null && rightDownBlock1_2 != null)
    //                {
    //                    if (gameObject.CompareTag(rightDownBlock1_1.tag) && gameObject.CompareTag(rightDownBlock1_2.tag))
    //                    {
    //                        rightDownBlock1_1.GetComponent<Block>().isMatched = true;
    //                        rightDownBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //            //좌상상
    //            if (column - 1 >= 0 && column - 2 >= 0 && row + 1 < board.height - 1)
    //            {
    //                GameObject leftUpBlock1_1 = board.allBlocks[(int)column - 1, (int)row];
    //                GameObject leftUpBlock1_2 = board.allBlocks[(int)column - 2, (int)row + 1];
    //                if (leftUpBlock1_1 != null && leftUpBlock1_2 != null)
    //                {
    //                    if (gameObject.CompareTag(leftUpBlock1_1.tag) && gameObject.CompareTag(leftUpBlock1_2.tag))
    //                    {
    //                        leftUpBlock1_1.GetComponent<Block>().isMatched = true;
    //                        leftUpBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //            //좌하하
    //            if (column - 1 >= 0 && column - 2 >= 0 && row - 1 >= 0)
    //            {
    //                GameObject leftDownBlock1_1 = board.allBlocks[(int)column - 1, (int)row - 1];
    //                GameObject leftDownBlock1_2 = board.allBlocks[(int)column - 2, (int)row - 1];
    //                if (leftDownBlock1_1 != null && leftDownBlock1_2 != null)
    //                {
    //                    if (gameObject.CompareTag(leftDownBlock1_1.tag) && gameObject.CompareTag(leftDownBlock1_2.tag))
    //                    {
    //                        leftDownBlock1_1.GetComponent<Block>().isMatched = true;
    //                        leftDownBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            //좌상단 + 우하단
    //            GameObject leftUpBlock1 = board.allBlocks[(int)column - 1, (int)row + 1];
    //            GameObject rightDownBlock1 = board.allBlocks[(int)column + 1, (int)row];
    //            if (leftUpBlock1 != null && rightDownBlock1 != null)
    //            {
    //                if (gameObject.CompareTag(leftUpBlock1.tag) && gameObject.CompareTag(rightDownBlock1.tag))
    //                {
    //                    //Debug.Log(gameObject.name + "(" + gameObject.tag + ")" + " // left : " + leftUpBlock1.name + "(" + leftUpBlock1.tag + ") , right :" + rightDownBlock1.name + "(" + rightDownBlock1.tag + ")");
    //                    //Debug.Log(gameObject.name + "(" + gameObject.tag + ")" + " // left : " + leftUpBlock1.tag + " , right :" + rightDownBlock1.tag);
    //                    leftUpBlock1.GetComponent<Block>().isMatched = true;
    //                    rightDownBlock1.GetComponent<Block>().isMatched = true;
    //                    isMatched = true;
    //                }
    //            }
    //            //좌하단 + 우상단
    //            GameObject leftDownBlock1 = board.allBlocks[(int)column - 1, (int)row];
    //            GameObject rightUpBlock1 = board.allBlocks[(int)column + 1, (int)row + 1];
    //            if (leftDownBlock1 != null && rightUpBlock1 != null)
    //            {
    //                if (gameObject.CompareTag(leftDownBlock1.tag) && gameObject.CompareTag(rightUpBlock1.tag))
    //                {
    //                    Debug.Log(gameObject.name + "(" + gameObject.tag + ")" + " // left : " + leftDownBlock1.name + "(" + leftDownBlock1.tag + ") , right :" + rightUpBlock1.name + "(" + rightUpBlock1.tag + ")");
    //                    leftDownBlock1.GetComponent<Block>().isMatched = true;
    //                    rightUpBlock1.GetComponent<Block>().isMatched = true;
    //                    isMatched = true;
    //                }
    //            }
    //            //우상상

    //            if (column + 1 <= board.width - 1 && column + 2 <= board.width - 1 && row + 1 <= board.height - 1)
    //            {
    //                GameObject rightUpBlock1_1 = board.allBlocks[(int)column + 1, (int)row + 1];
    //                GameObject rightUpBlock1_2 = board.allBlocks[(int)column + 2, (int)row + 1];
    //                if (rightUpBlock1_1 != null && rightUpBlock1_1 != null)
    //                {
    //                    if (gameObject.CompareTag(rightUpBlock1_1.tag) && gameObject.CompareTag(rightUpBlock1_2.tag))
    //                    {
    //                        rightUpBlock1_1.GetComponent<Block>().isMatched = true;
    //                        rightUpBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //            //우하하
    //            if (column + 1 <= board.width - 1 && column + 2 <= board.width - 1 && row - 1 >= 0)
    //            {
    //                GameObject rightDownBlock1_1 = board.allBlocks[(int)column + 1, (int)row];
    //                GameObject rightDownBlock1_2 = board.allBlocks[(int)column + 2, (int)row - 1];
    //                if (rightDownBlock1_1 != null && rightDownBlock1_2 != null)
    //                {
    //                    if (gameObject.CompareTag(rightDownBlock1_1.tag) && gameObject.CompareTag(rightDownBlock1_2.tag))
    //                    {
    //                        rightDownBlock1_1.GetComponent<Block>().isMatched = true;
    //                        rightDownBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //            //좌상상
    //            if (column - 1 >= 0 && column - 2 >= 0 && row + 1 <= board.height - 1)
    //            {
    //                GameObject leftUpBlock1_1 = board.allBlocks[(int)column - 1, (int)row + 1];
    //                GameObject leftUpBlock1_2 = board.allBlocks[(int)column - 2, (int)row + 1];

    //                if (leftUpBlock1_1 != null && leftUpBlock1_2 != null)
    //                {
    //                    if (gameObject.CompareTag(leftUpBlock1_1.tag) && gameObject.CompareTag(leftUpBlock1_2.tag))
    //                    {
    //                        leftUpBlock1_1.GetComponent<Block>().isMatched = true;
    //                        leftUpBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //            //좌하하
    //            if (column - 1 >= 0 && column - 2 >= 0 && row - 1 >= 0)
    //            {
    //                GameObject leftDownBlock1_1 = board.allBlocks[(int)column - 1, (int)row];
    //                GameObject leftDownBlock1_2 = board.allBlocks[(int)column - 2, (int)row - 1];
    //                if (leftDownBlock1_1 != null && leftDownBlock1_2 != null)
    //                {
    //                    if (gameObject.CompareTag(leftDownBlock1_1.tag) && gameObject.CompareTag(leftDownBlock1_2.tag))
    //                    {
    //                        leftDownBlock1_1.GetComponent<Block>().isMatched = true;
    //                        leftDownBlock1_2.GetComponent<Block>().isMatched = true;
    //                        isMatched = true;
    //                    }
    //                }
    //            }
    //        }
    //        //상상
    //        if (row + 1 <= board.height - 1 && row + 2 <= board.height - 1)
    //        {
    //            GameObject UpBlock1_1 = board.allBlocks[(int)column, (int)row + 1];
    //            GameObject UpBlock1_2 = board.allBlocks[(int)column, (int)row + 2];
    //            if (UpBlock1_1 != null && UpBlock1_2 != null)
    //            {
    //                if (gameObject.CompareTag(UpBlock1_1.tag) && gameObject.CompareTag(UpBlock1_2.tag))
    //                {
    //                    UpBlock1_1.GetComponent<Block>().isMatched = true;
    //                    UpBlock1_2.GetComponent<Block>().isMatched = true;
    //                    isMatched = true;
    //                }
    //            }
    //        }
    //        //상하
    //        GameObject UpBlock1 = board.allBlocks[(int)column, (int)row + 1];
    //        GameObject DownBlock1 = board.allBlocks[(int)column, (int)row - 1];
    //        if (UpBlock1 != null && DownBlock1 != null)
    //        {
    //            if (gameObject.CompareTag(UpBlock1.tag) && gameObject.CompareTag(DownBlock1.tag))
    //            {
    //                UpBlock1.GetComponent<Block>().isMatched = true;
    //                DownBlock1.GetComponent<Block>().isMatched = true;
    //                isMatched = true;
    //            }
    //        }
    //        //하하
    //        if (row - 1 >= 0 && row - 2 >= 0)
    //        {
    //            GameObject DownBlock1_1 = board.allBlocks[(int)column, (int)row - 1];
    //            GameObject DownBlock1_2 = board.allBlocks[(int)column, (int)row - 2];
    //            if (DownBlock1_1 != null && DownBlock1_2 != null)
    //            {
    //                if (gameObject.CompareTag(DownBlock1_1.tag) && gameObject.CompareTag(DownBlock1_2.tag))
    //                {
    //                    DownBlock1_1.GetComponent<Block>().isMatched = true;
    //                    DownBlock1_2.GetComponent<Block>().isMatched = true;
    //                    isMatched = true;
    //                }
    //            }
    //        }
    //    }
    //}
   
    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(0.2f);
        if (otherBlock != null)
        {
            //if (blockType < 10)
            //{
            //    FindMatches();
            //}
            if (!isMatched && !otherBlock.GetComponent<Block>().isMatched)
            {
                otherBlock.GetComponent<Block>().row = row;
                otherBlock.GetComponent<Block>().column = column;
                row = previousRow;
                column = previousColumn;
                //board.allBlocks[(int)column, (int)row] = gameObject;
                yield return new WaitForSeconds(0.2f);
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            otherBlock = null;
        }
    }
    //새로나온 블럭 빈칸 찾아가게하기
    private bool NewMatchesOnBoard()
    {
        if (row - 1 >= 0 && column - 1 >= 0 && column + 1 <= board.width - 1)
        {
            if (board.allBlocks[(int)column, (int)row - 1] == null)
            {
                board.allBlocks[(int)column, (int)row] = null;
                row -= 1;
                previousRow -= 1;
                return true;
            }
            else
            {
                if (column % 2 == 0)
                {
                    if (board.allBlocks[(int)column - 1, (int)row - 1] == null)     //왼쪽아래
                    {
                        board.allBlocks[(int)column, (int)row] = null;
                        column -= 1;
                        row -= 0.5f;
                        previousColumn -= 1f;
                        previousRow -= 0.5f;
                        return true;
                    }
                    else if(board.allBlocks[(int)column + 1, (int)row - 1] == null)     //오른쪽아래
                    {
                        board.allBlocks[(int)column, (int)row] = null;
                        column += 1;
                        row -= 0.5f;
                        previousColumn += 1f;
                        previousRow -= 0.5f;
                        return true;
                    }
                    else
                    {
                        startBlock = true;
                        board.allBlocks[(int)column, (int)row] = gameObject;
                        //if (blockType < 10)
                        //{
                        //    FindMatches();
                        //}
                        return false;
                    }

                }

                if(column % 2 != 0)
                {
                    if (board.allBlocks[(int)column - 1, (int)row] == null)     //왼쪽아래
                    {
                        board.allBlocks[(int)column, (int)row] = null;
                        column -= 1;
                        row -= 0.5f;
                        previousColumn -= 1f;
                        previousRow -= 0.5f;
                        return true;
                    }                    
                    else if (board.allBlocks[(int)column + 1, (int)row] == null)        //오른쪽아래
                    {
                        board.allBlocks[(int)column, (int)row] = null;
                        column += 1;
                        row -= 0.5f;
                        previousColumn += 1f;
                        previousRow -= 0.5f;
                        return true;
                    }
                    else
                    {
                        startBlock = true;
                        board.allBlocks[(int)column, (int)row] = gameObject;
                        //if (blockType < 10)
                        //{
                        //    FindMatches();
                        //}
                        return false;
                    }
                }
                return true;
            }
        }
        else
        {
            //if (blockType < 10)
            //{
            //    FindMatches();
            //}
            startBlock = true;
            return false;
        }

    }
    //private bool RandLCheck()
    //{
    //    if (column % 2 == 0)
    //    {
    //        if (board.allBlocks[(int)column + 1, (int)row - 1] == null)
    //        {
    //            column += 1;
    //            row -= 0.5f;
    //            previousColumn += 1f;
    //            previousRow -= 0.5f;
    //        }
    //        else if (board.allBlocks[(int)column - 1, (int)row - 1] == null)
    //        {
    //            column -= 1;
    //            row -= 0.5f;
    //            previousColumn -= 1f;
    //            previousRow -= 0.5f;
    //        }
    //        else
    //        {
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        if (board.allBlocks[(int)column + 1, (int)row] == null)
    //        {
    //            column += 1;
    //            row -= 0.5f;
    //            previousColumn += 1f;
    //            previousRow -= 0.5f;
    //        }
    //        else if (board.allBlocks[(int)column - 1, (int)row] == null)
    //        {
    //            column -= 1;
    //            row -= 0.5f;
    //            previousColumn -= 1f;
    //            previousRow -= 0.5f;
    //        }
    //        else
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public IEnumerator NewCheckMove()
    {
        while (NewMatchesOnBoard())
        {
            yield return new WaitForSeconds(0.1f);
        }
    }
}
