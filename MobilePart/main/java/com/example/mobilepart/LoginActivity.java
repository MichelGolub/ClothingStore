package com.example.mobilepart;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;


import com.example.mobilepart.api.ApiAuthClient;

import java.io.IOException;

public class LoginActivity extends AppCompatActivity {

    private static final String TAG = "LoginActivity";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
    }

    public void Login(View view) {
        EditText editTextEmail = findViewById(R.id.editTextEmail);
        EditText editTextPassword = findViewById(R.id.editTextPassword);
        String email = editTextEmail.getText().toString();
        String password = editTextPassword.getText().toString();

        AuthRequestTask request = new AuthRequestTask(this);
        request.execute(email, password);
    }

    private class AuthRequestTask extends AsyncTask<String, Void, Void> {
        private Context mContext;

        public AuthRequestTask (Context context){
            mContext = context;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            findViewById(R.id.buttonLogin).setClickable(false);
        }

        @Override
        protected Void doInBackground(String... params) {
            try {
                ApiAuthClient.Authorize(params[0], params[1], mContext);
            } catch (IOException ioException) {
                ioException.printStackTrace();
            }

            return null;
        }

        @Override
        protected void onPostExecute(Void result) {
            super.onPostExecute(result);

            if(ApiAuthClient.IsAuthenticated()) {
                goToMainActivity();
            } else {
                findViewById(R.id.buttonLogin).setClickable(true);
                String errorMessage = getResources()
                        .getString(ApiAuthClient.GetErrorMessageStringCode());
                Toast toast = Toast.makeText(mContext, errorMessage, Toast.LENGTH_LONG);
                toast.show();
            }
        }
    }

    private void goToMainActivity() {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
        finish();
    }
}