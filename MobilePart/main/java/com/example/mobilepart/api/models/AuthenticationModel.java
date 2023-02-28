package com.example.mobilepart.api.models;

import java.util.Date;
import java.util.List;

public class AuthenticationModel {
    public String message;
    public boolean isAuthenticated;
    public String userName;
    public String email;
    public List<String> roles;
    public String token;
    public String refreshToken;
    public Date refreshTokenExpiration;

    public AuthenticationModel() {}
}
