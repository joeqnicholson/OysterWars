using UnityEngine;

public class AngleTest : MonoBehaviour
{
    //Use these to get the GameObject's positions
    Vector2 m_MyFirstVector;
    Vector2 m_MySecondVector;

    float m_Angle;

    //You must assign to these two GameObjects in the Inspector
    public GameObject m_MyObject;
    public GameObject m_MyOtherObject;

    void Start()
    {
        //Initialise the Vector
        m_MyFirstVector = Vector2.zero;
        m_Angle = 0.0f;
    }

    void Update()
    {
        //Fetch the first GameObject's position
        m_MyFirstVector = m_MyObject.transform.position;
        //Fetch the second GameObject's position
        m_MySecondVector = m_MyOtherObject.transform.position;
        //Find the angle for the two Vectors
        m_Angle = Vector2.Angle(m_MyFirstVector, m_MySecondVector);
    }

     
}