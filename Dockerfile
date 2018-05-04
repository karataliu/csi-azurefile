FROM microsoft/dotnet:2.0-sdk-stretch as build
WORKDIR /build
COPY src .
RUN dotnet publish Csi.Plugins.AzureFile/Csi.Plugins.AzureFile.csproj -c Release -o /build/_output

FROM microsoft/dotnet:2.0-runtime-stretch
RUN apt update && apt install --no-install-recommends -y socat cifs-utils
COPY --from=build /build/_output /opt/csi-azurefile
CMD dotnet /opt/csi-azurefile/Csi.Plugins.AzureFile.dll
