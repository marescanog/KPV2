using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFaceDirectionUI : MonoBehaviour
{
    [SerializeField] SpriteRenderer _up;
    [SerializeField] SpriteRenderer _left;
    [SerializeField] SpriteRenderer _right;
    [SerializeField] SpriteRenderer _down;

    private void HideAll()
    {
        if(_up != null)
        {
            _up.enabled = false;
        }
        if (_left != null)
        {
            _left.enabled = false;
        }
        if (_right != null)
        {
            _right.enabled = false;
        }
        if (_down != null)
        {
            _down.enabled = false;
        }
    }

    public void ShowArrowUI(Vector2 faceDirection)
    {
        HideAll();
        if (faceDirection == Vector2.up)
        {
            _up.enabled = true;
        }
        if (faceDirection == Vector2.down)
        {
            _down.enabled = true;
        }
        if (faceDirection == Vector2.left)
        {
            _left.enabled = true;
        }
        if (faceDirection == Vector2.right)
        {
            _right.enabled = true;
        }
    }
}
