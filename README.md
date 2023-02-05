# Sonnet

A toy example that plugs two APIs together, "translating" the Pokemon's flavorText.

## Definitions

The Swagger UI is left running for easier browsing of the numerous (1) endpoints:
```
/swagger
```

See the complete spec and definitions in the UI, or directly at the OpenAPI spec:
```
swagger/v1/swagger.json
```

Get a `Pokemon` object and its translation from:
```
/api/pokemon/{identifier}
```

## Run

Run the API from `Sonnet/` with:

```dotnet run```

The API will be exposed on `localhost:5270/`

Alternatively, build and run via docker from `Sonnet/` by running:
```
docker build -t sonnet -f Dockerfile .
docker run -p 8080:80 sonnet:latest
``` 
This will expose the service locally on port `8080` for perusal.

## Tests

You can run all tests directly from the root of the repo with `dotnet test`