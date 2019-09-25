using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour
{
    //截图的相机 需要手动挂
    public Camera camera1;
    
    //public Image screenBg;

    //截图缩小图
     Sprite sprites;
    public RectTransform rect11;
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CameraScreenShot();
        }
        
    }
    /// <summary>
    /// 截图的按钮事件
    /// </summary>
    public void CameraScreenShot()
    {
        //调用截图方法
        CaptureCamera(camera1, rect11.rect);
        //调用给小截图添加图片
        //AddImageToScreenKuang();
    }
    Texture2D CaptureCamera(Camera camera, Rect rect)
    {
        //获取系统时间并命名相片名    
        DateTime now = DateTime.Now;
        string times = now.ToString();
        times = times.Trim();
        times = times.Replace(":", "");
        times = times.Replace("/", "");
        times = times.Remove(8, 1);
        string filename = "/Screenshot" + times + ".png";

        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 16);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();


        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.Destroy(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
#if UNITY_EDITOR
        string Path_save = Application.streamingAssetsPath + filename;
        //sprites = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector2(0.5f, 0.5f));
#elif UNITY_ANDROID
        string destination = "/storage/emulated/0/DCIM";  
        //判断目录是否存在，不存在则会创建目录    
        if (!Directory.Exists (destination)) {    
            Directory.CreateDirectory (destination);
            
        }    
        string Path_save = destination+"/" + filename;  
        sprites = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector2(0.5f, 0.5f));
#elif UNITY_IOS
 
#endif
        File.WriteAllBytes(Path_save, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", Path_save));
       
        return screenShot;
    }
    //添加截图缩小图
    //void AddImageToScreenKuang()
    //{
    //    screenBg.sprite = sprites;
    //    screenBg.gameObject.SetActive(true);
    //}
    //public void CancelClick()
    //{
    //    screenBg.gameObject.SetActive(false);
    //}
}
