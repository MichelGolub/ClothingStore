package com.example.mobilepart.api;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.Map;

public class ParameterStringBuilder {
    public static String getParamsString(Map<String, String> params)
            throws UnsupportedEncodingException {

        StringBuilder bld = new StringBuilder();

        for (Map.Entry<String, String> entry : params.entrySet()) {
            bld.append(URLEncoder.encode(entry.getKey(), "UTF-8"));
            bld.append("=");
            bld.append(URLEncoder.encode(entry.getValue(), "UTF-8"));
            bld.append("&");
        }
        if(bld.length() > 0) {
            bld.deleteCharAt(bld.length() - 1);
        }
        String resultString = bld.toString();
        return resultString;
    }
}
