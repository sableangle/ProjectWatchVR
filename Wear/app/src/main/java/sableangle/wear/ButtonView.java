package sableangle.wear;


import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Path;
import android.view.MotionEvent;
import android.view.View;

/**
 * Created by Usuario on 05/08/2016.
 * Modified by Miki on 04/07/2018.
 */

enum ButtonName {Center, Up, Down, Right, Left, None}

enum ViewType {HorizontalButtonView, PadButtonView, VerticalButtonView}

interface ButtonListener
{
    public void onButtonDown(ButtonName PressedButton,float x,float y);
    public void onButtonUp(ButtonName HoldButton,float x,float y);
    public void onButtonMove(float x,float y);
    public void onButtonMoveStart(float x,float y);
    public void onButtonMoveEnd(float x,float y);
}

public class ButtonView extends View
{
    protected boolean isMoving = false;
    protected ButtonListener mButtonListener;
    protected boolean mCenterEnabled=true;
    protected int mWidth=0, mHeight=0;
    protected float MidScreenWidth =0, MidScreenHeight =0;
    protected float centerButtonRadius = 0;
    protected final float movingGate = 5;

    protected int strokeWidth = 10;
    protected int arrowStrokeWidth = 5;

    protected float MidButtonWidth = 100, MidButtonHeight = 100;
    protected Path mUpButtonPath, mDownButtonPath, mRightButtonPath, mLeftButtonPath, mArrowsPath;
    protected Path touchPath;

    protected ViewType mViewType;

    public ButtonName mButtonPressed = ButtonName.None;
    public ButtonName getPressedButton(){return mButtonPressed;}

    public ButtonView(Context context, ButtonListener _listener, ViewType _type, boolean _centerButton) {
        super(context); mButtonListener =_listener; mViewType =_type; mCenterEnabled =_centerButton;
    }

    public ButtonView(Context context, ButtonListener _listener, ViewType _type)
    {
        super(context); mButtonListener=_listener; mViewType =_type;
    }

