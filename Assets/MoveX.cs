using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveX : MonoBehaviour
{



    void Start()
    {
        StartCoroutine(Tester());
    }

    // Update is called once per frame
    void Update()
    {

    }


    public IEnumerator Tester()
    {

        print("dank sauce");

        yield return 1243;

        print("all done sauce");

    }

}
