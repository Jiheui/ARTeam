using UnityEngine;
using UnityEngine.UI;
using Models;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class ResetPassword : MonoBehaviour
{
    public InputField resetPasswordEmail;
    public GameObject resetPasswordPanel;
    public InputField newPassword;
    bool inResetPasswordState;

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

        User u = new User();
        u.id = storeLoginSessionId.loginId;
        u.password = newPassword.text;
        u.ResetPassword();
    }

    public void toggleResetPasswordPlane()
    {
        inResetPasswordState = !inResetPasswordState;
        resetPasswordPanel.SetActive(inResetPasswordState);
    }
}
