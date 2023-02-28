package com.example.mobilepart.api;

import java.net.URLEncoder;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class HeadersMapBuilder {
    public static HashMap<String, String> getHeadersMap
            (Map<String, List<String>> headerWithParamsList) {
        HashMap<String, String> headersMap = new HashMap<String, String>();

        StringBuilder bld = new StringBuilder();

        for (Map.Entry<String, List<String>> entry : headerWithParamsList.entrySet()) {
            for(String param : entry.getValue()) {
                bld.append(param);
                bld.append(", ");
            }
            if(bld.length() > 2) {
                int start = bld.length() - 2;
                int end = bld.length() - 1;
                bld.delete(start, end);
            }
            headersMap.put(entry.getKey(), bld.toString());
            bld.delete(0, bld.length() - 1);
        }

        return headersMap;
    }
}
