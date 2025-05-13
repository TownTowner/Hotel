#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Hotel/Hotel.csproj", "Hotel/"]
COPY ["Hotel.Core/Hotel.Core.csproj", "Hotel.Core/"]
RUN dotnet restore "Hotel/Hotel.csproj"
COPY . .
WORKDIR "/src/Hotel"
RUN dotnet build "Hotel.csproj" -c Release -o /app/build

FROM build AS publish
ENV ASPNETCORE_ENVIRONMENT=Production
RUN dotnet publish "Hotel.csproj" -c Release -o /app/publish /p:UseAppHost=false

#React build
#http://alakbarv.azurewebsites.net/2019/01/11/dockerize-asp-net-core-angular-application-solving-error-the-command-npm-install-exited-with-code-127/
FROM node:16 as nodebuilder

# set working directory
RUN mkdir /usr/src/app
WORKDIR /usr/src/app

# add `/usr/src/app/node_modules/.bin` to $PATH
ENV PATH /usr/src/app/node_modules/.bin:$PATH

# install and cache app dependencies
COPY ["Hotel/ClientApp/package.json", "/usr/src/app/package.json"]
#有时会报错npm 有可用升级
RUN npm install -g npm@8.19.4
RUN npm install
#RUN npm install -g @angular/cli@1.7.0 --unsafe

# add app
COPY ["Hotel/ClientApp/.","/usr/src/app"]

RUN npm run build
#End React build


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#Start React copy
RUN mkdir -p /app/ClientApp/build
##debug
#RUN echo ${PWD} && ls -lR
COPY --from=nodebuilder /usr/src/app/build/. /app/ClientApp/build/
#End React copy

ENTRYPOINT ["dotnet", "Hotel.dll"]