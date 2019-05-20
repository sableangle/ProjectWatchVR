// ResizeScript
using System;
using UnityEngine;

public class ResizeScript : MonoBehaviour
{
    private const int TOTAL_RAY_CAST_COUNT = 108;

    private const float ray_cast_offset = 0.01f;

    private const float ray_cast_min_diff_ratio = 0.1f;

    private const int MAX_VERTEX_COUNT = 30;

    private float grab_distance = 0.5f;

    public bool is_grabbing;

    private GameObject grabbed_object;

    private Vector3 grabbed_local_vector;

    public bool can_player_grab;

    public bool target_object_blocked;

    private static float mass_constant = 1f;

    private Vector3 largest_vec = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);


    private float prevYRot;

    public Material DrawOutlineMat;

    private Color canSpinColor = new Color(0.3f, 0.3f, 1f, 1f);

    private Color canFollowColor = new Color(1f, 0.3f, 0.3f, 1f);

    private Color cannotDoAnythingColor = new Color(0f, 0f, 0f, 1f);

    // private MouseLook mouseLook1;

    // private MouseLook mouseLook2;

    private Vector3 grabbed_object_localPos;

    private Vector3 grabbed_local_forward_vec;

    private bool spunLastFrame;

    private float spinSpeed = 7f;

    private Vector3 prevGrabbedPosition;

    private float followVelocityRatio = 9f;

    private void Start()
    {

        // Screen.showCursor = false;
        // Screen.showCursor = false;
        // Screen.lockCursor = true;
        // DrawOutlineMat = GetComponents<PostProcessCamera>()[1].mat;
        // MouseLook[] componentsInChildren = base.transform.root.GetComponentsInChildren<MouseLook>();
        // mouseLook1 = componentsInChildren[0];
        // mouseLook2 = componentsInChildren[1];
    }

    public void trigger_drop_object()
    {
        drop_object();
    }

    private void Update()
    {
        can_player_grab = false;
        target_object_blocked = false;
        // mouseLook1.skipUpdate = false;
        // mouseLook2.skipUpdate = false;
        if (!is_grabbing)
        {
            grabbed_object = null;
            GameObject x = check_for_grabbable_object();
            if (x != null)
            {
                grabbed_object = x;
                can_player_grab = true;
            }
        }
        if (Input.GetMouseButtonDown(0) && !is_grabbing)
        {
            spunLastFrame = false;
            if (can_player_grab)
            {
                grabbed_object.layer = LayerMask.NameToLayer("Grabbed");
                grabbed_object.GetComponent<DropTriggerScript>().setChildrenColliderLayers(LayerMask.NameToLayer("Grabbed"));
                Vector3 vector = grabbed_object.transform.position - transform.position;
                float num = grab_distance / vector.magnitude;
                grabbed_object.transform.localScale *= num;
                grabbed_object.transform.position = transform.position + grab_distance * vector.normalized;
                is_grabbing = true;
                grabbed_local_vector = Quaternion.Inverse(transform.rotation) * (grab_distance * vector.normalized);
                var grabbed_object_rigidbody = grabbed_object.GetComponent<Rigidbody>();
                grabbed_object_rigidbody.isKinematic = false;
                grabbed_object_rigidbody.velocity = Vector3.zero;
                grabbed_object_rigidbody.angularVelocity = Vector3.zero;
                grabbed_object.GetComponent<DropTriggerScript>().isGrabbed = true;
                grabbed_object_rigidbody.useGravity = false;
                grabbed_object_rigidbody.mass = newMass(grabbed_object_rigidbody.mass, num);
                //sound_effect_script.play_pop_sound_grab();
                prevGrabbedPosition = grabbed_object.transform.position;
                if (grabbed_object.GetComponent<DropTriggerScript>().canSpin)
                {
                    //DrawOutlineMat.color = canSpinColor;
                }
                else if (grabbed_object.GetComponent<DropTriggerScript>().canFollow)
                {
                    //DrawOutlineMat.color = canFollowColor;
                }
                else
                {
                    //DrawOutlineMat.color = cannotDoAnythingColor;
                }
                return;
            }
            //sound_effect_script.play_denied_sound();
        }
        else if (Input.GetMouseButtonDown(0) && is_grabbing)
        {
            //Debug.Log("Release");
            //normal_recenter_object();
            float num2 = get_resize_ratio(grabbed_object);
            if (num2 == float.MaxValue)
            {
                //sound_effect_script.play_denied_sound();
            }
            else
            {
                Debug.Log("Release");

                Vector3 position = grabbed_object.transform.position;
                if (grabbed_object.transform.root != grabbed_object.transform)
                {
                    grabbed_object.transform.parent = null;
                }
                num2 *= 0.99f;
                transform_object_by_ratio(grabbed_object, num2);
                var grabbed_object_rigidbody = grabbed_object.GetComponent<Rigidbody>();

                grabbed_object_rigidbody.mass = newMass(grabbed_object_rigidbody.mass, num2);
                drop_object();
                setFollowVelocity(position, num2);
                //sound_effect_script.play_pop_sound_put();
            }
            spunLastFrame = false;
        }
        // else if (Input.GetMouseButtonDown(1) && is_grabbing && grabbed_object.GetComponent<DropTriggerScript>().canSpin)
        // {
        //     // mouseLook1.skipUpdate = true;
        //     // mouseLook2.skipUpdate = true;
        //     grabbed_object.transform.position = base.transform.position + base.transform.rotation * grabbed_local_vector;
        //     var grabbed_object_rigidbody = grabbed_object.GetComponent<Rigidbody>();
        //     grabbed_object_rigidbody.position = grabbed_object.transform.position;
        //     grabbed_object.transform.RotateAroundLocal(transform.right, Input.GetAxis("Mouse Y") * Time.deltaTime * spinSpeed);
        //     grabbed_object.transform.RotateAroundLocal(transform.up, (0f - Input.GetAxis("Mouse X")) * Time.deltaTime * spinSpeed);
        //     Debug.Log("ASD");
        // }
        else if (is_grabbing)
        {
            normal_recenter_object();
			//UpdatePosAndScale();
        }
        Vector3 eulerAngles = base.transform.rotation.eulerAngles;
        prevYRot = eulerAngles.y;
    }
   
    private void normal_recenter_object()
    {
        prevGrabbedPosition = grabbed_object.transform.position;
        Debug.Log(grabbed_object.transform.position);

        grabbed_object.transform.position = transform.position + transform.rotation * grabbed_local_vector;
        var grabbed_object_rigidbody = grabbed_object.GetComponent<Rigidbody>();
        Debug.Log(grabbed_object.transform.position);
        grabbed_object_rigidbody.position = grabbed_object.transform.position;
        Vector3 eulerAngles = base.transform.rotation.eulerAngles;
        float y = eulerAngles.y;
        grabbed_object.transform.RotateAround(Vector3.up, (float)Math.PI / 180f * (y - prevYRot));
    }

    private void drop_object()
    {
        grabbed_object.layer = LayerMask.NameToLayer("CanGrab");
        grabbed_object.GetComponent<DropTriggerScript>().returnChildrenColliderLayers();
        is_grabbing = false;
        var grabbed_object_rigidbody = grabbed_object.GetComponent<Rigidbody>();

        grabbed_object_rigidbody.useGravity = true;
        grabbed_object_rigidbody.isKinematic = false;
        grabbed_object_rigidbody.velocity = Vector3.zero;
        grabbed_object_rigidbody.angularVelocity = Vector3.zero;
        if (grabbed_object.GetComponent<DropTriggerScript>().is_kinematic)
        {
            grabbed_object_rigidbody.isKinematic = true;
        }
        grabbed_object_rigidbody.WakeUp();
        grabbed_object.GetComponent<DropTriggerScript>().isGrabbed = false;
    }
    public LayerMask layerMask;
    private GameObject check_for_grabbable_object()
    {
        RaycastHit hitInfo = default(RaycastHit);
        if (Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, float.PositiveInfinity, layerMask))
        {
            GameObject gameObject = hitInfo.collider.gameObject;
            if (!get_is_object_blocked(gameObject))
            {
                return gameObject;
            }
            target_object_blocked = true;
            return null;
        }
        return null;
    }

    private bool get_is_object_blocked(GameObject obj)
    {
        Mesh sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        int layer = obj.layer;
        obj.layer = LayerMask.NameToLayer("tempTarget");
        RaycastHit hitInfo = default(RaycastHit);
        int num = 108 / (sharedMesh.triangles.Length / 3 + sharedMesh.vertexCount / 3 - 2);
        Debug.Log("triangleCount:" + sharedMesh.triangles.GetLength(0) + "Verts:" + sharedMesh.vertexCount + "  Rays: " + num);
        int layerMask = -1029;
        int layerMask2 = 1 << obj.layer;
        obj.GetComponent<DropTriggerScript>().setChildrenColliderLayers(LayerMask.NameToLayer("Ignore Raycast"));
        if (num == 0)
        {
            Vector3[] vertices = sharedMesh.vertices;
            foreach (Vector3 position in vertices)
            {
                Vector3 dest = obj.transform.TransformPoint(position);
                dest = get_ray_cast_offset(dest, obj.transform.position);
                Vector3 direction = dest - base.transform.position;
                if (Physics.Raycast(base.transform.position, direction, out hitInfo, float.PositiveInfinity, layerMask) && hitInfo.collider.gameObject.GetInstanceID() != obj.GetInstanceID() && Physics.Raycast(base.transform.position, direction, out hitInfo, float.PositiveInfinity, layerMask2))
                {
                    obj.layer = layer;
                    obj.GetComponent<DropTriggerScript>().returnChildrenColliderLayers();
                    return true;
                }
            }
            obj.layer = layer;
            obj.GetComponent<DropTriggerScript>().returnChildrenColliderLayers();
            return false;
        }
        for (int j = 0; j < sharedMesh.triangles.Length; j += 3)
        {
            for (int k = 0; k < 3; k++)
            {
                Vector3 dest2 = obj.transform.TransformPoint(sharedMesh.vertices[sharedMesh.triangles[k + j]]);
                Vector3 dest3 = obj.transform.TransformPoint(sharedMesh.vertices[sharedMesh.triangles[(k + 1) % 3 + j]]);
                dest2 = get_ray_cast_offset(dest2, obj.transform.position);
                dest3 = get_ray_cast_offset(dest3, obj.transform.position);
                Vector3 a = (dest3 - dest2) / (num + 1);
                for (int l = 0; l <= num + 1; l++)
                {
                    Vector3 a2 = dest2 + l * a;
                    Vector3 vector = a2 - base.transform.position;
                    Debug.DrawRay(base.transform.position, vector);
                    if (Physics.Raycast(base.transform.position, vector, out hitInfo, float.PositiveInfinity, layerMask) && hitInfo.collider.gameObject.GetInstanceID() != obj.GetInstanceID() && Physics.Raycast(base.transform.position, vector, float.PositiveInfinity, layerMask2))
                    {
                        Debug.DrawRay(base.transform.position, vector * 3f, Color.red);
                        Debug.Log("Target ID: " + obj.ToString() + "  Blocked by: " + hitInfo.collider.gameObject.ToString());
                        Debug.Log("Target ID: " + hitInfo.collider.gameObject.GetInstanceID() + "  Blocked by: " + obj.GetInstanceID());
                        obj.layer = layer;
                        obj.GetComponent<DropTriggerScript>().returnChildrenColliderLayers();
                        return true;
                    }
                }
            }
        }
        obj.layer = layer;
        obj.GetComponent<DropTriggerScript>().returnChildrenColliderLayers();
        return false;
    }

    private bool does_ray_gets_blocked(GameObject obj, Vector3 dir, int layerMask_without_player, int layerMask_target_only)
    {
        RaycastHit hitInfo = default(RaycastHit);
        if (Physics.Raycast(base.transform.position, dir, out hitInfo, float.PositiveInfinity, layerMask_without_player) && hitInfo.collider.gameObject.GetInstanceID() != obj.GetInstanceID() && Physics.Raycast(base.transform.position, dir, float.PositiveInfinity, layerMask_target_only))
        {
            return true;
        }
        return false;
    }

    private float get_resize_ratio(GameObject obj)
    {
        Mesh sharedMesh = obj.GetComponent<MeshFilter>().sharedMesh;
        int layerMask = ~((1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Grabbed")));
        RaycastHit hitInfo = default(RaycastHit);
        float num = float.MaxValue;
        int num2 = 108 / (sharedMesh.triangles.Length / 3 + sharedMesh.vertexCount / 3 - 2);
        if (num2 == 0)
        {
            Vector3[] vertices = sharedMesh.vertices;
            foreach (Vector3 position in vertices)
            {
                Vector3 dest = obj.transform.TransformPoint(position);
                dest = get_ray_cast_offset(dest, obj.transform.position);
                Vector3 direction = dest - transform.position;
                if (Physics.Raycast(transform.position, direction, out hitInfo, float.PositiveInfinity, layerMask))
                {
                    float distance = hitInfo.distance;
                    Vector3 point = hitInfo.point;
                    Ray ray = new Ray(point, transform.position - point);
                    var obj_collider = obj.GetComponent<Collider>();
                    obj_collider.Raycast(ray, out hitInfo, float.MaxValue);
                    float distance2 = hitInfo.distance;
                    float num3 = distance / (distance - distance2);
                    if (num3 < num)
                    {
                        num = num3;
                    }
                }
            }
            return num;
        }
        for (int j = 0; j < sharedMesh.triangles.Length; j += 3)
        {
            for (int k = 0; k < 3; k++)
            {
                Vector3 dest2 = obj.transform.TransformPoint(sharedMesh.vertices[sharedMesh.triangles[k + j]]);
                Vector3 dest3 = obj.transform.TransformPoint(sharedMesh.vertices[sharedMesh.triangles[(k + 1) % 3 + j]]);
                dest2 = get_ray_cast_offset(dest2, obj.transform.position);
                dest3 = get_ray_cast_offset(dest3, obj.transform.position);
                Vector3 a = (dest3 - dest2) / (num2 + 1);
                for (int l = 1; l <= num2 + 1; l++)
                {
                    Vector3 a2 = dest2 + l * a;
                    Vector3 direction2 = a2 - base.transform.position;
                    if (Physics.Raycast(base.transform.position, direction2, out hitInfo, float.PositiveInfinity, layerMask))
                    {
                        float distance3 = hitInfo.distance;
                        Vector3 point2 = hitInfo.point;
                        Ray ray2 = new Ray(point2, base.transform.position - point2);
                        var obj_collider = obj.GetComponent<Collider>();
                        obj_collider.Raycast(ray2, out hitInfo, float.MaxValue);
                        float distance4 = hitInfo.distance;
                        float num4 = distance3 / (distance3 - distance4);
                        if (num4 < num)
                        {
                            num = num4;
                        }
                    }
                }
            }
        }
        return num;
    }

    private void setFollowVelocity(Vector3 grabbed_Pos, float resize_ratio)
    {
        if (grabbed_object.GetComponent<DropTriggerScript>().canFollow)
        {
            Vector3 a = grabbed_Pos - prevGrabbedPosition;
            var grabbed_object_rigidbody = grabbed_object.GetComponent<Rigidbody>();

            grabbed_object_rigidbody.velocity = a * resize_ratio * followVelocityRatio;
        }
    }

    private void transform_object_by_ratio(GameObject obj, float ratio)
    {
        Vector3 a = obj.transform.position - base.transform.position;
        float magnitude = a.magnitude;
        Vector3 localScale = obj.transform.localScale;
        obj.transform.position = base.transform.position + ratio * a;
        obj.transform.localScale = ratio * localScale;
    }

    private Vector3 get_ray_cast_offset(Vector3 dest, Vector3 center)
    {
        Vector3 direction = center - dest;
        Vector3 vector = transform.InverseTransformDirection(direction);
        Vector3 direction2 = new Vector3(vector.x, vector.y, 0f);
        return dest + 0.01f * transform.TransformDirection(direction2).normalized;
    }

    private float newMass(float oldMass, float ratio)
    {
        return Mathf.Pow(ratio, 3f) * oldMass * mass_constant;
    }

    public float getGrabDistance()
    {
        return grab_distance;
    }

    public void setGrabDistance(float f)
    {
        grab_distance = f;
    }
}
