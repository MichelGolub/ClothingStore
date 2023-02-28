package com.example.mobilepart.api.requests;

import android.util.Log;

import com.example.mobilepart.R;
import com.example.mobilepart.api.BodyStringBuilder;
import com.example.mobilepart.api.HeadersMapBuilder;
import com.example.mobilepart.api.ParameterStringBuilder;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.URL;
import java.nio.charset.StandardCharsets;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.net.ssl.HttpsURLConnection;

public class Request {
    private static final String TAG = "Request";

    private static final String URL_BASE = "https://192.168.0.104:45455/api/";
    private String urnResource = "";
    private String resourcePath = "";
    private MethodTypes methodType = MethodTypes.GET;
    private String payload = "";
    private HashMap<String, String> parameters = new HashMap<String, String>();
    private Map<String, List<String>> headers = new HashMap<String, List<String>>();
    public static enum MethodTypes {
        GET,
        POST,
        PUT,
        DELETE
    }

    public Request() {
        //System.setProperty("jsse.enableSNIExtension", "false");
    }

    private void setUrnResource(String urnResource) {
        this.urnResource = urnResource;
    }

    private void setResourcePath (String resourcePath) {
        this.resourcePath = resourcePath;
    }

    private void setMethodType(MethodTypes methodType) {
        this.methodType = methodType;
    }

    private void setParameters(HashMap<String, String> parameters) {
        this.parameters = parameters;
    }

    private void setHeaders(HashMap<String, List<String>> headers) {
        this.headers = headers;
    }

    public static String getUrlBase() {
        return URL_BASE;
    }

    public String Execute() {
        StringBuilder outputStringBuilder = new StringBuilder();

        try {
            StringBuilder uriString = new StringBuilder(URL_BASE + urnResource);

            if(!resourcePath.equals("")) {
                uriString.append("/" + resourcePath);
            }

            if(parameters.size() > 0 && methodType.equals(MethodTypes.GET)) {
                payload = ParameterStringBuilder.getParamsString(parameters);
                uriString.append("?" + payload);
            }

            URL url = new URL(uriString.toString());
            HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();

            connection.setRequestMethod(String.valueOf(methodType));
            //connection.setRequestProperty("User-Agent", "androidApp");
            connection.setRequestProperty("Content-Type", "application/json; utf-8");
            connection.setRequestProperty("Accept", "application/json");

            HashMap<String, String> headersMap = HeadersMapBuilder.getHeadersMap(headers);
            for(Map.Entry<String, String> header : headersMap.entrySet()) {
                connection.setRequestProperty(header.getKey(), header.getValue());
            }

            connection.setConnectTimeout(5000);
            connection.setReadTimeout(5000);

            if(methodType.equals(MethodTypes.POST) || methodType.equals(MethodTypes.PUT)) {
                payload = BodyStringBuilder.getBodyString(parameters);
                connection.setDoInput(true);
                connection.setDoOutput(true);

                try {
                    OutputStreamWriter writer = new OutputStreamWriter
                            (connection.getOutputStream());
                    writer.write(payload);
                    writer.flush();


                } catch (IOException ioException) {
                    Log.e(TAG, ioException.getMessage());
                    connection.disconnect();
                    return "error";
                }
            }
            connection.getResponseCode();
            InputStream stream = connection.getErrorStream();
            if (stream != null) {
                try(BufferedReader br = new BufferedReader(
                        new InputStreamReader(stream, StandardCharsets.UTF_8))) {
                    StringBuilder response = new StringBuilder();
                    String responseLine;
                    while ((responseLine = br.readLine()) != null) {
                        response.append(responseLine.trim());
                    }
                    Log.e(TAG, response.toString());
                }
            }

            outputStringBuilder.append(readResponse(connection));

        } catch (IOException ioException) {
            Log.e(TAG, ioException.getMessage());
            return "error";
        }

        return outputStringBuilder.toString();
    }

    protected String readResponse(HttpsURLConnection connection) throws IOException {
        try(BufferedReader br = new BufferedReader(
                new InputStreamReader(connection.getInputStream(), StandardCharsets.UTF_8))) {
            StringBuilder response = new StringBuilder();
            String responseLine;
            while ((responseLine = br.readLine()) != null) {
                response.append(responseLine.trim());
            }
            return response.toString();
        }
    }

    public static class Builder {
        private Request newRequest;

        public Builder() {
            newRequest = new Request();
        }

        public Builder SetUrn(String urn) {
            newRequest.setUrnResource(urn);
            return this;
        }

        public Builder SetResourcePath (String resourcePath) {
            newRequest.setResourcePath(resourcePath);
            return this;
        }

        public Builder SetMethodType(MethodTypes methodType) {
            newRequest.setMethodType(methodType);
            return this;
        }

        public Builder SetParameters(HashMap<String, String> parameters) {
            newRequest.setParameters(parameters);
            return this;
        }

        public Builder SetHeaders(HashMap<String, List<String>> headers) {
            newRequest.setHeaders(headers);
            return this;
        }

        public Request Build() {
            return newRequest;
        }
    }
}
