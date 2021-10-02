using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("移動中かどうかを判別する変数")]
    [SerializeField] bool isMoving;

    int axisX;
    int axisY;

    void Start()
    {
    }

    void Update()
    {
        axisX = (int)Input.GetAxisRaw("Horizontal");
        axisY = (int)Input.GetAxisRaw("Vertical");

        if (axisX != 0)
        {
            axisY = 0;

            if (axisX < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (axisX > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else if (axisY != 0)
        {
            axisX = 0;
        }

        if (axisX != 0 || axisY != 0)
        {
            // 移動
            PlayerMove(axisX, axisY);
        }
    }

    public void PlayerMove(int x, int y)
    {
        Vector2 currentPos = transform.position;                // Playerの現在座標
        Vector2 targetPos = currentPos + new Vector2(x, y);     // 移動先の座標

        if (!isMoving)
        {
            // 移動用のコルーチン
            StartCoroutine(PlayerMovement(targetPos));
        }
    }

    IEnumerator PlayerMovement(Vector3 targetPos)
    {
        isMoving = true;

        float distance = (transform.position - targetPos).sqrMagnitude; // 現在地と目的地との距離
        while (distance > float.Epsilon)
        {
            Vector3 currentPos = this.gameObject.transform.position;
            transform.position = Vector3.MoveTowards(currentPos, targetPos, 6f * Time.deltaTime);
            distance = (transform.position - targetPos).sqrMagnitude;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}
