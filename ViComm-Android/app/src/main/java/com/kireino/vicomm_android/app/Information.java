package com.kireino.vicomm_android.app;

import java.util.List;

/**
 * Created by Kireino on 2015-05-01.
 */
enum InformationType {
    Server,

    Writing,

    Joining,
    Leaving,
    Contacts
}

public class Information {
    public InformationType Type;
    public String User;
    public String Message;
    public List<String> Contacts;

    public Information(Information information) {
        Type = information.Type;
        User = information.User;
        Message = information.Message;
        Contacts = information.Contacts;
    }

    public Information(InformationType type, List<String> contacts) {
        Type = type;
        Contacts = contacts;
    }

    public Information(InformationType type, String user, String msg) {
        Type = type;
        User = user;
        Message = msg;
    }
}