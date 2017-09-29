using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This effect currently assumes that we are on a plane
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class ElectrifiedEffect : MonoBehaviour {

    private struct EPoint
    {
        public GameObject pointObject;
        public Vector2 velocity;
    }

    public int numOfPoints = 5;
    public float maxSpeed = 0.5f;
    public float pointAltitude = 0.5f;

    [SerializeField]
    private GameObject pointPrefab;

    private MeshFilter m_mf;
    private EPoint[] m_points;
    private Vector3 m_min;
    private Vector3 m_max;

	// Use this for initialization
	void Start () {
        m_mf = GetComponent<MeshFilter>();
        //Min and max possible locations need to be scaled
        m_min = Vector3.Scale(m_mf.mesh.bounds.min, transform.localScale);
        m_max = Vector3.Scale(m_mf.mesh.bounds.max, transform.localScale);

        m_points = new EPoint[numOfPoints];
        for(int i = 0; i < numOfPoints; i++)
        {
            m_points[i].pointObject = GameObject.Instantiate(pointPrefab, transform);
            m_points[i].pointObject.transform.localPosition = new Vector3(Random.Range(m_min.x, m_max.x), pointAltitude, Random.Range(m_min.z, m_max.z));
            m_points[i].velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * maxSpeed;
        }
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < m_points.Length; i++)
        {
            Transform pt = m_points[i].pointObject.transform;
            pt.localPosition += new Vector3(m_points[i].velocity.x,0,m_points[i].velocity.y);
            //Reflect velocity if out of bounds
            if(pt.localPosition.x < m_min.x || pt.localPosition.x > m_max.x)
            {
                m_points[i].velocity = new Vector2(m_points[i].velocity.x * -1, m_points[i].velocity.y);
            }
            if (pt.localPosition.z < m_min.z || pt.localPosition.z > m_max.z)
            {
                m_points[i].velocity = new Vector2(m_points[i].velocity.x, m_points[i].velocity.y * -1);
            }
        }
	}
}
