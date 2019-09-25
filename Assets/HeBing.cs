using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;




public class HeBing : MonoBehaviour
{
    public Texture2D[] t = new Texture2D[4];
    public Image image;
    void Start()
    {
        //加载图片
        //Texture2D[] t = Resources.LoadAll<Texture2D>("Icos");

        //合成图片
        Texture2D tex = MergeImage(t);

        //生成后的图自适应大小
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(tex.width, tex.height);

        //显示图片
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        image.GetComponent<Image>().sprite = sp;
    }

    /// <summary>
    /// @brief 多张Texture2D合成一张Texture2D
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public Texture2D MergeImage(Texture2D[] tex)
    {
        //这里测试合成图片所花的时间
        //TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);

        if (tex.Length == 0)
        {
            return null;
        }
        //定义新图的宽高
        int width = 0, height = 0;

        for (int i = 0; i < tex.Length; i++)
        {
            //Debug.Log(tex[i].ToString());
            //新图的宽度
            width += tex[i].width;
            if (i > 0)
            {
                //新图的高度，这里筛选为最高
                if (tex[i].height > tex[i - 1].height)
                {
                    height = tex[i].height;            
                }                    
            }
            else height = tex[i].height; //只有一张图
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
                texture2D.SetPixels32(x += tex[i - 1].width, y, tex[i].width, tex[i].height, color);
            }
            else
            {
                texture2D.SetPixels32(x, y, tex[i].width, tex[i].height, color);
            } 
        }

        //应用
        texture2D.Apply();

        //TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);
        //TimeSpan ts = ts2.Subtract(ts1).Duration();
        ////查看合成后的图所花时间 ms单位
        //Debug.Log(ts.TotalMilliseconds);

        return texture2D;
    }
}

