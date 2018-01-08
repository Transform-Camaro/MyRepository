using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

public class PrintScreen : MonoBehaviour
{
    //[DllImport("__Internal")]
    //private static extern void _SavePhoto(string readAddr);

    /// <param name="camera">Camera.要被截屏的相机</param>  
    /// <param name="rect">Rect.截屏的区域</param>  
    public static string CaptureCamera(Camera camera, Rect rect)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
        //ps: camera2.targetTexture = rt;  
        //ps: camera2.Render();  
        //ps: -------------------------------------------------------------------  

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

        System.DateTime now = new System.DateTime();
        now = System.DateTime.Now;
        string PhotoPath = Application.persistentDataPath + "/ARphoto";

        if (!Directory.Exists(PhotoPath))
        {
            Directory.CreateDirectory(PhotoPath);
        }

        string filename = string.Format(PhotoPath + "/ARphoto{0}{1}{2}{3}.png", now.Month, now.Day, now.Hour, now.Minute);

        // Debug.Log("Application.persistentDataPath = " + Application.persistentDataPath);
        File.WriteAllBytes(filename, bytes);//图片写入本地


        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    SaveAndroid(filename, bytes);
        //}
        //else
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            SaveIos(filename);
        }
        return filename;
    }
    private void SaveAndroid(string name, byte[] bytes)
    {

        string path = "/mnt/sdcard/DCIM/ARphoto";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = path + "/" + name;

        File.WriteAllBytes(path, bytes);
    }

    private static void SaveIos(string filename)
    {
        //  string path_save = Application.persistentDataPath + "/" + name;

        // File.WriteAllBytes(path_save, bytes);

        // _SavePhoto(filename);
    }
}
