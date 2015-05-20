package com.kireino.vicomm_android.app;

import android.location.Address;
import android.os.AsyncTask;
import android.util.Log;

import java.io.*;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;
import java.net.SocketException;
import java.nio.charset.StandardCharsets;

/**
 * Created by Kireino on 2015-05-01.
 */
public class Client {
    private Socket socket;
    private SocketAddress address;
    private String guid;

    public User user;

    private static Client _instance = null;

    public static Client GetInstance() {
        if (_instance == null) {
            _instance = new Client();
        }

        return _instance;
    }

    public boolean isConnected() {
        if (socket != null) {
            if (socket.isConnected()) {
                return true;
            }
        }

        return false;
    }

    public void Connect(String host, int port) {
        address = new InetSocketAddress(host, port);
        socket = new Socket();

        Connect();
    }

    private void Connect() {
        if (isConnected()) {
            return;
        }

        try {
            socket.connect(address);
            socketOut = new PrintWriter(socket.getOutputStream(), true);
            inputStream = new DataInputStream(socket.getInputStream());
            StartReceive();
        } catch (SocketException e) {
            Log.d("TAG", "DEBUG", e);
        } catch (IOException e) {
            Log.d("TAG", "DEBUG", e);
        } catch (Exception e) {
            Log.d("TAG", "DEBUG", e);
        }
    }

    private PrintWriter socketOut;

    public void Send(final Packet packet) {
        if (isConnected()) {
            socketOut.println(packet.toString());
            socketOut.flush();
        }
    }

    private InputStream socketIn;
    private DataInputStream inputStream;
    private boolean _flagReceive = false;

    private void StartReceive() {
        _flagReceive = true;

        new Thread(new Runnable() {
            @Override
            public void run() {
                while (_flagReceive == true) {
                    try {
                        byte[] message = new byte[8192];
                        int length = inputStream.read(message);
                        if(length>0) {
                            String data = new String(message, 0, length);

                            Log.d("TAG", data);
                            Log.d("TAG", "LOL");

                            if (data != null)
                                Received(new Packet(data));
                        }
                    } catch (Exception e) {
                        Log.d("TAG", "DEBUG", e);
                    }
                }
            }
        }).start();
    }

    private void Received(Packet packet) {
        switch (packet.Type) {
            case Login:
                LoginResult(packet);
                break;
            case Register:
                break;
            case Disconnect:
                break;

            case Information:
                break;

            case SingleChat:
                break;
            case MultiChat:
                break;
        }
    }

    int mID;

    private void LoginResult(Packet packet) {
        MainActivity.activity.ShowSystemNotification(mID, R.mipmap.ic_launcher, packet.User.Username, packet.Message);
    }
}
