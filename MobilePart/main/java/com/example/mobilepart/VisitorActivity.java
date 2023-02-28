package com.example.mobilepart;

import androidx.appcompat.app.AppCompatActivity;

import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.widget.ArrayAdapter;
import android.widget.ListView;

import com.example.mobilepart.api.ApiAuthClient;
import com.example.mobilepart.api.models.AuthenticationModel;
import com.example.mobilepart.api.requests.Request;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class VisitorActivity extends AppCompatActivity {
    private List<String> productsNames = new ArrayList<>();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_visitor);

        Bundle args = getIntent().getExtras();
        int[] productsId = args.getIntArray("productsId");
        GetProductsRequestTask request = new GetProductsRequestTask(productsId);
        request.execute();
    }

    private class GetProductsRequestTask extends AsyncTask<Void, String, Void> {
        private int[] productsId;

        private GetProductsRequestTask(int[] productsId) {
            this.productsId = productsId;
        }

        @Override
        protected Void doInBackground(Void... voids) {
            if(ApiAuthClient.IsAuthenticated()) {
                HashMap<String, List<String>> headers = new HashMap<>();
                ArrayList<String> tokenHeader = new ArrayList<>();
                tokenHeader.add(ApiAuthClient.GetToken());
                headers.put("Authorization", tokenHeader);

                GsonBuilder builder = new GsonBuilder();
                builder.serializeNulls();
                Gson gson = builder.create();



                for(int i = 0; i < productsId.length; i++) {
                    Request request = new Request.Builder()
                            .SetHeaders(headers)
                            .SetUrn("products")
                            .SetResourcePath(String.valueOf(productsId[i]))
                            .SetMethodType(Request.MethodTypes.GET)
                            .Build();
                    String response = request.Execute();
                    ProductModel product = gson.fromJson(response, ProductModel.class);
                    publishProgress(product.name);
                }
            }
            return null;
        }

        @Override
        protected void onProgressUpdate(String... values) {
            super.onProgressUpdate(values);
            productsNames.add(values[0]);
        }

        @Override
        protected void onPostExecute(Void unused) {
            super.onPostExecute(unused);

            ListView productsList = (ListView) findViewById(R.id.visitorsList);
            ArrayAdapter<String> adapter = new ArrayAdapter(getApplicationContext(),
                    android.R.layout.simple_list_item_1, productsNames);
            productsList.setAdapter(adapter);
        }
    }

    private class ProductModel {
        private int id;
        private String name;
        private int shopId;

        private ProductModel() {}
    }
}