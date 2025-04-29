using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PrefaceManager : MonoBehaviour
{
    public AudioSource audioSource;

    public Transform player;

    public PostProcessVolume postVolume;

    private ColorGrading colorGrading;
    enum State
    {
        None,
        Start,
        PlayingPreface,
        FadingOut,
        LoadNextScene,
    } State state;

    public float fadeStrength = 1.0f; // 缩放强度（相对于原始尺寸）
    public float fadeSpeed = 3.1f;    // 缩放速度

    private float originalValue;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Start;

        if (postVolume.profile.TryGetSettings(out colorGrading))
        {
            // 初始化参数（可选）
            originalValue = colorGrading.postExposure.value; // colorGrading.temperature.value = 0f; // 默认色温
        }
    }

    // Update is called once per frame
    void Update()
    {


        if(player.position.z > 3 && state == State.Start)
        {
            audioSource.Play();
            state = State.PlayingPreface;
        }


        if (state == State.PlayingPreface && audioSource.time > 42.0f)
        {
            state = State.FadingOut;
            
        }

        

        if(state == State.LoadNextScene)
        {
            
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.LoadSceneWithIndex(1);
        }
    }
    private void FixedUpdate()
    {
        if (state == State.FadingOut)
        {
            colorGrading.postExposure.value += ((Time.fixedDeltaTime * fadeSpeed) * fadeStrength);

            if (colorGrading.postExposure.value > 17.0f)
            {
                GameManager.Instance.LoadSceneWithIndex(1);
                state = State.LoadNextScene;
            }
        }
    }
}
