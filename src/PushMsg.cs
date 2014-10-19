using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//	PushMsg.cs
//	Author: Lu Zexi
//	2014-08-08



/// <summary>
/// Push message.
/// </summary>
public class PushMsg : MonoBehaviour
{
	private const string TOKEN_KEY = "PushMsg_Token";

	public delegate void MESSAGE_ACTION(IDictionary idc );
	public MESSAGE_ACTION MsgAction = null;	//local action
	public string URL = "";	//http://www.luzexi.com/?name=ddd&

	private static PushMsg s_cInstance;
	public static PushMsg sInstance
	{
		get
		{
			if(s_cInstance == null)
			{
				GameObject obj = new GameObject("PushMsg");
				s_cInstance = obj.AddComponent<PushMsg>();
			}
			return s_cInstance;
		}
	}

	void OnDestroy()
	{
		s_cInstance = null;
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	public void Init()
	{
		//DateTime dt = 
		string strTicks = PlayerPrefs.GetString(TOKEN_KEY);
		long dtTicks = strTicks == "" ? 0 : long.Parse(strTicks);
		if(DateTime.Now.Ticks - dtTicks >= 24L*3600L*60L*1000L*10000L)
		{
			NotificationServices.RegisterForRemoteNotificationTypes(RemoteNotificationType.Alert | 
			                                                        RemoteNotificationType.Badge | 
			                                                        RemoteNotificationType.Sound);
			StartCoroutine("RegistRemote");
			PlayerPrefs.SetString(TOKEN_KEY , "" + DateTime.Now.Ticks);
		}
	}

	//clear the notifications
	public void ClearNotifications()
	{
		NotificationServices.ClearLocalNotifications();
		NotificationServices.ClearRemoteNotifications();
	}

	/// <summary>
	/// Pushs the local message.
	/// </summary>
	/// <param name="dtime">Dtime.</param>
	/// <param name="alertBody">Alert body.</param>
	/// <param name="alertAction">Alert action.</param>
	/// <param name="alertLaunchImage">Alert launch image.</param>
	/// <param name="applicationIconBadgeNumber">Application icon badge number.</param>
	/// <param name="soundName">Sound name.</param>
	/// <param name="userinfo">Userinfo.</param>
	public void PushLocalMsg( DateTime dtime , string alertBody ,string alertAction = "" ,
	                         string alertLaunchImage = "", int applicationIconBadgeNumber = -1 ,
	                         string soundName = "" , IDictionary userinfo = null
	                         )
	{
		LocalNotification noti = new LocalNotification();
		noti.alertBody = alertBody;
		noti.fireDate = dtime;

		if(alertAction != "" )
			noti.alertAction = alertAction;
		if(alertLaunchImage != "")
			noti.alertLaunchImage = alertLaunchImage;
		if(applicationIconBadgeNumber != -1 )
			noti.applicationIconBadgeNumber = applicationIconBadgeNumber;
		if(soundName != "")
			noti.soundName = soundName;
		if(userinfo != null)
			noti.userInfo = userinfo;

		if(dtime.Ticks == DateTime.Now.Ticks)
			NotificationServices.PresentLocalNotificationNow(noti);
		else
			NotificationServices.ScheduleLocalNotification(noti);
	}

	/// <summary>
	/// Regists the remote.
	/// </summary>
	/// <returns>The remote.</returns>
	private IEnumerator RegistRemote()
	{
		for( ;NotificationServices.deviceToken == null; )
		{
			yield return new WaitForSeconds(0.1f);
		}
		WWW www = new WWW(URL + "token="+NotificationServices.deviceToken);

		yield return www;
	}


	// Update is called once per frame
	void Update ()
	{
		if (NotificationServices.localNotificationCount > 0)
		{
			LocalNotification noti = NotificationServices.localNotifications[NotificationServices.localNotificationCount-1];
			if(MsgAction!= null)
			{
				MsgAction(noti.userInfo);
			}
			NotificationServices.ClearLocalNotifications();
		}
		if(NotificationServices.remoteNotificationCount > 0 )
		{
			RemoteNotification noti = NotificationServices.remoteNotifications[NotificationServices.remoteNotificationCount-1];
			if(MsgAction!= null)
			{
				MsgAction(noti.userInfo);
			}
			NotificationServices.ClearRemoteNotifications();
		}
	}
}

