# HttpClient-CopyToAsyncCore-Ex
Sample app, to demonstrate issue: https://github.com/dotnet/corefx/issues/23504

Issue is reproduced when conditions are met:
1) Kestrel self-hosted web app as a server.
   - "System.Net.Http.HttpRequestException: Error while copying content to a stream." 
     is reproduced, only in case server is running as self-host.
   - "System.Net.Http.WinHttpException: Not enough storage is available to process this command"
     is reproduced for self-host and IIS Express also, when there about 200 outgoing requests or more.
2) Http client is used for "Post" inside Task.Run 
   - sample: Task.Run(async() => { ... await httpClient.PostAsync(...); })
   - reproduced for PostAsync and SendAsync with HttpPost method.
3) Windows OS. 
   - reproduced in Windows 10
   - works fine in linux docker container (could handle tens of thousands requests without exceptions).
4) Reproduced when 150-1000 requests are issued in short term.
