dotnet run Bundle
docker build -t wilkens-avenue .
heroku container:push -a wilkens-avenue web
heroku container:release -a wilkens-avenue web
heroku container:release web -a wilkens-avenue