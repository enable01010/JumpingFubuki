using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class SceneAnimation : Singleton<SceneAnimation>
{
    Animator _anim;
    [SerializeField] float FADEIN_TIME = 0.5f;
    [SerializeField] float ANIMATION_TIME;
    [SerializeField] Slider _slider;
    private AsyncOperation async;
    float nowTime = 0;

    public bool isLoading { get; private set; } = false;

    public override void OnInitialize()
    {
        base.OnInitialize();

        _anim = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(int sceneNumbaer)
    {
        StartCoroutine(LoadStart(true, sceneNumbaer,null));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadStart(false, 0, sceneName));
    }

    IEnumerator LoadStart(bool isNumber,int number,string name)
    {
        nowTime = 0;
        isLoading = true;

        _anim.SetBool("FadeOut", false);
        _anim.SetBool("FadeIn",true);
        yield return new WaitForSeconds(FADEIN_TIME);

        if (isNumber == true)
        {
            async = SceneManager.LoadSceneAsync(number);
        }
        else
        {
            async = SceneManager.LoadSceneAsync(name);
        }

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (!async.isDone || nowTime < ANIMATION_TIME)
        {
            nowTime += Time.deltaTime;
            float timeRate = nowTime / ANIMATION_TIME;
            float progressVal = Mathf.Clamp01(async.progress / 0.9f);
            float lessValue = (timeRate > progressVal) ? progressVal : timeRate;
            if (_slider != null)
                _slider.value = lessValue;
            yield return null;
        }
        _anim.SetBool("FadeIn", false);
        _anim.SetBool("FadeOut", true);
        yield return new WaitForSeconds(ANIMATION_TIME);
        isLoading = false;
    }
}
