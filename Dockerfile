# Build asamas
# WinForms uygulamasi oldugu icin sadece build alinir, calistirilmaz.
# Uygulama Windows ortaminda calistirilmalidir.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

COPY PCBBaglantiAgiOptimizasyonu/PCBBaglantiAgiOptimizasyonu.csproj ./PCBBaglantiAgiOptimizasyonu/
RUN dotnet restore ./PCBBaglantiAgiOptimizasyonu/PCBBaglantiAgiOptimizasyonu.csproj

COPY . .

RUN dotnet publish ./PCBBaglantiAgiOptimizasyonu/PCBBaglantiAgiOptimizasyonu.csproj \
    -c Release \
    -r win-x64 \
    --self-contained true \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS output

WORKDIR /app
COPY --from=build /app/publish .

CMD ["echo", "Build basarili. Uygulamayi calistirmak icin publish/ klasoründeki .exe dosyasini Windows ortaminda kullaniniz."]

