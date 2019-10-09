using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerBehaviour : MonoBehaviour
{
    [SerializeField] private Text text;

    private string previousLog;

    private bool isStarted;

    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif

        // 状態の表示
        {
            var log = $"LocationService/.isEnabledByUser = {Input.location.isEnabledByUser}\n";
            log += $"LocationService.status = {Input.location.status}\n";
//            if (Input.location.status == LocationServiceStatus.Running)
            {
                log += "\n";
                log += $"LocationService.lastData.latitude = {Input.location.lastData.latitude}\n";
                log += $"LocationService.lastData.longitude = {Input.location.lastData.longitude}\n";
                log += $"LocationService.lastData.altitude = {Input.location.lastData.altitude}\n";
                log += $"LocationService.lastData.horizontalAccuracy = {Input.location.lastData.horizontalAccuracy}\n";
                log += $"LocationService.lastData.verticalAccuracy = {Input.location.lastData.verticalAccuracy}\n";
                log += $"LocationService.lastData.timestamp = {Input.location.lastData.timestamp}";
            }

            text.text = log;

            if (previousLog != log)
            {
                Debug.Log($"{System.DateTime.Now}\n{log}");
                previousLog = log;
            }
        }

#if UNITY_ANDROID
        // Androidでは、パーミッションの許可ダイアログで「許可」を選んだ際にLocationServiceが自動では起動しないので、
        // ここでもう一度起動する
        if (isStarted && Input.location.status == LocationServiceStatus.Stopped && Input.location.isEnabledByUser)
        {
            Input.location.Start();
        }
#endif
    }

    public void OnClickStart()
    {
        Input.location.Start();
        isStarted = true;

//        StartCoroutine(LocationServiceStart());
    }

    public void OnClickStop()
    {
        Input.location.Stop();
        isStarted = false;
    }

    private IEnumerator LocationServiceStart()
    {
#if UNITY_IOS
        // iOSでisEnabledByUserがfalseの場合は、「設定 > プライバシー > 位置情報サービス」で位置情報サービスがオフ
        if (Input.location.isEnabledByUser == false)
        {
            yield break;
        }
#endif

        // すでに起動中なので終了
        if (Input.location.status == LocationServiceStatus.Running)
        {
            yield break;
        }

        Input.location.Start();

        // 初期化が終了するまで待つ
        var maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            --maxWait;
        }

        // タイムアウトによる終了
        if (maxWait < 1)
        {
            yield break;
        }
    }
}
