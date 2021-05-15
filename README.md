# grpc-demo
Demo of gRPC service in docker + donet 5 + ef Core + integration tests

# Interacting with the service

See [link](https://docs.microsoft.com/en-us/aspnet/core/grpc/test-tools?view=aspnetcore-5.0) for more documentation, but basically there is grpcurl which will enable the developer to interact with the service via cmd. Then there is grpcui which builds ontop of grpcurl and provides a ui sort of like postman or swagger ui. 

Note: You ahve to download the relased versions of each from github and sotre in an easily accessable dir and use it similar to how you'd use the nuget.exe for example.

You will need to setup [grpcReflection](https://github.com/grpc/grpc/blob/master/doc/server-reflection.md) and install nuget package:  [Grpc.AspNetCore.Server.Reflection](https://www.nuget.org/packages/Grpc.AspNetCore.Server.Reflection)

You'll need to add line 20 and 44


### Commands 
These commands are run against the basic greeter service

#### giturl
Request:
```
grpcurl -d "{\"name\": \"World\"}" localhost:5001 greet.Greeter/SayHello
```

Response:
```
{
  "message": "Hello World"
}
```
note the double quotes this is for some cmd weirdness on windows.

