using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float visAngle;
    public float visRadius;

    public LayerMask observablesMask;
    public LayerMask obstaclesMask;
    public LayerMask illusionMask;

    [HideInInspector]
    public bool detectedPlayer; 
    [HideInInspector]
    public bool detectedIllusion;

    [HideInInspector]
    public Transform playerDetected;
    public Transform illusionDetected;

    public float fovMeshRes;

    public MeshFilter viewMeshFilter;
    private MeshRenderer meshRenderer;
    Mesh viewMesh;


    public Material mat_patrol;
    public Material mat_detected;


    public void Start()
    {
        meshRenderer = viewMeshFilter.gameObject.GetComponent<MeshRenderer>();  
        viewMesh = new Mesh();
        viewMesh.name = "FOVMesh";
        viewMeshFilter.mesh = viewMesh;
        meshRenderer.material = mat_patrol;
        detectedPlayer = false;
        StartCoroutine(FOVCheck());
    }
    

    IEnumerator FOVCheck()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            DrawFOV();
            DetectPlayersinFOV();
            
        }
    }

    void DetectPlayersinFOV()
    {
        Collider[] playersInRange = Physics.OverlapSphere(transform.position, visRadius, observablesMask);
        Collider[] illusionInRange = Physics.OverlapSphere(transform.position, visRadius, illusionMask);

        if (illusionInRange.Length != 0)
        {
            illusionDetected = illusionInRange[0].transform;

            Vector3 dirToTarget = (illusionDetected.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < visAngle / 2)
            {
                float distToTarget = Vector3.Distance(this.transform.position, illusionDetected.transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstaclesMask))
                {
                    if (!illusionDetected.GetComponent<IllusionScript>().isStunned)
                    {
                        detectedIllusion = true;
                        meshRenderer.material = mat_detected;
                    }
                    else
                    {
                        detectedIllusion = false;
                        meshRenderer.material = mat_patrol;
                    }
                }
                else
                {
                    detectedIllusion = false;
                    meshRenderer.material = mat_patrol;
                }
            }
            else
            {
                detectedIllusion = false;
                meshRenderer.material = mat_patrol;
            }
        }
        else if (detectedIllusion)
        {
            detectedIllusion = false;
            meshRenderer.material = mat_patrol;
        }


        if (playersInRange.Length != 0 && !detectedIllusion)
         {
             playerDetected = playersInRange[0].transform;

             Vector3 dirToTarget = (playersInRange[0].transform.position - transform.position).normalized;

             if (Vector3.Angle(transform.forward, dirToTarget) < visAngle / 2)
             {
                 float distToTarget = Vector3.Distance(this.transform.position, playerDetected.transform.position);

                 if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstaclesMask))
                 {

                     if (!playerDetected.parent.GetComponent<PlayerScript>().isStunned)
                     {
                         detectedPlayer = true;
                         meshRenderer.material = mat_detected;
                     }
                     else
                     {
                         detectedPlayer = false;
                         meshRenderer.material = mat_patrol;
                     }
                 }
                 else
                 {
                     detectedPlayer = false;
                     meshRenderer.material = mat_patrol;
                 }
             }
             else
             {
                 detectedPlayer = false; 
                 meshRenderer.material = mat_patrol;
             }
         }
         else if (detectedPlayer)
         {
             detectedPlayer = false; 
             meshRenderer.material = mat_patrol;
         }
    }

    void DrawFOV()
    {
        int rayCount = Mathf.RoundToInt(visAngle * fovMeshRes);
        float stepAngleSize = visAngle / rayCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 1; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.y - visAngle / 2 + stepAngleSize * i;

            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero; // zero because mesh will be made in local pos
        for(int i=0; i<vertexCount-1;i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = AngleDir(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, visRadius, obstaclesMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * visRadius, visRadius, globalAngle);
        }
    }

    public Vector3 AngleDir(float angleInDeg, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angleInDeg += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDeg * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle; 

        }
    }
}
