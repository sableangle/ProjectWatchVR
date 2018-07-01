package sableangle.wear;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.support.wearable.activity.WearableActivity;
import android.text.format.Formatter;
import android.widget.ImageView;
import android.widget.Toast;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.EncodeHintType;
import com.google.zxing.WriterException;
import com.google.zxing.qrcode.decoder.ErrorCorrectionLevel;
import com.journeyapps.barcodescanner.BarcodeEncoder;

import java.util.EnumMap;
import java.util.Map;

import okhttp3.Response;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;
import okhttp3.mockwebserver.MockResponse;
import okhttp3.mockwebserver.MockWebServer;

public class PairActivity extends WearableActivity {

    private ImageView mImageView;
    Activity mActivity;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_pair);

        mImageView = (ImageView) findViewById(R.id.qrImageView);

        GenerateQrCode();
        //開啟 WebSocketServer
        StartWebSocketServer();
        mActivity = this;
        // Enables Always-on
        setAmbientEnabled();
    }
    @Override
    protected void onDestroy() {
        super.onDestroy();
        try{
            pairWebSocketServer.shutdown();
            Toast.makeText(this, "Get Data Success", Toast.LENGTH_LONG).show();

        }
        catch (Exception ex){
            Toast.makeText(this, "Get Data Faild", Toast.LENGTH_LONG).show();
        }
    }
    MockWebServer pairWebSocketServer;
    String phoneIp;
    void StartWebSocketServer(){
        pairWebSocketServer = new MockWebServer();

        pairWebSocketServer.enqueue(new MockResponse().withWebSocketUpgrade(new WebSocketListener() {
            @Override
            public void onOpen(WebSocket webSocket, Response response) {
                System.out.println("server onOpen");
                System.out.println("server request header:" + response.request().headers());
                System.out.println("server response header:" + response.headers());
                System.out.println("server response:" + response);


            }
            @Override
            public void onMessage(WebSocket webSocket, String msg) {
                System.out.println("server onMessage");
                System.out.println("message:" + msg);
                //收到有ip訊息的資料
                if(msg.contains("ip")){


                    String[] ips = msg.split("_");
                    phoneIp = ips[1];
                    Intent mIntent = new Intent();
                    mIntent.putExtra("phoneIp", phoneIp);
                    mActivity.setResult(MainActivity.PairResult, mIntent);
                    mActivity.finish();
                }
            }
            @Override
            public void onClosing(WebSocket webSocket, int code, String reason) {
                System.out.println("server onClosing");
                System.out.println("code:" + code + " reason:" + reason);
            }
            @Override
            public void onClosed(WebSocket webSocket, int code, String reason) {
                System.out.println("server onClosed");
                System.out.println("code:" + code + " reason:" + reason);
            }
            @Override
            public void onFailure(WebSocket webSocket, Throwable t, Response response) {
                //出现异常会进入此回调
                System.out.println("server onFailure");
                System.out.println("throwable:" + t);
                System.out.println("response:" + response);
            }
        }));

    }

    void GenerateQrCode(){
        BarcodeEncoder encoder = new BarcodeEncoder();
        Map hints = new EnumMap(EncodeHintType.class);
        hints.put(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
        try {
            Bitmap bit = encoder.encodeBitmap( GetIp(), BarcodeFormat.QR_CODE, 320, 320,hints);
            mImageView.setImageBitmap(bit);
        } catch (WriterException e) {
            e.printStackTrace();
        }
    }

    String GetIp(){
        WifiManager wifiMgr = (WifiManager) getApplicationContext().getSystemService(WIFI_SERVICE);
        WifiInfo wifiInfo = wifiMgr.getConnectionInfo();
        int ip = wifiInfo.getIpAddress();
        return Formatter.formatIpAddress(ip);
    }
}
