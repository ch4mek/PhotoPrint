# Modsen Portal k8s Setup

## Kubernetes Cluster

### Setup Cluster

1. Setup namespace
   ```
   ./setup-cluster.sh modsen-portal dev
   ```
2. Switch namespace
   ```
   kubectl config set-context --current --namespace modsen-portal-dev
   ``` 
3. Install shared databases
   - Postgres 
    ```
    cd ./helmcharts/postgres
    helm upgrade --install postgres-modsen-portal-dev . --namespace modsen-portal-dev
    ```
4. Connect to private container Registry
   ```
   kubectl create secret docker-registry modsen-portal-container-registry --docker-username=...  --docker-password=... --docker-email=... --docker-server=...
   ```
