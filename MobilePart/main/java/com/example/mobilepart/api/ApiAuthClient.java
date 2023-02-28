package com.example.mobilepart.api;

import android.content.Context;
import android.content.SharedPreferences;
import android.util.Log;

import com.example.mobilepart.R;
import com.example.mobilepart.api.models.AuthenticationModel;
import com.example.mobilepart.api.requests.Request;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class ApiAuthClient {
    private static final String TAG = "ApiAuthClient";

    private static String token = "";
    private static boolean isAuthenticated = false;
    private static int errorMessageStringCode;

    public static String GetToken() {
        return "Bearer " + token;
    }
    public static int GetErrorMessageStringCode() {
        return errorMessageStringCode;
    }

    public static boolean IsAuthenticated() {
        Log.d(TAG, token);
        return isAuthenticated && !token.equals("");
    }

    public static void Authorize(String email, String password, Context context)
            throws IOException {

        HashMap<String, String> authData = new HashMap<String, String>();
        authData.put("email", email);
        authData.put("password", password);
        //Request PostUser = new Request("users", "token", authData, Request.MethodTypes.POST);
        Request PostUser = new Request.Builder()
                .SetUrn("users")
                .SetResourcePath("token")
                .SetParameters(authData)
                .SetMethodType(Request.MethodTypes.POST)
                .Build();
        String responseJSON = PostUser.Execute();

        AuthenticationModel authModel = new AuthenticationModel();
        if(responseJSON == "error") {
            authModel.isAuthenticated = false;
        } else {
            GsonBuilder builder = new GsonBuilder();
            builder.setDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            builder.serializeNulls();
            Gson gson = builder.create();
            authModel = gson.fromJson(responseJSON, AuthenticationModel.class);
        }

        if(authModel.isAuthenticated) {
            final String managerRole = "Manager";
            if(authModel.roles.contains(managerRole)) {
                token = authModel.token;
                isAuthenticated = true;
                SharedPreferences settings = context.getSharedPreferences("auth", Context.MODE_PRIVATE);
                SharedPreferences.Editor editor = settings.edit();
                editor.putString("refreshToken", authModel.refreshToken);
                editor.apply();
            } else {
                errorMessageStringCode = R.string.invalid_role;
            }

        } else {
            errorMessageStringCode = R.string.invalid_credentials;
        }
    }

    public static void RefreshToken(String refreshToken, Context context) {

        HashMap<String, List<String>> headers = new HashMap<>();
        ArrayList<String> refreshTokenHeader = new ArrayList<>();
        refreshTokenHeader.add(refreshToken);
        headers.put("refreshToken", refreshTokenHeader);

        Request refreshTokenRequest = new Request.Builder()
                .SetUrn("users")
                .SetResourcePath("refresh-token")
                .SetMethodType(Request.MethodTypes.POST)
                .SetHeaders(headers)
                .Build();
        String response = refreshTokenRequest.Execute();

        AuthenticationModel authModel = new AuthenticationModel();
        if(response == "error") {
            authModel.isAuthenticated = false;
        } else {
            GsonBuilder builder = new GsonBuilder();
            builder.setDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            builder.serializeNulls();
            Gson gson = builder.create();
            authModel = gson.fromJson(response, AuthenticationModel.class);
        }

        if(authModel.isAuthenticated) {
            isAuthenticated = true;
            token = authModel.token;
            SharedPreferences settings = context.getSharedPreferences("auth", Context.MODE_PRIVATE);
            SharedPreferences.Editor editor = settings.edit();
            editor.putString("refreshToken", authModel.refreshToken);
            editor.apply();
        }
    }

    public static void Logout(Context context) {
        isAuthenticated = false;
        token = "";
        SharedPreferences settings = context.getSharedPreferences("auth", Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = settings.edit();
        if(settings.contains("refreshToken")) {
            editor.remove("refreshToken");
            editor.apply();
        }
    }
}
