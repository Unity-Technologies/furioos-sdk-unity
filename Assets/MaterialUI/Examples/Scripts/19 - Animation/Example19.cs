//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections;
using UnityEngine;
using MaterialUI;
using UnityEngine.UI;

public class Example19 : MonoBehaviour
{
    [SerializeField]
    private Image m_ColorImage1;
    [SerializeField]
    private Image m_ColorImage2;
    [SerializeField]
    private Image m_ColorImage3;
    [SerializeField]
    private Image m_ColorImage4;

    [SerializeField]
    private Image[] m_ColorImagesArray1;
    [SerializeField]
    private Image[] m_ColorImagesArray2;
    [SerializeField]
    private Image[] m_ColorImagesArray3;

    [SerializeField]
    private AnimationCurve m_AnimCurve;

    private int m_SavedId = -1;

    public void Simple1(bool isRed)
    {
        Color targetColor = isRed ? MaterialColor.red500 : MaterialColor.blue500;
        TweenManager.TweenColor(color => m_ColorImage1.color = color, m_ColorImage1.color, targetColor, 1f);
    }

    public void Simple2(bool isRed)
    {
        Color targetColor = isRed ? MaterialColor.red500 : MaterialColor.blue500;
        float delay = isRed ? 0f : 0.5f;
        Action callback = null;
        if (isRed)
        {
            callback = () => ToastManager.Show("Red tween finished!");
        }
        TweenManager.TweenColor(color => m_ColorImage2.color = color, m_ColorImage2.color, targetColor, 1f, delay, callback);
    }

    public void Advanced1(bool isRed)
    {
        if (isRed)
        {
            TweenManager.TweenColorCustom(color => m_ColorImage3.color = color, m_ColorImage3.color, MaterialColor.red500, 1f, m_AnimCurve);
        }
        else
        {
            TweenManager.TweenColor(color => m_ColorImage3.color = color, m_ColorImage3.color, MaterialColor.blue500, 1f, 0f, null, false, Tween.TweenType.Linear);
        }
    }

    public void Advanced2(int buttonType)
    {
        Color targetColor;

        TweenManager.EndTween(m_SavedId);

        switch (buttonType)
        {
            case 0:
                return;
            case 1:
                targetColor = MaterialColor.red500;
                break;
            default:
                targetColor = MaterialColor.blue500;
                break;
        }

        m_SavedId = TweenManager.TweenColor(color => m_ColorImage4.color = color, m_ColorImage4.color, targetColor, 1f, tweenType: Tween.TweenType.Linear);
    }

    #region CoroutineIssue
    //  An issue occurs in the tweening system if you try call tweens within a loop, inside a coroutine
    public void Issue(bool isRed)
    {
        StartCoroutine(IssueCoroutine(isRed));
    }

    IEnumerator IssueCoroutine(bool isRed)
    {
        Color targetColor = isRed ? MaterialColor.red500 : MaterialColor.blue500;
        for (int i = 0; i < m_ColorImagesArray1.Length; i++)
        {
            int i1 = i;
            TweenManager.TweenColor(color => m_ColorImagesArray1[i1].color = color, m_ColorImagesArray1[i1].color, targetColor, 1.5f, tweenType: Tween.TweenType.Linear);
            yield return new WaitForSeconds(0.5f);
        }
    }

    #endregion

    #region CoroutineIssueWorkaround1
    //  One workaround is to try and avoid the coroutine (here, we put the delay inside the tween calls)
    public void Workaround1(bool isRed)
    {
        Color targetColor = isRed ? MaterialColor.red500 : MaterialColor.blue500;
        for (int i = 0; i < m_ColorImagesArray2.Length; i++)
        {
            int i1 = i;
            TweenManager.TweenColor(color => m_ColorImagesArray2[i1].color = color, m_ColorImagesArray2[i1].color, targetColor, 1.5f, i1 * 0.5f, tweenType: Tween.TweenType.Linear);
        }
    }

    #endregion

    #region CoroutineIssueWorkaround2
    //  Another workaround is to make another method and send it the reference of the value being tweened.
    public void Workaround2(bool isRed)
    {
        StartCoroutine(Workaround2Coroutine(isRed));
    }

    IEnumerator Workaround2Coroutine(bool isRed)
    {
        for (int i = 0; i < m_ColorImagesArray3.Length; i++)
        {
            Workaround2Workaround(m_ColorImagesArray3[i], isRed);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Workaround2Workaround(Image image, bool isRed)
    {
        Color targetColor = isRed ? MaterialColor.red500 : MaterialColor.blue500;
        TweenManager.TweenColor(color => image.color = color, image.color, targetColor, 1.5f, tweenType: Tween.TweenType.Linear);
    }

    #endregion
}