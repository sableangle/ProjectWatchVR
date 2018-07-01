package sableangle.wear;

import android.content.Intent;
import android.os.Bundle;
import android.support.wearable.activity.WearableActivity;
import android.util.Log;
import android.view.WindowManager;
import android.widget.TextView;

public class MainActivity extends WearableActivity {

    public static final int PairResult = 1;


    private TextView mTextView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);


        mTextView = (TextView) findViewById(R.id.text);
        startActivityForResult(new Intent(this,PairActivity.class),PairResult);
        // Enables Always-on
        setAmbientEnabled();
    }


    // 回调方法，从第二个页面回来的时候会执行这个方法
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        String change01 = data.getStringExtra("change01");
        String change02 = data.getStringExtra("change02");
        // 根据上面发送过去的请求吗来区别
        switch (requestCode) {
            case PairResult:
                Log.d("asdfasf","asdfasfd");
                break;
            default:
                break;
        }
    }
}
