// =================================
//
//	Calibration.cs
//	Created by Takuya Himeji
//
// =================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReadyForPutEgg : MonoBehaviour
{
    #region Inspector Settings
    [SerializeField] private Transform floorObj;
    [SerializeField] private Transform mark;
    [SerializeField] private Image circleImage;
    [SerializeField] private Text heightText;
    [SerializeField] private Text noticeText;
    [SerializeField] private ParticleSystem effect;
    
    #endregion // Inspector Settings


    #region Member Field
    private float targetWaitTime = 10f;
    private float changeSceneTime = 2f;
    private float waitTimer = 0f;
    #endregion // Member Field


    #region MonoBehaviour Methods

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    #endregion // MonoBehaviour Methods


    #region Member Methods
    public void CalibrationUpdate(CalibrationData data)
    {
        
        if (data != null)
        {
            if (!mark.gameObject.activeSelf)
            {
                mark.gameObject.SetActive(true);
            }
            mark.position = data.hitPlaneInfo.point;
            heightText.text = (int)(data.height * 100f) + "cm";

            if (waitTimer < targetWaitTime)
            {
                waitTimer += Time.deltaTime;
                circleImage.fillAmount = waitTimer / targetWaitTime;
            }
            else
            {
                StartCoroutine(PutEgg(data));
                return;
            }
        }
        else if (mark.gameObject.activeSelf)
        {
            // Planeから視線が外れた場合、初期化
            waitTimer = 0f;
            circleImage.fillAmount = 0f;
            mark.gameObject.SetActive(false);
        }
    }

    private IEnumerator PutEgg(CalibrationData data)
    {
        //gvrTrackerCalibration.SetEyeHeight();

        effect.Play();
        noticeText.text = "識別完成，將轉移至遊戲場景";

        floorObj.gameObject.SetActive(true);
        floorObj.position = new Vector3(floorObj.position.x, data.hitPlaneInfo.point.y, floorObj.position.z);

        yield return new WaitForSeconds(changeSceneTime);
        mark.gameObject.SetActive(false);

        //SceneManager.LoadScene(nextScene);
        Destroy(gameObject);
    }

    #endregion // Member Methods
}