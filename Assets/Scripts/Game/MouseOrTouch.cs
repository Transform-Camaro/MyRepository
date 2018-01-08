using UnityEngine;

public class MouseOrTouch : MonoBehaviour
{
    private Camera mCamera;

    private MouseOrTouchInfo mInfo;

    private Touch mTouch;

    private Ray ray;
    private RaycastHit hit;

    public class MouseOrTouchInfo
    {
        public Vector2 beganPos = Vector2.zero;
        public Vector2 pos = Vector2.zero;
        public Vector2 lastPos = Vector2.zero;
        public Vector2 delta = Vector2.zero;
        public Vector2 totalDelta = Vector2.zero;

        public GameObject currentGo = null;
    }

    public delegate void VoidDelegate(MouseOrTouchInfo info);

    static public VoidDelegate OnTouchBegan;
    static public VoidDelegate OnTouchMove;
    static public VoidDelegate OnTouchEnd;


    void Awake()
    {
        mCamera = Camera.main;
    }
    void Update()
    {
#if UNITY_EDITOR
        OnMouse();
#elif UNITY_STANDALONE_WIN
        OnMouse();
#elif UNITY_IPHONE
         OnTouch();
#endif
    }

    private void OnMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mInfo = new MouseOrTouchInfo();
            mInfo.beganPos = mInfo.pos = Input.mousePosition;
            RayCast(mInfo.beganPos);
            if (OnTouchBegan != null)
            {
                OnTouchBegan(mInfo);
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition;
            mInfo.delta = pos - mInfo.pos;
            mInfo.pos = pos;
            mInfo.totalDelta += mInfo.delta;
            RayCast(mInfo.pos);
            if (OnTouchMove != null)
            {
                OnTouchMove(mInfo);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 pos = Input.mousePosition;
            mInfo.delta = pos - mInfo.pos;

            mInfo.totalDelta += mInfo.delta;

            mInfo.lastPos = mInfo.pos = pos;

            RayCast(mInfo.pos);
            if (OnTouchEnd != null)
            {
                OnTouchEnd(mInfo);
            }
            mInfo = null;
        }
    }

    private void OnTouch()
    {
        if (Input.touchCount > 0)
        {
            mTouch = Input.GetTouch(0);
            if (mTouch.phase == TouchPhase.Began)
            {
                mInfo = new MouseOrTouchInfo();
                mInfo.beganPos = mInfo.pos = mTouch.position;
                RayCast(mInfo.beganPos);
                if (OnTouchBegan != null)
                {
                    OnTouchBegan(mInfo);
                }
            }
            else if (mTouch.phase == TouchPhase.Moved)
            {
                mInfo.delta = mTouch.deltaPosition;
                mInfo.pos = mTouch.position;
                mInfo.totalDelta += mInfo.delta;
                RayCast(mInfo.pos);
                if (OnTouchMove != null)
                {
                    OnTouchMove(mInfo);
                }
            }
            else if (mTouch.phase == TouchPhase.Ended)
            {
                mInfo.delta = mTouch.deltaPosition;
                mInfo.totalDelta += mInfo.delta;
                mInfo.lastPos = mInfo.pos = mTouch.position;
                RayCast(mInfo.pos);
                if (OnTouchEnd != null)
                {
                    OnTouchEnd(mInfo);
                }
                mInfo = null;
            }

        }
    }

    private void RayCast(Vector3 inpos)
    {
        Vector3 pos = mCamera.ScreenToViewportPoint(inpos);
        if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f) return;

        ray = mCamera.ScreenPointToRay(inpos);
        if (Physics.Raycast(ray, out hit))
        {
            mInfo.currentGo = hit.collider.gameObject;
        }
        else
        {
            mInfo.currentGo = null;
        }

    }
}
