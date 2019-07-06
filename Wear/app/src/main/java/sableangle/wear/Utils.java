package sableangle.wear;

import android.content.Context;
import android.content.SharedPreferences;

import java.util.Calendar;
import java.util.TimeZone;
import java.util.concurrent.TimeUnit;

public class Utils {
    public static String getPString(Context mContext, String key){
        SharedPreferences settings = mContext.getSharedPreferences("WearController", 0);
        if (settings == null) {
            return "";
        }
        return settings.getString(key, "");
    }

    public static void setPString(Context mContext, String key, String value){
        SharedPreferences settings = mContext.getSharedPreferences("WearController", 0);
        settings.edit().putString(key, value).apply();
    }
    public static float getPFloat(Context mContext, String key,float defaultValue){
        SharedPreferences settings = mContext.getSharedPreferences("WearController", 0);
        if (settings == null) {
            return defaultValue;
        }
        return settings.getFloat(key, defaultValue);
    }

    public static void setPFloat(Context mContext, String key, float value){
        SharedPreferences settings = mContext.getSharedPreferences("WearController", 0);
        settings.edit().putFloat(key, value).apply();
    }
    public static String timestamp()
    {
        TimeZone tz = TimeZone.getTimeZone("GMT+8");
        Calendar c = Calendar.getInstance(tz);
        long timeMillis = c.getTimeInMillis();
        //long timeMillis = System.currentTimeMillis();
        return Long.toString(TimeUnit.MILLISECONDS.toSeconds(timeMillis));
    }
}
