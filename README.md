# Pastebin
This is a command line application which uses the Pastebin api to manage pastebin repositories.

Build the application in visual studios (tested in VS12 and VS15).

To use-
When you start the application, it will ask you for your encryption key. This is a password which is used to encypt all of your important personal settings. It is not stored, and you will only know that you have entered it incorrectly when your settings fail to deserialize.
After you've supplied your encryption key, you will be presented with a command prompt.
All commands start with the prefix "cmd" (e.g. "cmd help" for general help).

All commands can be specified as commandline options (e.g. "cmd login") which are executed after you supply your encryption key.

In addition, if you have the login command specified, you can drag files onto a Pastebin shortcut, and it will automatically try to paste the files to pastebin for you.
