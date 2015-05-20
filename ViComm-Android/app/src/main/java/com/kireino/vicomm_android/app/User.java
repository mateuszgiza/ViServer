package com.kireino.vicomm_android.app;

/**
 * Created by Kireino on 2015-05-01.
 */
public class User {
    public int ID;
    public String Username;
    public String Nickname;
    public String Email;
    public int Type;
    public byte[] Password;
    public byte[] Salt;
    public String AvatarURI;
    public String NickColor;

    public User() {
    }

    public User(User user) {
        Username = user.Username;
        Email = user.Email;
        Password = user.Password;
        Salt = user.Salt;
    }

    public User(String Login, byte[] password) {
        Username = Login;
        Password = password;
    }

    public User(String username, String email, byte[] password, byte[] salt) {
        Username = username;
        Email = email;
        Password = password;
        Salt = salt;
    }
}
