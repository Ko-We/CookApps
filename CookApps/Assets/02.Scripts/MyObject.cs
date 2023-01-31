using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyObject : MonoBehaviour
{
    /********************************** ½Ì ±Û Åæ *******************************************/
    private static MyObject s_MyObject = null;
    public static MyObject MyChar
    {
        get
        {
            if (s_MyObject == null)
            {
                s_MyObject = FindObjectOfType(typeof(MyObject)) as MyObject;
                if (s_MyObject == null)
                {
                    GameObject obj = new GameObject("MyChar");
                    s_MyObject = obj.AddComponent(typeof(MyObject)) as MyObject;
                }
            }
            return s_MyObject;
        }
    }
    /*************************************************************************************/

    public int DestroyScore;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
