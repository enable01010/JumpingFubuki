using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] int number = 1;
    public void OnClicked()
    {
        SceneAnimation.instance.LoadScene(number);
    }
}
