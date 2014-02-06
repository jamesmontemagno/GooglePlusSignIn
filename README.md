GooglePlusSignIn
================

Sample of Google Plus Sign in and +1 in Xamarin.Android

Blog Post: http://motzcod.es/post/67077106339/google-plus-services-login-and-1-in-xamarin-android


Ensure that you setup and register your keystore for deployment! I have directions in the blog:

Settings up Google App & API:
The next part is bit in depth as you will need to setup a google app and the API that the google play services will talk to. Google has a pretty great guide, but here are the pointers: *Note I am doing this on Mac OSX 10.9, on Windows the locations will differ.

1.) Create new app at Google App Console

2.) Go to Services and enable Google+ API

For this next part I switched back to the old console

3.) Get the SHA1 for your keystore (I am using debug to test, but you will want to use a final release keystore that you have to create before you ship).

4.) Find your debug.keystore in: /Users/<UserName>/.local/share/Xamarin/Mono for Android/debug.keystore

5.) Copy it into Java in /Library/Java/Home/bin

6.) Run this command in terminal after navigating to that directory:


keytool -exportcert -alias androiddebugkey -keystore -list -v

7.) This will output your SHA1 you will need:

8.) Use this to add oAuth to your app!
