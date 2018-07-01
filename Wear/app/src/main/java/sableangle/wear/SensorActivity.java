package sableangle.wear;

import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.support.wearable.activity.WearableActivity;
import android.widget.TextView;

import java.util.List;

import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;
import okhttp3.WebSocket;
import okhttp3.WebSocketListener;

public class SensorActivity extends WearableActivity {

    private TextView mTextView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sensor);

        mTextView = (TextView) findViewById(R.id.text);


        ConnectWebSocket();
        MakeSensor();
        // Enables Always-on
        setAmbientEnabled();
    }
    private static final int MAX_MILLIS_BETWEEN_UPDATES = 50;
    private SensorManager mSensorManager;
    private SensorEventListener mOrientationListener;
    float azimuth;
    float pitch;
    float roll;

    final int worldAxisForDeviceAxisX = SensorManager.AXIS_MINUS_Z;
    final int worldAxisForDeviceAxisY = SensorManager.AXIS_X;
    void MakeSensor(){

        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);

        mOrientationListener=new SensorEventListener()
        {
            float [] mGravityValues;
            float [] mGeoMagneticValues;
            long mLastOrientationSent=0;

            float [] R=new float[9];
            float [] orientation=new float[3];
            float [] adjustedRotationMatrix = new float[9];

            @Override
            public void onSensorChanged(SensorEvent event)
            {
                switch(event.sensor.getType())
                {
                    case Sensor.TYPE_ACCELEROMETER:
                        mGravityValues =event.values;
                        break;

                    case Sensor.TYPE_MAGNETIC_FIELD:
                        mGeoMagneticValues =event.values;
                        break;
                }

                if(System.currentTimeMillis()-mLastOrientationSent < MAX_MILLIS_BETWEEN_UPDATES){
                    return;
                }

                //Log.d("Send","Send");
                if(mGeoMagneticValues !=null && mGravityValues !=null)
                {
                    if(SensorManager.getRotationMatrix(R, null, mGravityValues, mGeoMagneticValues))
                    {

                        SensorManager.remapCoordinateSystem(R, worldAxisForDeviceAxisX, worldAxisForDeviceAxisY, adjustedRotationMatrix);
                        SensorManager.getOrientation(adjustedRotationMatrix, orientation);

                        azimuth = orientation[0] * 57.2957795f; //looks like we don't need this one
                        pitch =orientation[1] * 57.2957795f;
                        roll = orientation[2] * 57.2957795f;
                        if(mWebSocket != null){
                            mWebSocket.send(azimuth + "_" +  pitch+ "_" + roll);
                        }
                        mLastOrientationSent=System.currentTimeMillis();
                    }
                }
            }

            @Override
            public void onAccuracyChanged(Sensor sensor, int i) {}
        };

        ActiveSensors();
    }
    boolean mSensorsActivated = false;
    public void ActiveSensors()
    {
        if(!mSensorsActivated)
        {
            List<Sensor> sensorsAcelerometer = mSensorManager.getSensorList(Sensor.TYPE_ACCELEROMETER);
            List<Sensor> sensorsMagnetic = mSensorManager.getSensorList(Sensor.TYPE_MAGNETIC_FIELD);
            if (sensorsAcelerometer.size() > 0 && sensorsMagnetic.size()>0)
            {
                mSensorManager.registerListener(mOrientationListener, sensorsAcelerometer.get(0), SensorManager.SENSOR_DELAY_GAME);
                mSensorManager.registerListener(mOrientationListener, sensorsMagnetic.get(0), SensorManager.SENSOR_DELAY_GAME);
                mSensorsActivated = true;
            }
        }
    }
    private String WebSocketTAG = "";
    private String wsUrl = "ws://192.168.0.131:24681/Rotation";
    private WebSocket mWebSocket;
    void ConnectWebSocket() {
        OkHttpClient client = new OkHttpClient.Builder()
                .build();
        //构造request对象
        Request request = new Request.Builder()
                .url(wsUrl)
                .build();
        //建立连接
        client.newWebSocket(request, new WebSocketListener() {
            @Override
            public void onOpen(WebSocket webSocket, Response response) {
                mWebSocket = webSocket;
                System.out.println("client onOpen");
                System.out.println("client request header:" + response.request().headers());
                System.out.println("client response header:" + response.headers());
                System.out.println("client response:" + response);

            }

            @Override
            public void onMessage(WebSocket webSocket, String text) {
                System.out.println("client onMessage");
                System.out.println("message:" + text);
            }

            @Override
            public void onClosing(WebSocket webSocket, int code, String reason) {
                System.out.println("client onClosing");
                System.out.println("code:" + code + " reason:" + reason);
            }

            @Override
            public void onClosed(WebSocket webSocket, int code, String reason) {
                System.out.println("client onClosed");
                System.out.println("code:" + code + " reason:" + reason);
            }

            @Override
            public void onFailure(WebSocket webSocket, Throwable t, Response response) {
                //出现异常会进入此回调
                System.out.println("client onFailure");
                System.out.println("throwable:" + t);
                System.out.println("response:" + response);
            }
        });
    }
}
