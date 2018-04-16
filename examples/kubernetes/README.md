# Azure File CSI plugin on Kubernetes

## Prerequisite
An azure stroage account

## Deploy
1. Fill in storage account information in secret.env
2. Deploy Azure File CSI plugin, run following
```
kubectl create ns csi-plugins-azurefile
kubectl --namespace=csi-plugins-azurefile create secret generic csi-azurefile-secret --from-env-file=secret.env
kubectl --namespace=csi-plugins-azurefile create -f .
```
3. Deploy demo pod, run following
```
kubectl create ns demo
kubectl --namespace=demo create -f pod
kubectl --namespace=demo get pods
kubectl --namespace=demo get pvc
```

## Cleanup
```
kubectl delete ns demo

# pending pvc clean up, after demo namespace deleted
kubectl delete ns csi-plugins-azurefile

kubectl delete clusterrole external-attacher-runner
kubectl delete clusterrole external-provisioner-runner
kubectl delete clusterrole csi-plugin-azurefile-runner

kubectl delete clusterrolebinding csi-attacher-role
kubectl delete clusterrolebinding csi-provisioner-role
kubectl delete clusterrolebinding csi-plugin-azurefile-role

kubectl delete storageclasses azurefile-csi

```
