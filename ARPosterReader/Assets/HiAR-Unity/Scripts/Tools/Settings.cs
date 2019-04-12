using System;
using Proyecto26;
using UnityEngine;
using UnityEditor;
using Models;

public class Settings
{
	public static string serverIP {get { return "shmily.me";}}
	public static string serverPort {get { return "8080";}}

	public static string server { get { return serverIP + ":" + serverPort;}}
}