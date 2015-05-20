package com.kireino.vicomm_android.app;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.util.Date;


/**
 * Created by Kireino on 2015-05-01.
 */

enum PacketType {
    Login,
    Register,
    Disconnect,

    Information,

    MultiChat,
    SingleChat
}

public class Packet {
    public PacketType Type;
    public User User;
    public Information Information;
    public Date Date;
    public String Sender;
    public String Receiver;
    public String Message;

    public Packet(PacketType type) {
        Type = type;
    }

    public Packet(String json) {
        Gson gson = new Gson();
        Packet packet = gson.fromJson(json, Packet.class);

        Type = packet.Type;
        User = packet.User;
        Information = packet.Information;
        Date = packet.Date;
        Sender = packet.Sender;
        Receiver = packet.Receiver;
        Message = packet.Message;
    }

    @Override
    public String toString() {
        GsonBuilder builder = new GsonBuilder();
        Gson gson = builder.create();

        return gson.toJson(this);
    }
}
