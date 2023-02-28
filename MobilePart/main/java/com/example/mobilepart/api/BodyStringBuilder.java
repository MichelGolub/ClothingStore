package com.example.mobilepart.api;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.Map;

public class BodyStringBuilder {
    public static String getBodyString(Map<String, String> params)
            throws UnsupportedEncodingException {

        StringBuilder bld = new StringBuilder();

        bld.append("{");
        for (Map.Entry<String, String> entry : params.entrySet()) {
            bld.append(" \"");
            bld.append(URLEncoder.encode(entry.getKey(), "UTF-8"));
            bld.append("\": \"");
            //bld.append(URLEncoder.encode(entry.getValue(), "UTF-8"));
            bld.append(entry.getValue());
            bld.append("\",");
        }
        if(bld.length() > 1) {
            bld.deleteCharAt(bld.length() - 1);
        }
        bld.append(" }");

        String resultString = bld.toString();
        return resultString;
    }
}
