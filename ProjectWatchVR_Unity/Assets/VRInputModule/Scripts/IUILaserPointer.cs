using UnityEngine;


namespace Wacki
{
    abstract public class IUILaserPointer : MonoBehaviour
    {

        public float laserThickness = 0.002f;
        public float laserHitScale = 0.02f;
        public bool laserAlwaysOn = false;
        public bool isHit = false;
        public Color color;

        public Transform pointer;
        //public Transform pointer;

        private float _distanceLimit;

        // Use this for initialization
        void Start()
        {
            // todo:    let the user choose a mesh for laser pointer ray and hit point
            //          or maybe abstract the whole menu control some more and make the 
            //          laser pointer a module.
            // pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // pointer.transform.SetParent(transform, false);
            // pointer.transform.localScale = new Vector3(laserThickness, laserThickness, 100.0f);
            // pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);

            pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
            pointer.SetParent(transform, false);
            pointer.localScale = new Vector3(laserHitScale, laserHitScale, laserHitScale);
            pointer.localPosition = new Vector3(0.0f, 0.0f, 100.0f);

            pointer.gameObject.SetActive(false);

            // remove the colliders on our primitives
            Object.DestroyImmediate(pointer.GetComponent<SphereCollider>());
            // Object.DestroyImmediate(pointer.GetComponent<BoxCollider>());

            Material newMaterial = new Material(Shader.Find("Wacki/LaserPointer"));

            newMaterial.SetColor("_Color", color);
            // pointer.GetComponent<MeshRenderer>().material = newMaterial;
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
            // initialize concrete class
            Initialize();

            // register with the LaserPointerInputModule
            if (LaserPointerInputModule.instance == null)
            {
                new GameObject().AddComponent<LaserPointerInputModule>();
            }


            LaserPointerInputModule.instance.AddController(this);
        }

        void OnDestroy()
        {
            if (LaserPointerInputModule.instance != null)
                LaserPointerInputModule.instance.RemoveController(this);
        }

        protected virtual void Initialize() { }
        public virtual void OnEnterControl(GameObject control) { }
        public virtual void OnExitControl(GameObject control) { }
        abstract public bool ButtonDown();
        abstract public bool ButtonUp();

        protected virtual void Update()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
            bool isHit = Physics.Raycast(ray, out hitInfo);

            float distance = 100.0f;

            if (isHit)
            {
                distance = hitInfo.distance;
            }

            // ugly, but has to do for now
            if (_distanceLimit > 0.0f)
            {
                distance = Mathf.Min(distance, _distanceLimit);
                isHit = true;
            }
            distance = distance / 1.8f;
            // pointer.transform.localScale = new Vector3(laserThickness, laserThickness, distance);
            // pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

            if (isHit)
            {
                pointer.gameObject.SetActive(true);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
            }
            else
            {
                pointer.gameObject.SetActive(false);
            }

            // reset the previous distance limit
            _distanceLimit = -1.0f;
        }

        // limits the laser distance for the current frame
        public virtual void LimitLaserDistance(float distance)
        {
            if (distance < 0.0f)
                return;

            if (_distanceLimit < 0.0f)
                _distanceLimit = distance;
            else
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
        }
    }

}