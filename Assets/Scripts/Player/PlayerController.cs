using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController
{
    private enum PlayerDirection
    {
        self = 0,
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4
    }

    private Vector2[] Direction;

    private Player mPlayer;

    private Vector2 touchOnBegan = new Vector2();
    private Vector2 touchOnEnded = new Vector2();
    private Vector2 touchOnMove = new Vector2();

    private bool isValidTouch = false;

    public PlayerController(Player player)
    {
        mPlayer = player;
        touchOnBegan = touchOnEnded = Vector2.zero;
        isValidTouch = false;
        Direction = GameController.instance.gameConfig.mDirectionExample;
        MouseOrTouch.OnTouchBegan += OnTouchDown;
        MouseOrTouch.OnTouchEnd += OnTouchUp;
        MouseOrTouch.OnTouchMove += OnTouch;
    }
    public void OnManagerDestory()
    {
        isValidTouch = false;
        mPlayer = null;
        touchOnBegan = touchOnEnded = Vector2.zero;
        MouseOrTouch.OnTouchBegan -= OnTouchDown;
        MouseOrTouch.OnTouchEnd -= OnTouchUp;
        MouseOrTouch.OnTouchMove -= OnTouch;
    }
    private void MoveDirection(PlayerDirection direction)
    {

        if (GameManager.instance.IsGameing)
            mPlayer.OnMoveBegin(Direction[(int)direction]);

        //正确的手势，播放效果

    }

    private void WrongDirection()//错误的手势,播放效果
    {
    }

    private void OnTouchDown(MouseOrTouch.MouseOrTouchInfo info)
    {
        isValidTouch = true;
        touchOnBegan = touchOnEnded = Vector2.zero;
    }

    private void OnTouch(MouseOrTouch.MouseOrTouchInfo info)
    {
        if (EventSystem.current.IsPointerOverGameObject())//True是点到了UI，False是3D
        {
            isValidTouch = false;
            WrongDirection();
            return;
        }

        if (!isValidTouch)
            return;

        touchOnMove = info.delta;

        if (Mathf.Abs(touchOnMove.x) > Mathf.Abs(touchOnMove.y))//失败的手势
        {
            isValidTouch = false;
            WrongDirection();
        }

    }
    private void OnTouchUp(MouseOrTouch.MouseOrTouchInfo info)
    {
        if (EventSystem.current.IsPointerOverGameObject())//True是点到了UI，False是3D
        {
            isValidTouch = false;
            WrongDirection();
            return;
        }
        if (!isValidTouch)
            return;

        if (Mathf.Abs(info.totalDelta.x) > Mathf.Abs(info.totalDelta.y))//失败的手势
        {
            isValidTouch = false;
            WrongDirection();
            return;
        }

        isValidTouch = false;

        touchOnBegan = info.beganPos;
        touchOnEnded = info.lastPos;

        if (Vector2.Distance(touchOnBegan, touchOnEnded) < 0.1f)
        {
            if (touchOnEnded.x < Screen.width / 2)//左边屏幕
            {
                MoveDirection(PlayerDirection.Left);
            }
            else
            {
                MoveDirection(PlayerDirection.Right);
            }
        }
        else if(Vector2.Distance(touchOnBegan, touchOnEnded) > 1f)//视为滑动
        {
            if (touchOnEnded.y - touchOnBegan.y > 0)
            {
                MoveDirection(PlayerDirection.Up);
            }
            else
            {
                MoveDirection(PlayerDirection.Down);
            }
        }
        touchOnBegan = touchOnEnded = Vector2.zero;
    }

}
