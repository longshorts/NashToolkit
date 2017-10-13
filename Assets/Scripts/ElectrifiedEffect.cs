using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This effect currently assumes that we are on a plane
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class ElectrifiedEffect : MonoBehaviour {

    /// <summary>
    /// Defines the tracked points in space that ELinks are drawn between
    /// </summary>
    private struct EPoint
    {
        public GameObject pointObject;
        public Vector2 velocity;
    }

    /// <summary>
    /// Define properties nessesary to draw the line renderer between the given points
    /// </summary>
    private struct ELink
    {
        public GameObject linkObject;
        public LineRenderer lr;
        public Transform pointA;
        public Transform pointB;
    }

    public int numOfPoints = 5;
    public float maxSpeed = 0.5f;
    public float pointAltitude = 0.5f;

    [SerializeField]
    private GameObject pointPrefab;

    private MeshFilter m_mf;
    private EPoint[] m_points;
    private List<ELink> m_links;
    private Vector3 m_min;
    private Vector3 m_max;
    private float m_scale;

	void Start () {
        m_mf = GetComponent<MeshFilter>();
        m_min = m_mf.mesh.bounds.min;
        m_max = m_mf.mesh.bounds.max;
        m_scale = (transform.localScale.x + transform.localScale.z) / 2;

        //Create the effect's EPoints and give them a random velocity
        m_points = new EPoint[numOfPoints];
        for(int i = 0; i < numOfPoints; i++)
        {
            m_points[i].pointObject = GameObject.Instantiate(pointPrefab, transform);
            m_points[i].pointObject.transform.localPosition = new Vector3(Random.Range(m_min.x, m_max.x), pointAltitude, Random.Range(m_min.z, m_max.z));
            m_points[i].velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * maxSpeed;
        }

        //Create a link between each couple of points
        m_links = new List<ELink>();
        for(int i = 0; i < numOfPoints; i++){
            for(int j = i+1; j < numOfPoints; j++){
                ELink link = new ELink();
                link.pointA = m_points[i].pointObject.transform;
                link.pointB = m_points[j].pointObject.transform;

                link.linkObject = new GameObject();
                link.lr = link.linkObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
                link.lr.material = new Material(Shader.Find("Particles/Additive"));
                link.lr.widthMultiplier = 0.02f*m_scale;
                link.lr.positionCount = 2;
                link.lr.startColor = Color.blue;
                link.lr.endColor = Color.white;

                link.lr.SetPosition(0, link.pointA.position);
                link.lr.SetPosition(1, link.pointB.position);
                m_links.Add(link);
            }
        }
	}
	
	void Update () {

        #region EPoint movement
        for(int i = 0; i < m_points.Length; i++)
        {
            Transform pt = m_points[i].pointObject.transform;
            pt.localPosition += new Vector3(m_points[i].velocity.x,0,m_points[i].velocity.y) * Time.deltaTime;
            //Reflect velocity if out of bounds
            if(pt.localPosition.x < m_min.x || pt.localPosition.x > m_max.x)
            {
                m_points[i].velocity = new Vector2(m_points[i].velocity.x * -1, m_points[i].velocity.y);
                pt.localPosition += new Vector3(m_points[i].velocity.x,0,m_points[i].velocity.y) * Time.deltaTime;
            }
            if (pt.localPosition.z < m_min.z || pt.localPosition.z > m_max.z)
            {
                m_points[i].velocity = new Vector2(m_points[i].velocity.x, m_points[i].velocity.y * -1);
                pt.localPosition += new Vector3(m_points[i].velocity.x,0,m_points[i].velocity.y) * Time.deltaTime;
            }
        }
        #endregion

        #region ELink rendering
        for (int i = 0; i < m_links.Count; i++){
            m_links[i].lr.SetPosition(0, m_links[i].pointA.position);
            m_links[i].lr.SetPosition(1, m_links[i].pointB.position);
            float distance = Vector3.Distance(m_links[i].pointA.position, m_links[i].pointB.position);
            Color c1 = m_links[i].lr.startColor;
            Color c2 = m_links[i].lr.endColor;
            //Alpha is calculated according to the distance between each point, relative to the plane's scale.
            c1.a = Mathf.Max(2.0f - distance/m_scale, 0.0f);
            c2.a = c1.a;
            m_links[i].lr.startColor = c1;
            m_links[i].lr.endColor = c2;
        }
        #endregion
    }
}