    public void setCenterButtonEnabled(boolean x)
    {
        mCenterEnabled=x;
        this.invalidate();
    }



    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh)
    {
        super.onSizeChanged(w, h, oldw, oldh);

        mWidth=this.getWidth();
        mHeight=this.getHeight();

        MidScreenWidth =mWidth/2.0f;
        MidScreenHeight =mHeight/2.0f;

        MidButtonWidth =mWidth/3.0f;
        MidButtonHeight =mHeight/3.0f;

        centerButtonRadius = MidScreenHeight*0.6f;

        initializePaths();
    }

    private Paint CreatePaintWithAntialias()
    {
        Paint _paint = new Paint();
        //Enabling antialias
        _paint.setDither(true);                    // set the dither to true
        _paint.setStrokeJoin(Paint.Join.ROUND);    // set the join to round you want
        _paint.setStrokeCap(Paint.Cap.ROUND);      // set the paint cap to round too
        _paint.setAntiAlias(true);                 // set anti alias so it smooths

        return _paint;
    }

    @Override
    protected void onDraw(Canvas canvas)
    {

        Paint _touchPaint = CreatePaintWithAntialias();

        Paint _mainPaint = CreatePaintWithAntialias();
        Paint _linesPaint = CreatePaintWithAntialias();
        Paint _arrowLinesPaint = CreatePaintWithAntialias();

        int _backgroundColor= Color.LTGRAY;
        int _linesColor=Color.GRAY;
        int _arrowFilledColor=Color.WHITE;
        int _centralButtonPressedColor=Color.GRAY;
        int _centralButtonUnpressedColor=Color.DKGRAY;

        canvas.drawColor(_backgroundColor);

        _linesPaint.setColor(_linesColor);
        _linesPaint.setStyle(Paint.Style.STROKE);
        _linesPaint.setStrokeWidth(strokeWidth);

        _arrowLinesPaint.setColor(_linesColor);
        _arrowLinesPaint.setStyle(Paint.Style.STROKE);
        _arrowLinesPaint.setStrokeWidth(arrowStrokeWidth);

        _touchPaint.setColor(Color.RED);
        _touchPaint.setStyle(Paint.Style.STROKE);
        _touchPaint.setStrokeWidth(3);

        canvas.drawColor(_backgroundColor);

        _mainPaint.setColor(_linesColor);
        _mainPaint.setStyle(Paint.Style.FILL);

        if(mViewType != ViewType.HorizontalButtonView)
        {
            if(mButtonPressed == ButtonName.Right && !isMoving)
                canvas.drawPath(mRightButtonPath, _mainPaint);
            if(mButtonPressed == ButtonName.Left && !isMoving)
                canvas.drawPath(mLeftButtonPath, _mainPaint);
        }

        if(mViewType != ViewType.VerticalButtonView)
        {

            if(mButtonPressed == ButtonName.Up && !isMoving)
                canvas.drawPath(mUpButtonPath, _mainPaint);
            if(mButtonPressed == ButtonName.Down && !isMoving)
                canvas.drawPath(mDownButtonPath, _mainPaint);
        }

        _mainPaint.setColor(_arrowFilledColor);
        canvas.drawPath(mArrowsPath, _mainPaint);
        canvas.drawPath(mArrowsPath, _arrowLinesPaint);

        if (mViewType == ViewType.HorizontalButtonView)
        {
            canvas.drawLine(0, MidScreenHeight, mWidth, MidScreenHeight, _linesPaint);
        }else  if (mViewType == ViewType.VerticalButtonView)
        {
            canvas.drawLine(MidScreenWidth, 0, MidScreenWidth, mHeight, _linesPaint);
        }else if (mViewType == ViewType.PadButtonView)
        {
            canvas.drawLine(0, 0, mWidth, mHeight, _linesPaint);
            canvas.drawLine(mWidth, 0, 0, mHeight, _linesPaint);
        }

        if(mCenterEnabled)
        {
            _mainPaint.setStyle(Paint.Style.FILL);
            _mainPaint.setColor(mButtonPressed == ButtonName.Center && !isMoving ? _centralButtonPressedColor:_centralButtonUnpressedColor);
            canvas.drawCircle(MidScreenWidth,MidScreenHeight,centerButtonRadius,_mainPaint);
            canvas.drawCircle(MidScreenWidth,MidScreenHeight,centerButtonRadius,_linesPaint);
            //canvas.drawRoundRect(MidScreenWidth - MidButtonWidth, MidScreenHeight - MidButtonHeight, MidScreenWidth + MidButtonWidth, MidScreenHeight + MidButtonHeight, 25, 25, _mainPaint);
            //canvas.drawRoundRect(MidScreenWidth - MidButtonWidth, MidScreenHeight - MidButtonHeight, MidScreenWidth + MidButtonWidth, MidScreenHeight + MidButtonHeight, 25, 25, _linesPaint);
        }


        if(isMoving){
            canvas.drawPath(touchPath, _touchPaint);
        }
    }

    private void initializePaths()
    {
        touchPath = new Path();
        mArrowsPath = new Path();
        if (mViewType == ViewType.HorizontalButtonView)
        {
            MidButtonWidth = mWidth *0.4f;
            MidButtonHeight = mHeight /6;

            mUpButtonPath =getPathFromPoints(new float[]{0, 0, mWidth, 0, mWidth, MidScreenHeight, 0, MidScreenHeight});
            mDownButtonPath =getPathFromPoints(new float[]{0, MidScreenHeight, mWidth, MidScreenHeight, mWidth, mHeight, 0, mHeight});

            mArrowsPath.moveTo(MidScreenWidth, mHeight / 10);
            mArrowsPath.lineTo(3 * mWidth / 10, mHeight / 5);
            mArrowsPath.lineTo(7 * mWidth / 10, mHeight / 5);
            mArrowsPath.lineTo(MidScreenWidth, mHeight / 10);

            mArrowsPath.moveTo(MidScreenWidth, 9 * mHeight / 10);
            mArrowsPath.lineTo(3 * mWidth / 10, 4 * mHeight / 5);
            mArrowsPath.lineTo(7 * mWidth / 10, 4 * mHeight / 5);
            mArrowsPath.lineTo(MidScreenWidth, 9 * mHeight / 10);
        }else if (mViewType == ViewType.VerticalButtonView)
        {
            MidButtonWidth = mWidth /6.f;
            MidButtonHeight = mHeight *0.4f;

            mRightButtonPath =getPathFromPoints(new float[]{mWidth, 0, mWidth, mHeight, MidScreenWidth, mHeight, MidScreenWidth, 0});
            mLeftButtonPath =getPathFromPoints(new float[]{0, 0, 0, mHeight, MidScreenWidth, mHeight, MidScreenWidth, 0});

            mArrowsPath.moveTo(mWidth /10, MidScreenHeight);
            mArrowsPath.lineTo(mWidth /5, 3* mHeight /10);
            mArrowsPath.lineTo(mWidth /5, 7* mHeight /10);
            mArrowsPath.lineTo(mWidth /10, MidScreenHeight);

            mArrowsPath.moveTo(9* mWidth /10, MidScreenHeight);
            mArrowsPath.lineTo(4* mWidth /5, 3* mHeight /10);
            mArrowsPath.lineTo(4* mWidth /5, 7* mHeight /10);
            mArrowsPath.lineTo(9* mWidth /10, MidScreenHeight);
        }else if (mViewType == ViewType.PadButtonView)
        {
            MidButtonWidth = mWidth /6.f;
            MidButtonHeight = mHeight /6.f;

            mUpButtonPath =getPathFromPoints(new float[]{0, 0, mWidth, 0, MidScreenWidth, MidScreenHeight});
            mDownButtonPath =getPathFromPoints(new float[]{mWidth, mHeight, 0, mHeight, MidScreenWidth, MidScreenHeight});
            mRightButtonPath =getPathFromPoints(new float[]{mWidth, 0, mWidth, mHeight, MidScreenWidth, MidScreenHeight});
            mLeftButtonPath =getPathFromPoints(new float[]{0, 0, 0, mHeight, MidScreenWidth, MidScreenHeight});


            //上箭頭
            mArrowsPath.moveTo(MidScreenWidth, mHeight /12);
            mArrowsPath.lineTo(5* mWidth /12, mHeight /7);
            mArrowsPath.lineTo(7* mWidth /12, mHeight /7);
            mArrowsPath.lineTo(MidScreenWidth, mHeight /12);

            //下箭頭
            mArrowsPath.moveTo(MidScreenWidth, 11* mHeight /12);
            mArrowsPath.lineTo(5* mWidth /12, 6* mHeight /7);
            mArrowsPath.lineTo(7* mWidth /12, 6* mHeight /7);
            mArrowsPath.lineTo(MidScreenWidth, 11* mHeight /12);

            //左箭頭
            mArrowsPath.moveTo(mWidth /12, MidScreenHeight);
            mArrowsPath.lineTo(mWidth /7, 5* mHeight /12);
            mArrowsPath.lineTo(mWidth /7, 7* mHeight /12);
            mArrowsPath.lineTo(mWidth /12, MidScreenHeight);

            //右箭頭
            mArrowsPath.moveTo(11* mWidth /12, MidScreenHeight);
            mArrowsPath.lineTo(6* mWidth /7, 5* mHeight /12);
            mArrowsPath.lineTo(6* mWidth /7, 7* mHeight /12);
            mArrowsPath.lineTo(11* mWidth /12, MidScreenHeight);
        }
    }

    private Path getPathFromPoints(float[] points)
    {
        Path newPath = new Path();
        newPath.reset();

        int tam=points.length/2;
        newPath.moveTo(points[0], points[1]);

        for(int i=1; i<tam; i++)
            newPath.lineTo(points[i*2], points[i*2+1]);

        newPath.close();
        return newPath;
    }

    float lastPosX = -1;
    float lastPosY = -1;
    @Override
    public boolean onTouchEvent(MotionEvent event)
    {
        float x = event.getX();
        float y = event.getY();
        mButtonPressed = CalculatePressedButton(x, y);
//        if (event.getAction() == MotionEvent.ACTION_DOWN )
//        {
//
//            return true;
//        }

//        if(event.getAction() == MotionEvent.ACTION_MOVE){
//
//        }
//        if (event.getAction() == MotionEvent.ACTION_UP)
//        {
//            //setPressedButton(ButtonName.None);
//            // Send Event
//
//
//        }

        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN:
                mButtonListener.onButtonDown(mButtonPressed,x,y);
                lastPosX = x;
                lastPosY = y;
                touchPath.moveTo(x,y);
                break;
            case MotionEvent.ACTION_MOVE:
                float deltaX =  Math.abs(lastPosX - x);
                float deltaY = Math.abs( lastPosY - y);

                if(isMoving == false)mButtonListener.onButtonMoveStart(x,y);
                isMoving = deltaX > movingGate || deltaY > movingGate;
                touchPath.lineTo(x, y);
                mButtonListener.onButtonMove(x,y);
                break;
            case MotionEvent.ACTION_UP:
                if(isMoving == false)mButtonListener.onButtonUp(mButtonPressed,x,y);
                else mButtonListener.onButtonMoveEnd(x,y);
                //Reset
                mButtonPressed = ButtonName.None;
                lastPosX = -1;
                lastPosY = -1;

                //Draw Line
                touchPath.lineTo(x, y);
                touchPath = new Path();
                isMoving = false;
                break;
            default:
                return false;
        }

        invalidate();

        return true;
        // return detectorGestos.onTouchEvent(event);
    }

