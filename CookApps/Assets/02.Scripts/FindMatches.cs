using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FindallMatch()
    {
        StartCoroutine(FindAllMatches());
    }

    private IEnumerator FindAllMatches()
    {
        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                GameObject currentBlock = board.allBlocks[x, y];
                
                if (currentBlock != null && !currentBlock.CompareTag("CleanBlock") && currentBlock.GetComponent<Block>())
                {
                    if (x > 0 && x < board.width - 1 && y > 0 && y < board.height - 1 && currentBlock.GetComponent<Block>().blockType < 10)
                    {
                        if (x % 2 == 0)
                        {
                            //ÁÂ»ó ¿ìÇÏ
                            GameObject leftBlock1 = board.allBlocks[x - 1, y];
                            GameObject rightBlock1 = board.allBlocks[x + 1, y - 1];

                            //ÁÂÇÏ ¿ì»ó
                            GameObject leftBlock2 = board.allBlocks[x - 1, y - 1];
                            GameObject rightBlock2 = board.allBlocks[x + 1, y];
                            if (leftBlock1 != null && rightBlock1 != null && leftBlock2 != null && rightBlock2 != null)
                            {
                                if (currentBlock.CompareTag(leftBlock1.tag) && currentBlock.CompareTag(rightBlock1.tag))
                                {
                                    if (!currentMatches.Contains(leftBlock1))
                                    {
                                        currentMatches.Add(leftBlock1);
                                    }
                                    leftBlock1.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(rightBlock1))
                                    {
                                        currentMatches.Add(rightBlock1);
                                    }
                                    rightBlock1.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(currentBlock))
                                    {
                                        currentMatches.Add(currentBlock);
                                    }
                                    currentBlock.GetComponent<Block>().isMatched = true;
                                }
                                else if (currentBlock.CompareTag(leftBlock2.tag) && currentBlock.CompareTag(rightBlock2.tag))
                                {
                                    if (!currentMatches.Contains(leftBlock2))
                                    {
                                        currentMatches.Add(leftBlock2);
                                    }
                                    leftBlock2.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(rightBlock2))
                                    {
                                        currentMatches.Add(rightBlock2);
                                    }
                                    rightBlock2.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(currentBlock))
                                    {
                                        currentMatches.Add(currentBlock);
                                    }
                                    currentBlock.GetComponent<Block>().isMatched = true;
                                }
                            }
                        }
                        else
                        {
                            //ÁÂ»ó ¿ìÇÏ
                            GameObject leftBlock1 = board.allBlocks[x - 1, y + 1];
                            GameObject rightBlock1 = board.allBlocks[x + 1, y];

                            //ÁÂÇÏ ¿ì»ó
                            GameObject leftBlock2 = board.allBlocks[x - 1, y];  
                            GameObject rightBlock2 = board.allBlocks[x + 1, y + 1];
                            if (leftBlock1 != null && rightBlock1 != null && leftBlock2 != null && rightBlock2 != null)
                            {
                                if (currentBlock.CompareTag(leftBlock1.tag) && currentBlock.CompareTag(rightBlock1.tag))
                                {
                                    if (!currentMatches.Contains(leftBlock1))
                                    {
                                        currentMatches.Add(leftBlock1);
                                    }
                                    leftBlock1.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(rightBlock1))
                                    {
                                        currentMatches.Add(rightBlock1);
                                    }
                                    rightBlock1.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(currentBlock))
                                    {
                                        currentMatches.Add(currentBlock);
                                    }
                                    currentBlock.GetComponent<Block>().isMatched = true;
                                }
                                else if (currentBlock.CompareTag(leftBlock2.tag) && currentBlock.CompareTag(rightBlock2.tag))
                                {
                                    if (!currentMatches.Contains(leftBlock2))
                                    {
                                        currentMatches.Add(leftBlock2);
                                    }
                                    leftBlock2.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(rightBlock2))
                                    {
                                        currentMatches.Add(rightBlock2);
                                    }
                                    rightBlock2.GetComponent<Block>().isMatched = true;
                                    if (!currentMatches.Contains(currentBlock))
                                    {
                                        currentMatches.Add(currentBlock);
                                    }
                                    currentBlock.GetComponent<Block>().isMatched = true;
                                }
                            }
                        }
                    }
                    if (y > 0 && y < board.height - 1)
                    {
                        GameObject upBlock = board.allBlocks[x, y + 1];
                        GameObject downBlock = board.allBlocks[x, y - 1];
                        if (upBlock != null && downBlock != null)
                        {
                            if (currentBlock.CompareTag(upBlock.tag) && currentBlock.CompareTag(downBlock.tag))
                            {
                                if (!currentMatches.Contains(upBlock))
                                {
                                    currentMatches.Add(upBlock);
                                }
                                upBlock.GetComponent<Block>().isMatched = true;
                                if (!currentMatches.Contains(downBlock))
                                {
                                    currentMatches.Add(downBlock);
                                }
                                downBlock.GetComponent<Block>().isMatched = true;
                                if (!currentMatches.Contains(currentBlock))
                                {
                                    currentMatches.Add(currentBlock);
                                }
                                currentBlock.GetComponent<Block>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
