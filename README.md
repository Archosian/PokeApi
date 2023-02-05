# Sonnet

A toy example that plugs two APIs together, "translating" the Pokemon's flavorText.

## Run

Run the API from `Sonnet/` with:

```dotnet run```

Alternatively, build and run via docker from `Sonnet/` by running:
```
docker build -t sonnet -f Dockerfile .
docker run -p 8080:80 sonnet:latest
``` 
This will expose the service locally on port `8080` for perusal. The Swagger UI is left on for perusal.

## Tests

You can run all tests directly from the root of the repo with `dotnet test`