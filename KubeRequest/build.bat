dotnet publish -c Release
docker build -t krq-img:0001 -f Dockerfile .
kubectl apply -f Deployment.yaml
kubectl apply -f role.yaml