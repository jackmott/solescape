using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Notification : MonoBehaviour
{
    
    public dfLabel  notification;
    public Stack<string> notifications;

    float timeStamp;
    bool showing = false;
           

    private static Notification instance;
    



    void Awake()
    {
        instance = this;
        notifications = new Stack<string>();


    }


    public static Notification Instance
    {
        get
        {
            while (instance == null)
            {
                System.Threading.Thread.Sleep(1000);
                print("notification instance");

            }
            return instance;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    public void SetNotification(string text)
    {
        if (notifications.Count == 0)
        {
            notifications.Push(text);
        }
        else if (notifications.Peek() != text && notifications.Count <= 2)
        {
            notifications.Push(text);
        }
    }


    void Update()
    {
        if (notifications.Count > 0 && !showing)
        {
            notification.Text = notifications.Pop();
            timeStamp = Time.time;
            showing = true;
            notification.Show();
        } else if (showing)
        {
            if (Time.time - timeStamp > 2)
            {
                notification.Hide();
                showing = false;
            }
        }

    }
    

    
}
