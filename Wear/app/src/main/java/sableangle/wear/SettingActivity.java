package sableangle.wear;

import android.os.Bundle;
import android.support.wearable.activity.WearableActivity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

public class SettingActivity extends WearableActivity {
    EditText ipText;
    Button switchButton;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting);
        ipText = findViewById(R.id.ipText);
        ipText.setText(Utils.getPString(this,GlobalDefine.ipAddressKey));
    }
    public void OnSwitchButton(View view) {
        if(view.getId() == R.id.switch_button){
            float current = Utils.getPFloat(this,GlobalDefine.centerButtonRadius,0.75f);
            if(current > 0.51f){
                Utils.setPFloat(this,GlobalDefine.centerButtonRadius,0.5f);
            }
            else{
                Utils.setPFloat(this,GlobalDefine.centerButtonRadius,0.75f);
            }
        }
        // 寫要做的事...
        finish();
    }
    public void OnFinishSetting(View view) {
        if(view.getId() == R.id.okButton){
            Utils.setPString(this,GlobalDefine.ipAddressKey,ipText.getText().toString());
        }
        // 寫要做的事...
        finish();
    }
}
