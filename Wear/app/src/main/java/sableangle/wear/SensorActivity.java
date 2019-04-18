package sableangle.wear;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.PowerManager;
import android.support.wearable.activity.WearableActivity;
import android.util.Log;
import android.widget.TextView;

import java.util.List;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;
import sableangle.wear.sensor_fusion.CalibratedGyroscopeProvider;
import sableangle.wear.sensor_fusion.Quaternion;
import sableangle.wear.sensor_fusion.RotationVectorProvider;
import sableangle.wear.sensor_fusion.Vector3f;

public class SensorActivity extends WearableActivity  implements ButtonListener {

    private TextView mTextView;


    private SensorManager mSensorManager;
//    protected PowerManager.WakeLock mWakeLock;

    public static final String BUTTONS_NAME[]={"Center", "Up", "Down", "Right", "Left", "None"};
    //Android Life Cycle
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(new ButtonView(this, this, ViewType.PadButtonView, true));

        //mTextView = (TextView) findViewById(R.id.text);
        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);

        HardWareCheck(mSensorManager);

        ConnectWebSocket();
        StartTrackingSensor();
        BuildDataThread();

//        final PowerManager pm = (PowerManager) getSystemService(Context.POWER_SERVICE);
//        this.mWakeLock = pm.newWakeLock(PowerManager.SCREEN_DIM_WAKE_LOCK, "watch:wakelock");
//        this.mWakeLock.acquire();
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
//        this.mWakeLock.release();
        DataThread.destroy();
    }

    @Override
    public void onResume(){
        super.onResume();
        ConnectWebSocket();
        orientationProvider.start();
    }

    @Override
    public void onPause(){
        super.onPause();
        orientationProvider.stop();
    }

    public void HardWareCheck (SensorManager sensorManager) {
        if(sensorManager.getSensorList(Sensor.TYPE_GYROSCOPE).size() > 0) {
            Log.e("Support","Support");
        }
        else{
            Log.e("No Support","No Support");
        }
    }

    private Quaternion quaternion = new Quaternion();
    private float[] accelerometer =  new float[3];
    CalibratedGyroscopeProvider orientationProvider;
    void StartTrackingSensor(){
        orientationProvider = new CalibratedGyroscopeProvider(mSensorManager);
        orientationProvider.start();
    }

    private Thread DataThread = null;
    private static final int MAX_MILLIS_BETWEEN_UPDATES = 50;
    void BuildDataThread(){
        if(DataThread != null){
            DataThread.destroy();
        }
        DataThread = new Thread(new Runnable(){
            @Override
            public void run() {
                while(true){
                    try{
                        //小於間隔時間跳過
                        if(System.currentTimeMillis() - mLastOrientationSent < MAX_MILLIS_BETWEEN_UPDATES){
                            continue;
                        }
                        orientationProvider.getQuaternion(quaternion);
                        accelerometer = orientationProvider.getAccelerometerValues();
                        if(sensorSocket != null){
                            sensorSocket.send(
                                quaternion.getX() + "_" +  quaternion.getY()+ "_" + quaternion.getZ()+ "_" + quaternion.getW() + "@" +
                                accelerometer[0] + "_" +  accelerometer[1] + "_" + accelerometer[2]
                            );
                        }
                        mLastOrientationSent=System.currentTimeMillis();
                    }
                    catch(Exception e){
                        e.printStackTrace();
                    }
                }
            }
        });
        DataThread.start();
    }

    long mLastOrientationSent=0;
    private String WebSocketTAG = "";
    private String sensorUrl = "ws://192.168.0.5:24681/Sensor";
    private String inputUrl = "ws://192.168.0.5:24681/Input";

    private WebSocket sensorSocket;
    private WebSocket inputSocket;

    void DestoryWebSocket(){
        sensorSocket.cancel();
        inputSocket.cancel();
    }

    void ConnectWebSocket() {
        OkHttpClient client = new OkHttpClient.Builder()
                .build();
        //建立 WebSocketRequest
        Request sensorRequset = new Request.Builder()
                .url(sensorUrl)
                .build();

        //建立 感應器資料連線
        client.newWebSocket(sensorRequset, new WebSocketListener() {
            @Override
            public void onOpen(WebSocket webSocket, Response response) {
                sensorSocket = webSocket;
            }
            @Override
            public void onClosed(WebSocket webSocket, int code, String reason) {
                sensorSocket = null;
            }

            @Override
            public void onMessage(WebSocket webSocket, String text) {}
            @Override
            public void onClosing(WebSocket webSocket, int code, String reason) {}
            @Override
            public void onFailure(WebSocket webSocket, Throwable t, Response response) {}
        });

        //建立 WebSocketRequest
        Request inputRequset = new Request.Builder()
                .url(inputUrl)
                .build();

        //建立 輸入連線
        client.newWebSocket(inputRequset, new WebSocketListener() {
            @Override
            public void onOpen(WebSocket webSocket, Response response) {
                inputSocket = webSocket;
            }
            @Override
            public void onClosed(WebSocket webSocket, int code, String reason) {
                inputSocket = null;
            }

            @Override
            public void onMessage(WebSocket webSocket, String text) {}
            @Override
            public void onClosing(WebSocket webSocket, int code, String reason) {}
            @Override
            public void onFailure(WebSocket webSocket, Throwable t, Response response) {}
        });
    }

     String TagButtonEvent = "buttonEvent";
    @Override
    public void onButtonDown(ButtonName button,float x, float y) {
        Log.d(TagButtonEvent, "onButtonDown : " + button.toString());
        inputSocket.send("Down_"+  button.toString() + "," + x + "," + y + ",");
    }

    @Override
    public void onButtonUp(ButtonName button,float x, float y) {
        Log.d(TagButtonEvent,"onButtonUp : " + button.toString());
        inputSocket.send("Up_"+  button.toString() +"," + x + "," + y + ",");
    }

    @Override
    public void onButtonMove(float x, float y) {
        Log.d(TagButtonEvent,"onButtonMove : ");
        inputSocket.send("Move," + x + "," + y + ",");
    }
    @Override
    public void onButtonMoveStart(float x, float y) {
        Log.d(TagButtonEvent,"onButtonMoveStart : ");
        inputSocket.send("Start," + x + "," + y + ",");
    }
    @Override
    public void onButtonMoveEnd(float x, float y) {
        Log.d(TagButtonEvent,"onButtonMoveEnd : " );
        inputSocket.send("End," + x + "," + y + ",");
    }



    // Old
