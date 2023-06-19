using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour
{
    [SerializeField] Transform barTransform;
    [SerializeField] GameObject bar;
    public void SetSize(float size)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (size > 1f )
        {
            size = 1;
        }

        if (size < 0)
        {
            size = 0;
        }

        barTransform.localScale = new Vector3(size, 1f);

    }

    public void ResetBar()
    {
        barTransform.localScale = new Vector3(0, 1f);
    }

    public void EnableBar()
    {
        bar.SetActive(true);
    }

    public void DisableBar()
    {
        gameObject.SetActive(false);
    }

    public void SetBarColor(Color newColor)
    {
        // To Do
    }


}
