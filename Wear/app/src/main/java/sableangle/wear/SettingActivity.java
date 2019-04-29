package sableangle.wear;

import android.os.Bundle;
import android.support.wearable.activity.WearableActivity;
import android.view.View;
import android.widget.EditText;

public class SettingActivity extends WearableActivity {
    EditText ipText;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_setting);
        ipText = findViewById(R.id.ipText);
        ipText.setText(Utils.getPString(this,GlobalDefine.ipAddressKey));
    }
    public void OnFinishSetting(View view) {
        if(view.getId() == R.id.okButton){
            Utils.setPString(this,GlobalDefine.ipAddressKey,ipText.getText().toString());
        }
        // 寫要做的事...
        finish();
    }
}