//    private SensorEventListener mOrientationListener;
//    float azimuth;
//    float pitch;
//    float roll;
//    boolean mSensorsActivated = false;
//    final int worldAxisForDeviceAxisX = SensorManager.AXIS_MINUS_Z;
//    final int worldAxisForDeviceAxisY = SensorManager.AXIS_X;
//    void MakeSensor(){
//
//
//        mOrientationListener=new SensorEventListener()
//        {
//            float [] mGravityValues;
//            float [] mGeoMagneticValues;
//
//            float [] R=new float[9];
//            float [] orientation=new float[3];
//            float [] adjustedRotationMatrix = new float[9];
//
//            @Override
//            public void onSensorChanged(SensorEvent event)
//            {
//                switch(event.sensor.getType())
//                {
//                    case Sensor.TYPE_ACCELEROMETER:
//                        mGravityValues =event.values;
//                        break;
//
//                    case Sensor.TYPE_MAGNETIC_FIELD:
//                        mGeoMagneticValues =event.values;
//                        break;
//                }
//
//                if(System.currentTimeMillis()-mLastOrientationSent < MAX_MILLIS_BETWEEN_UPDATES){
//                    return;
//                }
//
//                //Log.d("Send","Send");
//                if(mGeoMagneticValues !=null && mGravityValues !=null)
//                {
//                    if(SensorManager.getRotationMatrix(R, null, mGravityValues, mGeoMagneticValues))
//                    {
//
//                        SensorManager.remapCoordinateSystem(R, worldAxisForDeviceAxisX, worldAxisForDeviceAxisY, adjustedRotationMatrix);
//                        SensorManager.getOrientation(adjustedRotationMatrix, orientation);
//
//                        azimuth = orientation[0] * 57.2957795f; //looks like we don't need this one
//                        pitch =orientation[1] * 57.2957795f;
//                        roll = orientation[2] * 57.2957795f;
//                        if(mWebSocket != null){
//                            mWebSocket.send(azimuth + "_" +  pitch+ "_" + roll);
//                        }
//                        mLastOrientationSent=System.currentTimeMillis();
//                    }
//                }
//            }
//
//            @Override
//            public void onAccuracyChanged(Sensor sensor, int i) {}
//        };
//
//        ActiveSensors();
//    }

//    public void ActiveSensors()
//    {
//        if(!mSensorsActivated)
//        {
//            List<Sensor> sensorsAcelerometer = mSensorManager.getSensorList(Sensor.TYPE_ACCELEROMETER);
//            List<Sensor> sensorsMagnetic = mSensorManager.getSensorList(Sensor.TYPE_MAGNETIC_FIELD);
//            if (sensorsAcelerometer.size() > 0 && sensorsMagnetic.size()>0)
//            {
//                mSensorManager.registerListener(mOrientationListener, sensorsAcelerometer.get(0), SensorManager.SENSOR_DELAY_GAME);
//                mSensorManager.registerListener(mOrientationListener, sensorsMagnetic.get(0), SensorManager.SENSOR_DELAY_GAME);
//                mSensorsActivated = true;
//            }
//        }
//    }
}
