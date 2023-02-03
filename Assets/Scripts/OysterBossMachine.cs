using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OysterBossMachine : MonoBehaviour
{
    public List<OysterPieceMachine> oysters = new List<OysterPieceMachine>(4);
    public GameObject singleOyster;
    public GameObject oysterExplosion;
    public List<Transform> positions = new List<Transform>();
    public int[] oysterIndices = new int[] {0,1,2,3};
    private int numberOfOysters = 0;
    private bool active;



    public IEnumerator SpawnOysters()
    {
        print("we're in BABAY");
        float time = 0;

        while(time < .5f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if(numberOfOysters < 4)
        {
            SpawnOyster();
        }
        else
        {
            StartCoroutine(Execute());
        }

    }

    public void SpawnOyster()
    {
        Instantiate(oysterExplosion,positions[numberOfOysters].position + Vector3.up * 15,Quaternion.identity);
        OysterPieceMachine oysterPiece = Instantiate(singleOyster, positions[numberOfOysters].position, Quaternion.identity).GetComponent<OysterPieceMachine>();
        oysterPiece.Index = numberOfOysters;
        oysters[numberOfOysters] = oysterPiece;
        numberOfOysters += 1;
        StartCoroutine(SpawnOysters());
    }

    public IEnumerator Execute()
    {
        float time = 0;

        while(time<2)
        {
            time+=Time.deltaTime;
            yield return null;
        }

        ShootOysters();

        time = 0;

        while(time<5)
        {
            time+=Time.deltaTime;
            yield return null;
        }

        MoveOysters();

        StartCoroutine(Execute());
    }


    void MoveOysters()
    {
        int index = 0;
        foreach(OysterPieceMachine oyster in oysters)
        {
            int positionIndex = oysterIndices[index];
            Vector3 positionToMove = positions[positionIndex%4].position;
            StartCoroutine(oyster.Move(positionToMove, positionIndex%4));
            oysterIndices[index] += 1;
            index += 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponentInParent<WadeMachine>() && !active)
        {
            StartCoroutine(SpawnOysters());
            active = true;
        }


    }


    void ShootOysters()
    {
        float delay = 0;
        foreach(OysterPieceMachine oyster in oysters)
        {
            StartCoroutine(oyster.SpitRoutine(delay));
            delay += 0.75f;
        }
    }
}
