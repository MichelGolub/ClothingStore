package com.example.mobilepart.services;

import android.app.Service;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Binder;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.util.Log;

import com.example.mobilepart.api.ApiAuthClient;
import com.example.mobilepart.api.requests.Request;
import com.microsoft.signalr.HubConnection;
import com.microsoft.signalr.HubConnectionBuilder;
import com.microsoft.signalr.HubConnectionState;
import com.microsoft.signalr.OnClosedCallback;
import com.microsoft.signalr.Subscription;

import java.util.List;

import io.reactivex.Completable;
import io.reactivex.Single;

public class VisitorsService extends Service {
    private HubConnection hubConnection;
    private Handler handler;
    private final IBinder binder = new LocalBinder();
    private ServiceCallbacks serviceCallbacks;

    public VisitorsService() {
    }

    @Override
    public void onCreate() {
        super.onCreate();
        handler = new Handler(Looper.getMainLooper());
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        int result = super.onStartCommand(intent, flags, startId);
        startHub();
        return result;
    }

    @Override
    public void onDestroy() {
        hubConnection.stop();
        super.onDestroy();
    }

    @Override
    public IBinder onBind(Intent intent) {
        startHub();
        return binder;
    }

    public class LocalBinder extends Binder {
        public VisitorsService getService() {
            return VisitorsService.this;
        }
    }

    public void startHub() {
        StringBuilder hubUrlBuilder = new StringBuilder();
        hubUrlBuilder.append(Request.getUrlBase());
        int apiIdx = hubUrlBuilder.indexOf("api/");
        hubUrlBuilder.replace(apiIdx, hubUrlBuilder.length() - 1, "visitors");

        String hubEndpoint = hubUrlBuilder.toString();
        hubConnection = HubConnectionBuilder.create(hubEndpoint)
                .withHeader("Authorization", ApiAuthClient.GetToken())
                .build();
        hubConnection.start();
        hubConnection.on("Recieve", (productsId) -> {
            if(serviceCallbacks != null) {
                serviceCallbacks.pushVisitor(productsId);
            }
        }, int[].class);

        closedConnectionHandler handler = new closedConnectionHandler();
        hubConnection.onClosed(handler);
    }

    public void restartHub() {
        hubConnection.stop();
        startHub();
    }

    public void Send(int[] productsId) {
        if(hubConnection.getConnectionState().equals(HubConnectionState.CONNECTED)) {
            hubConnection.invoke("Send", productsId);
        }
    }

    public interface ServiceCallbacks {
        void pushVisitor(int[] productsId);
    }

    private class closedConnectionHandler implements OnClosedCallback {

        @Override
        public void invoke(Exception exception) {
            Log.d("Hub", "COn closed!");
        }
    }

    public void setCallbacks(ServiceCallbacks callbacks) {
        serviceCallbacks = callbacks;
    }
}