//    private void setPressedButton(ButtonName _button)
//    {
//        if(_button!= mButtonPressed)
//        {
//            if(mButtonPressed != ButtonName.None)
//                mButtonListener.onButtonUp(mButtonPressed);
//
//            mButtonPressed = _button;
//
//            if(mButtonPressed != ButtonName.None)
//                mButtonListener.onButtonDown(mButtonPressed);
//
//            this.invalidate();
//        }
//    }

    private ButtonName CalculatePressedButton(float newX, float newY)
    {
        if(collidesCenterButton(newX,newY))
            return ButtonName.Center;

        float NormalizedX=(newX- MidScreenWidth)/(MidScreenWidth);
        float NormalizedY=-(newY- MidScreenHeight)/(MidScreenHeight);

        if(NormalizedX>1) NormalizedX=1; else if(NormalizedX<-1) NormalizedX=-1;
        if(NormalizedY>1) NormalizedY=1; else if(NormalizedY<-1) NormalizedY=-1;

        if(mViewType == ViewType.VerticalButtonView)
        {
            if(NormalizedX>0)   return ButtonName.Right;
            else                return ButtonName.Left;
        }else if(mViewType == ViewType.HorizontalButtonView)
        {
            if(NormalizedY>0)   return ButtonName.Up;
            else                return ButtonName.Down;
        }else if(mViewType == ViewType.PadButtonView)
        {
            if(Math.abs(NormalizedX)>Math.abs(NormalizedY))
            {
                if(NormalizedX>0)   return ButtonName.Right;
                else                return ButtonName.Left;
            }else
            {
                if(NormalizedY>0)   return ButtonName.Up;
                else                return ButtonName.Down;
            }
        }

        return ButtonName.None;
    }

    protected boolean collidesCenterButton(float ex, float ey)
    {
        if(!mCenterEnabled) return false;

//        if( ex >= MidScreenWidth - MidButtonWidth &&
//            ex <= MidScreenWidth + MidButtonWidth &&
//            ey >= MidScreenHeight - MidButtonHeight &&
//            ey <= MidScreenHeight + MidButtonHeight)
//        {
//            return true;
//        }

        if( getDistance(ex,ey,MidScreenWidth,MidScreenHeight) < centerButtonRadius){
            return  true;
        }

        return false;
    }


    double getDistance(double pointA_x,double pointA_y,double pointB_x,double pointB_y){
        double vectorX = pointA_x - pointB_x;
        double vectorY= pointA_y - pointB_y;
        return Math.sqrt((vectorX *vectorX) + (vectorY*vectorY));
    }
}