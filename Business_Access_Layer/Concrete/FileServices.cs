﻿using Business_Access_Layer.Abstract;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Business_Access_Layer.Concrete
{
    public class FileServices:IFileServices
    {

        private static string ApiKey = "AIzaSyAeiPs8hJAXqkh2amJI75CpXksPOvDWh68";
        private static string Bucket = "imagenet-5a741.appspot.com";
        private static string AuthEmail = "es@gmail.com";
        private static string AuthPassword = "asdf12";
        public FileServices()
        {

        }
       
        public async Task<string> SaveImage(IFormFile imageFile, string fileName)
        {
            var stream = new MemoryStream();
            imageFile.CopyTo(stream);
            stream.Seek(0, SeekOrigin.Begin); // Reset the stream position to the beginning

            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("images")
                .Child(fileName + ".jpg")
                .PutAsync(stream, cancellation.Token);

            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // cancel the upload
            // cancellation.Cancel();

            try
            {
                string link = await task;
                return link;
                // error during upload will be thrown when you await the task

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return "";
            }
        }
    }
}
