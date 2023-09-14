dotnet publish -c Release
docker build -t rep-img:0001 -f Dockerfile .
kubectl apply -f Deployment.yaml