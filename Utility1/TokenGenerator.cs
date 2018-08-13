﻿using System;
using Twilio;
using System.Collections.Generic;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;
using System.Linq;



namespace VideoApplication.Utility1
{
    public static class TokenGenerator
    {
        public static string AccessTokenGenerator()
        {
            // Substitute your Twilio AccountSid and ApiKey details
            var AccountSid = "AC629d1233ead64b5341d2728f5eb160dc";
            var ApiKeySid = "SK0678c0ad0045e85ac68f91d3eca7c87c";
            var ApiKeySecret = "6AQNum22c9t20kZsbGXffSZQDpEyjyWs";
            TwilioClient.Init(ApiKeySid, ApiKeySecret);           

            //Console.WriteLine(room.Sid);
            var identity = "bhanus";

            // Create a video grant for the token
            var grant = new VideoGrant();
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            Random rndm = new Random();
            string ServerVariable = new string(Enumerable.Repeat(letters, 10).Select(s => s[rndm.Next(s.Length)]).ToArray());
        
            grant.Room ="bhanu";
            var grants = new HashSet<IGrant> { grant };

            // Create an Access Token generator
            var token = new Token(AccountSid, ApiKeySid, ApiKeySecret, identity: identity, grants: grants);

            // Serialize the token as a JWT
            String Jwttoken = token.ToJwt();
            Console.WriteLine(token.ToJwt());
            return Jwttoken;       

           
        }

    }
    
}