## Simple Web Server ##

### Summary ###

This package includes a simple web server which accepts only one GET request before it disposes itself. The purpose of this web server is to server the OAuth redirect URL locally. 

### Details ###

When using OAuth 2.0 workflow to authorize an application to resources, there is usually requirement of a redirect URL in which the **authorization code** is encoded as query string. The authorization grant is then used to obtain an **access token** for the application to access the required resources.

The redirect URL is registered with service provider while registering the an application to access the resources hosted by the provider. 

The redirect URL can be a WEB application on the internet, with which the application can communicate to obtain the encoded authorization code. This involves multiple network round trip and pooling.

As an alternative, we can register a localhost URL as redirect URL and starts a local web server to retrieve the authorization code instead. A web server that can receive GET request and parse query string will do the job.

This Simple Web Server is to fullfill this purpose.

### Usages ###

1. Add the package to your project:
   ```
   dotnet add pacakge zhusmelb.NetLib.Http.SimpleWebServer
   ```
