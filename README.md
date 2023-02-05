# Sonnet

A toy example that plugs two APIs together, "translating" the Pokemon's flavorText.

## Run

Run the API from `Sonnet/` with `dotnet run`. Alternatively, build and run via docker from `Sonnet/` by running `docker build -t sonnet -f Dockerfile .` to build the `sonnet:latest` container, followed by `docker run -p 8080:80 sonnet:latest` to run locally on port `8080`.

## Tests

You can run all tests directly from the root of the repo with `dotnet test`