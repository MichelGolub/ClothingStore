package com.example.mobilepart;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.RecyclerView;

import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.content.SharedPreferences;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.IBinder;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;


import com.example.mobilepart.api.ApiAuthClient;
import com.example.mobilepart.services.VisitorsService;


import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;


public class MainActivity extends AppCompatActivity implements VisitorsService.ServiceCallbacks {
    private final static String TAG = "MainActivity";
    private boolean serviceBounded = false;
    private VisitorsService visitorsService;

    private ArrayList<visitorInfo> visitors = new ArrayList<>();
    private visitorAdapter viewAdapter;
    private RecyclerView recyclerView;

    private SharedPreferences sharedPreferences;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        sharedPreferences = getSharedPreferences("auth", MODE_PRIVATE);

        initializeRecycleView();
    }

    private void initializeRecycleView() {
        recyclerView = findViewById(R.id.recyclerViewVisitors);

        visitorAdapter.OnVisitorClickListener visitorClickListener = new visitorAdapter
                .OnVisitorClickListener() {
            @Override
            public void onVisitorClick(visitorInfo visitor, int position) {
                goToVisitorActivity(visitor.productsId);
            }
        };

        viewAdapter = new visitorAdapter(this, visitors, visitorClickListener);
        recyclerView.setAdapter(viewAdapter);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.main_menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int id = item.getItemId();
        switch (id) {
            case R.id.action_logout :
                goToLoginActivity(this);
            return  true;
        }

        return super.onOptionsItemSelected(item);
    }

    private void goToLoginActivity(Context mContext) {
        Intent intent = new Intent(mContext, LoginActivity.class);
        intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);
        startActivity(intent);
        ApiAuthClient.Logout(mContext);
        finish();
    }

    private void goToVisitorActivity(int[] productsId) {
        Intent intent = new Intent(getApplicationContext(), VisitorActivity.class);
        intent.putExtra("productsId", productsId);
        startActivity(intent);
    }

    //TODO для тестирования, не входит в функционал
    public void sendMessage(View view) {
        EditText etMessage = findViewById(R.id.editTextTextPersonName);
        String message = etMessage.getText().toString();
        message = message.replaceAll("[^0-9]+", " ");
        if(message.equals("")) {
            return;
        }
        String[] messages = message.split(" ");

        int[] productsId = new int[messages.length];
        for(int i = 0; i < messages.length; i++) {
            productsId[i] = Integer.valueOf(messages[i]);
        }

        if(serviceBounded) {
            visitorsService.Send(productsId);
        }
    }

    @Override
    protected void onResume() {
        if(!ApiAuthClient.IsAuthenticated()) {
            Log.d(TAG, "не авторизован");
            if(!sharedPreferences.contains("refreshToken")) {
                goToLoginActivity(this);
            } else {
                String refreshToken = sharedPreferences.getString("refreshToken", "");
                refreshTokenTask refreshTokenTask = new refreshTokenTask(this);
                refreshTokenTask.execute(refreshToken);
                Log.d(TAG, "обновляется кокен");
            }
        } else {
            if(!serviceBounded) {
                Intent intent = new Intent();
                intent.setClass(getApplicationContext(), VisitorsService.class);
                bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE);
            }
        }

        super.onResume();
    }

    @Override
    protected void onStop(){
        if (serviceBounded) {
            visitorsService.setCallbacks(null);
            unbindService(serviceConnection);
            serviceBounded = false;
        }
        super.onStop();
    }

    private final ServiceConnection serviceConnection = new ServiceConnection() {
        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {
            VisitorsService.LocalBinder binder = (VisitorsService.LocalBinder) service;
            visitorsService = binder.getService();
            serviceBounded = true;
            visitorsService.setCallbacks(MainActivity.this);
        }

        @Override
        public void onServiceDisconnected(ComponentName name) {
            serviceBounded = false;
        }
    };

    @Override
    public void pushVisitor(int[] productsId) {
        visitors.add(new visitorInfo(new Date(), productsId));

        updateViewTask task = new updateViewTask();
        task.execute();
    }

    public void clearVisitorsList(View view) {
        visitors.clear();
        viewAdapter.notifyDataSetChanged();
    }

    private class visitorInfo {
        private Date visitTime;
        private int[] productsId;

        private visitorInfo(Date time, int[] productsId) {
            visitTime = time;
            this.productsId = productsId;
        }
    }

    public static class visitorAdapter extends RecyclerView.Adapter<visitorAdapter.ViewHolder> {

        interface OnVisitorClickListener {
            void onVisitorClick(visitorInfo visitor, int position);
        }

        private final OnVisitorClickListener onClickListener;

        private LayoutInflater inflater;
        private List<visitorInfo> adapterVisitors;

        visitorAdapter(Context context, List<visitorInfo> visitors,
                       OnVisitorClickListener onClickListener) {
            this.adapterVisitors = visitors;
            this.inflater = LayoutInflater.from(context);
            this.onClickListener = onClickListener;
        }

        @Override
        public ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {

            View view = inflater.inflate(R.layout.visitor_item, parent, false);
            return new ViewHolder(view);
        }

        @Override
        public void onBindViewHolder(ViewHolder holder, int position) {
            visitorInfo visitor = adapterVisitors.get(position);
            DateFormat dateFormat = new SimpleDateFormat("H:mm:ss");
            String strDate = dateFormat.format(visitor.visitTime);
            holder.dateView.setText(strDate);

            holder.itemView.setOnClickListener(new View.OnClickListener(){
                @Override
                public void onClick(View v) {
                    onClickListener.onVisitorClick(visitor, position);
                }
            });
        }

        @Override
        public int getItemCount() {
            return adapterVisitors.size();
        }

        public class ViewHolder extends RecyclerView.ViewHolder {
            final TextView dateView;
            ViewHolder(View view){
                super(view);
                dateView = (TextView) view.findViewById(R.id.tvVisitDate);
            }
        }

    }

    private class updateViewTask extends AsyncTask<Void, Void, Void> {

        @Override
        protected Void doInBackground(Void... voids) {
            publishProgress();
            return null;
        }

        @Override
        protected void onProgressUpdate(Void... values) {
            super.onProgressUpdate(values);
            viewAdapter.notifyDataSetChanged();
        }
    }

    private class refreshTokenTask extends AsyncTask<String, Void, Void> {
        private Context mContext;

        public refreshTokenTask(Context context) {
            mContext = context;
        }

        @Override
        protected Void doInBackground(String... params) {
            ApiAuthClient.RefreshToken(params[0], mContext);
            return null;
        }

        @Override
        protected void onPostExecute(Void unused) {
            super.onPostExecute(unused);
            if(!ApiAuthClient.IsAuthenticated()) {
                goToLoginActivity(mContext);
            } else {
                if(!serviceBounded) {
                    Intent intent = new Intent();
                    intent.setClass(mContext, VisitorsService.class);
                    bindService(intent, serviceConnection, Context.BIND_AUTO_CREATE);
                } else {
                    visitorsService.restartHub();
                }
            }
        }
    }
}