using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class AsyncServer : MonoBehaviour {
	private const string RotationFunction = "Rotation";
    private byte[] data = new byte[1024];
    private int size = 1024;
    private Socket server;
    private Socket client;
    public static Vector4 rotationEularAngel;

    void Start () {
        server = new Socket(AddressFamily.InterNetwork,
                  SocketType.Stream, ProtocolType.Tcp);

    	IPAddress ipAd = IPAddress.Parse("192.168.0.131");
        //IPEndPoint iep = new IPEndPoint(IPAddress.Any, 1234);
        IPEndPoint iep = new IPEndPoint(ipAd, 1234);
        server.Bind(iep);
        server.Listen(5);
		Debug.Log("Start Socket");
        server.BeginAccept(new AsyncCallback(AcceptConn), server);

    }
	void OnDestroy(){
		
	}
    void AcceptConn(IAsyncResult iar)
    {
        Socket oldserver = (Socket)iar.AsyncState;
        client = oldserver.EndAccept(iar);
        Debug.Log("Accepted client: " + client.ToString());
        client.BeginReceive(data, 0, size, SocketFlags.None,
            new AsyncCallback(ReceiveData), client);
    }

    void SendData(IAsyncResult iar)
    {
        Socket client = (Socket)iar.AsyncState;
        int sent = client.EndSend(iar);
        client.BeginReceive(data, 0, size, SocketFlags.None,
                    new AsyncCallback(ReceiveData), client);
    }

    void ReceiveData(IAsyncResult iar)
    {
        Socket client = (Socket)iar.AsyncState;
        int recv = client.EndReceive(iar);
        if (recv == 0)
        {
            client.Close();
            Debug.LogError("Waiting for client...");
            server.BeginAccept(new AsyncCallback(AcceptConn), server);
            return;
        }
        string receivedData = Encoding.ASCII.GetString(data, 0, recv);
    	Debug.Log(receivedData);

        {
            var functions = receivedData.Split('_');
			string function = "";
			if(function.Length > 0){
				function = functions[0];
			}
            
			if(!string.IsNullOrEmpty(function)){
				if(function == RotationFunction){
					var content = functions[1].Split('#');
					rotationEularAngel = new Vector4(int.Parse(content[0]),int.Parse(content[1]),int.Parse(content[2]));
				}
			}

			//  float xx = float.Parse(s[0]);
            //     float yy = float.Parse(s[1]);
            //     float zz = float.Parse(s[2]);
            //     s[3] = s[3].Replace("/","");
            //     float ww = float.Parse(s[3]);
            //     Debug.Log("len: " + recv + ", " + xx + ", " + yy + ", " + zz + ", " + ww);
     
        }

        client.BeginReceive(data, 0, size, SocketFlags.None,
            new AsyncCallback(ReceiveData), client);
    }
}