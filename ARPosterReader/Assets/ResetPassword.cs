using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Collections;

public class ResetPassword : MonoBehaviour
{
    public InputField resetPasswordEmail;
    public GameObject resetPasswordPanel;
    public InputField newPassword;
    public InputField confirmPassword;
    bool inResetPasswordState;
    public Image toast;
    public Text toastText;

    public void resetPasswordByEmail()
    {
        User u = new User();
        u.email = resetPasswordEmail.text;
        u.ResetPassword();
    }

    public void resetPasswordById()
    {
        if (storeLoginSessionId.loginId==-1 || string.IsNullOrEmpty(newPassword.text))
            return;
        if (!newPassword.text.Equals(confirmPassword.text))
            return;
        User u = new User();
        u.id = storeLoginSessionId.loginId;
        u.password = newPassword.text;
        u.ResetPassword();
        showToast("Reset password success", 2);
    }

    public void toggleResetPasswordPlane()
    {
        inResetPasswordState = !inResetPasswordState;
        resetPasswordPanel.SetActive(inResetPasswordState);
    }

    void showToast(string text, int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = toastText.color;

        toastText.text = text;
        toastText.enabled = true;

        //Fade in
        yield return fadeInAndOut(true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(false, 0.5f);

        toastText.enabled = false;
        toastText.color = orginalColor;
    }

    IEnumerator fadeInAndOut(bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = new Color(0.7f, 0.7f, 0.7f);
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            toast.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }
}
