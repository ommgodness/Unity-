using System.Collections;
using UnityEngine;
using System;
using System.IO;




public class DuoCiJieTuPingJie : MonoBehaviour
{
    //自动长截图
    public RectTransform Conent;

    //拼接图片
    private Texture2D[] t = new Texture2D[4];
    

    
    //截屏   
    public Camera camera1;//截图的相机 需要手动挂
    Texture2D screenShot;//储存截屏的图片
    int ZhangShu = 1;//记录截图张数，以免截图被覆盖
    int x = 0;//用于数组存放四张图片；

    float y = -0.7f;//滑动面板的初始位置

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ChangJieTu());
        }
        
    }

    //长截图自动截图四张
    IEnumerator ChangJieTu()
    {
        for (int i = 0; i < 4; i++)
        {           
            Conent.transform.position = new Vector3(0, y, 1);
            yield return  StartCoroutine(CameraScreenShot());
            t[i] = screenShot;
            Debug.Log(Conent.transform.position);
            y += 1.1f;
        }
        PinJieTuPian();
        y = -0.7f;
    }


    //截屏
    IEnumerator  CameraScreenShot()
    {
        yield return new WaitForEndOfFrame();
        //调用截图方法
        CaptureCamera(camera1, new Rect(0, 0, Screen.width, Screen.height));            
    }
    Texture2D CaptureCamera(Camera camera, Rect rect)//截图方法
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 16);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();
        

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
        GameObject.Destroy(rt);     
          return screenShot;
    }

   
    //拼接图片
    public void PinJieTuPian()
    {
        //获取系统时间并命名相片名    
        DateTime now = DateTime.Now;
        string times = now.ToString();
        times = times.Trim();
        times = times.Replace(":", "");
        times = times.Replace("/", "");
        times = times.Remove(8, 1);

        string filename = "/Screenshot" + times + ZhangShu + ".png";
        //数组元素顺序颠倒
        Debug.Log(t.Length);
        int j = 0;
        Texture2D[] t1 = new Texture2D[t.Length];
        for (int i = t.Length-1; i >= 0; i--,j++)
        {
            t1[j] = t[i];
        }
        //MergeImage(t1);
        //合成图片
        Texture2D tex = MergeImage(t1);

        byte[] bytes = tex.EncodeToJPG();

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
        //sprites = Sprite.Create(screenShot, new Rect(0, 0, screenShot.width, screenShot.height), new Vector2(0.5f, 0.5f));
#elif UNITY_IOS
 
#endif
        File.WriteAllBytes(Path_save, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", Path_save));
        ZhangShu += 1;
    }
    /// <summary>
    /// @brief 多张Texture2D合成一张Texture2D
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public Texture2D MergeImage(Texture2D[] tex)
    {       
        if (tex.Length == 0)
        {
            return null;
        }
        //定义新图的宽高
        int width = 0, height = 0;

        for (int i = 0; i < tex.Length; i++)
        {
            //Debug.Log(tex[i].ToString());
            //新图的高度
            height += tex[i].height;
            if (i > 0)
            {
                //新图的宽度，这里筛选为最宽
                if (tex[i].width > tex[i - 1].width)
                {
                    width = tex[i].width;
                }
            }
            else width = tex[i].width; //只有一张图
        }

        //初始Texture2D
        Texture2D texture2D = new Texture2D(width, height);

        int x = 0, y = 0;
        for (int i = 0; i < tex.Length; i++)
        {
            //取图
            Color32[] color = tex[i].GetPixels32(0);

            //赋给新图
            if (i > 0)
            {
                texture2D.SetPixels32(x, y+=tex[i-1].height, tex[i].width, tex[i].height, color);
            }
            else
            {
                texture2D.SetPixels32(x, y, tex[i].width, tex[i].height, color);
            }
        }

        //应用
        texture2D.Apply();
       
        return texture2D;
    }
}

