FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

ENV DEBIAN_FRONTEND=noninteractive
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV DOTNET_GCHeapHardLimitPercent=75

RUN apt-get update && \
    apt-get install -y fontconfig fonts-dejavu-core fonts-dejavu-extra && \
    fc-cache -f -v

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["NuGet.Config", "."]

COPY ["src/Core/Modsen.Portal.Core/Modsen.Portal.Core.csproj", "src/Core/Modsen.Portal.Core/"]
COPY ["src/Core/Modsen.Portal.Core.Authentication/Modsen.Portal.Core.Authentication.csproj", "src/Core/Modsen.Portal.Core.Authentication/"]
COPY ["src/Core/Modsen.Portal.Core.Context/Modsen.Portal.Core.Context.csproj", "src/Core/Modsen.Portal.Core.Context/"]
COPY ["src/Core/Modsen.Portal.Core.Services/Modsen.Portal.Core.Services.csproj", "src/Core/Modsen.Portal.Core.Services/"]
COPY ["src/Core/Modsen.Portal.Core.Exceptions/Modsen.Portal.Core.Exceptions.csproj", "src/Core/Modsen.Portal.Core.Exceptions/"]
COPY ["src/Core/Modsen.Portal.Core.Utils/Modsen.Portal.Core.Utils.csproj", "src/Core/Modsen.Portal.Core.Utils/"]
COPY ["src/Core/Modsen.Portal.Core.HostBuilder/Modsen.Portal.Core.HostBuilder.csproj", "src/Core/Modsen.Portal.Core.HostBuilder/"]
COPY ["src/Core/Modsen.Portal.Core.Api/Modsen.Portal.Core.Api.csproj", "src/Core/Modsen.Portal.Core.Api/"]
COPY ["src/Core/Modsen.Portal.Core.Database/Modsen.Portal.Core.Database.csproj", "src/Core/Modsen.Portal.Core.Database/"]
COPY ["src/Core/Modsen.Portal.Core.UseCases/Modsen.Portal.Core.UseCases.csproj", "src/Core/Modsen.Portal.Core.UseCases/"]
COPY ["src/Core/Modsen.Portal.Core.Communication/Modsen.Portal.Core.Communication.csproj", "src/Core/Modsen.Portal.Core.Communication/"]
COPY ["src/Core/Modsen.Portal.Core.Outbox/Modsen.Portal.Core.Outbox.csproj", "src/Core/Modsen.Portal.Core.Outbox/"]
COPY ["src/Core/Modsen.Portal.Core.Export/Modsen.Portal.Core.Export.csproj", "src/Core/Modsen.Portal.Core.Export/"]
COPY ["src/Core/Modsen.Portal.Core.OData/Modsen.Portal.Core.OData.csproj", "src/Core/Modsen.Portal.Core.OData/"]

COPY ["src/Infrastructure/Agent/Modsen.Portal.Agent/Modsen.Portal.Agent.csproj", "src/Infrastructure/Agent/Modsen.Portal.Agent/"]
COPY ["src/Infrastructure/Agent/Modsen.Portal.Agent.Contract/Modsen.Portal.Agent.Contract.csproj", "src/Infrastructure/Agent/Modsen.Portal.Agent.Contract/"]
COPY ["src/Infrastructure/Csv/Modsen.Portal.Csv/Modsen.Portal.Csv.csproj", "src/Infrastructure/Csv/Modsen.Portal.Csv/"]
COPY ["src/Infrastructure/StimulsoftReport/Modsen.Portal.StimulsoftReport.Contract/Modsen.Portal.StimulsoftReport.Contract.csproj", "src/Infrastructure/StimulsoftReport/Modsen.Portal.StimulsoftReport.Contract/"]
COPY ["src/Infrastructure/Excel/Modsen.Portal.Excel/Modsen.Portal.Excel.csproj", "src/Infrastructure/Excel/Modsen.Portal.Excel/"]
COPY ["src/Infrastructure/Migrator/Modsen.Portal.Migrator/Modsen.Portal.Migrator.csproj", "src/Infrastructure/Migrator/Modsen.Portal.Migrator/"]
COPY ["src/Infrastructure/Migrator/Modsen.Portal.Migrator.DataAccess/Modsen.Portal.Migrator.DataAccess.csproj", "src/Infrastructure/Migrator/Modsen.Portal.Migrator.DataAccess/"]
COPY ["src/Infrastructure/Modsen.Portal.Authentication.Microsoft/Modsen.Portal.Authentication.Microsoft.csproj", "src/Infrastructure/Modsen.Portal.Authentication.Microsoft/"]
COPY ["src/Infrastructure/Modsen.Portal.BackgroundJob.Hangfire/Modsen.Portal.BackgroundJob.Hangfire.csproj", "src/Infrastructure/Modsen.Portal.BackgroundJob.Hangfire/"]
COPY ["src/Infrastructure/Modsen.Portal.CloudStorage.AwsS3/Modsen.Portal.CloudStorage.AwsS3.csproj", "src/Infrastructure/Modsen.Portal.CloudStorage.AwsS3/"]
COPY ["src/Infrastructure/ModsenApi/Modsen.Portal.ModsenApi.Adapter/Modsen.Portal.ModsenApi.Adapter.csproj", "src/Infrastructure/ModsenApi/Modsen.Portal.ModsenApi.Adapter/"]
COPY ["src/Infrastructure/ModsenApi/Modsen.Portal.ModsenApi.Contract/Modsen.Portal.ModsenApi.Contract.csproj", "src/Infrastructure/ModsenApi/Modsen.Portal.ModsenApi.Contract/"]
COPY ["src/Infrastructure/StimulsoftReport/Modsen.Portal.StimulsoftReport/Modsen.Portal.StimulsoftReport.csproj", "src/Infrastructure/StimulsoftReport/Modsen.Portal.StimulsoftReport/"]
COPY ["src/Infrastructure/Modsen.Portal.PowerAutomate/Modsen.Portal.PowerAutomate.csproj", "src/Infrastructure/Modsen.Portal.PowerAutomate/"]
COPY ["src/Infrastructure/Jira/Modsen.Portal.Jira/Modsen.Portal.Jira.csproj", "src/Infrastructure/Jira/Modsen.Portal.Jira/"]
COPY ["src/Infrastructure/Jira/Modsen.Portal.Jira.Contract/Modsen.Portal.Jira.Contract.csproj", "src/Infrastructure/Jira/Modsen.Portal.Jira.Contract/"]
COPY ["src/Infrastructure/BcseApi/Modsen.Portal.BcseApi.Adapter/Modsen.Portal.BcseApi.Adapter.csproj", "src/Infrastructure/BcseApi/Modsen.Portal.BcseApi.Adapter/"]
COPY ["src/Infrastructure/BcseApi/Modsen.Portal.BcseApi.Contract/Modsen.Portal.BcseApi.Contract.csproj", "src/Infrastructure/BcseApi/Modsen.Portal.BcseApi.Contract/"]

COPY ["src/Modules/TimeTrack/Modsen.Portal.TimeTrack.UseCases/Modsen.Portal.TimeTrack.UseCases.csproj", "src/Modules/TimeTrack/Modsen.Portal.TimeTrack.UseCases/"]
COPY ["src/Modules/Identity/Modsen.Portal.Identity.Contract/Modsen.Portal.Identity.Contract.csproj", "src/Modules/Identity/Modsen.Portal.Identity.Contract/"]
COPY ["src/Modules/Notification/Modsen.Portal.Notification.Contract/Modsen.Portal.Notification.Contract.csproj", "src/Modules/Notification/Modsen.Portal.Notification.Contract/"]
COPY ["src/Modules/Storage/Modsen.Portal.Storage.Contract/Modsen.Portal.Storage.Contract.csproj", "src/Modules/Storage/Modsen.Portal.Storage.Contract/"]
COPY ["src/Modules/TeamBoard/Modsen.Portal.TeamBoard.Contract/Modsen.Portal.TeamBoard.Contract.csproj", "src/Modules/TeamBoard/Modsen.Portal.TeamBoard.Contract/"]
COPY ["src/Modules/TimeTrack/Modsen.Portal.TimeTrack.Contract/Modsen.Portal.TimeTrack.Contract.csproj", "src/Modules/TimeTrack/Modsen.Portal.TimeTrack.Contract/"]
COPY ["src/Modules/TimeTrack/Modsen.Portal.TimeTrack.DataAccess.Abstractions/Modsen.Portal.TimeTrack.DataAccess.Abstractions.csproj", "src/Modules/TimeTrack/Modsen.Portal.TimeTrack.DataAccess.Abstractions/"]
COPY ["src/Modules/TimeTrack/Modsen.Portal.TimeTrack.Domain/Modsen.Portal.TimeTrack.Domain.csproj", "src/Modules/TimeTrack/Modsen.Portal.TimeTrack.Domain/"]
COPY ["src/Modules/TimeTrack/Modsen.Portal.TimeTrack.DataAccess.Npgsql/Modsen.Portal.TimeTrack.DataAccess.Npgsql.csproj", "src/Modules/TimeTrack/Modsen.Portal.TimeTrack.DataAccess.Npgsql/"]
COPY ["src/Modules/Equipment/Modsen.Portal.Equipment.Requests/Modsen.Portal.Equipment.Requests.csproj", "src/Modules/Equipment/Modsen.Portal.Equipment.Requests/"]
COPY ["src/Modules/Equipment/Modsen.Portal.Equipment.Contract/Modsen.Portal.Equipment.Contract.csproj", "src/Modules/Equipment/Modsen.Portal.Equipment.Contract/"]
COPY ["src/Modules/Equipment/Modsen.Portal.Equipment.DataAccess.Npgsql/Modsen.Portal.Equipment.DataAccess.Npgsql.csproj", "src/Modules/Equipment/Modsen.Portal.Equipment.DataAccess.Npgsql/"]
COPY ["src/Modules/Booking/Modsen.Portal.Booking.DataAccess.Npgsql/Modsen.Portal.Booking.DataAccess.Npgsql.csproj", "src/Modules/Booking/Modsen.Portal.Booking.DataAccess.Npgsql/"]
COPY ["src/Modules/Cv/Modsen.Portal.Cv.DataAccess.Npgsql/Modsen.Portal.Cv.DataAccess.Npgsql.csproj", "src/Modules/Cv/Modsen.Portal.Cv.DataAccess.Npgsql/"]
COPY ["src/Modules/Identity/Modsen.Portal.Identity.DataAccess.Npgsql/Modsen.Portal.Identity.DataAccess.Npgsql.csproj", "src/Modules/Identity/Modsen.Portal.Identity.DataAccess.Npgsql/"]
COPY ["src/Modules/Inquiries/Modsen.Portal.Inquiries.DataAccess.Npgsql/Modsen.Portal.Inquiries.DataAccess.Npgsql.csproj", "src/Modules/Inquiries/Modsen.Portal.Inquiries.DataAccess.Npgsql/"]
COPY ["src/Modules/KnowledgeBase/Modsen.Portal.KnowledgeBase.DataAccess.Npgsql/Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.csproj", "src/Modules/KnowledgeBase/Modsen.Portal.KnowledgeBase.DataAccess.Npgsql/"]
COPY ["src/Modules/Merch/Modsen.Portal.Merch.DataAccess.Npgsql/Modsen.Portal.Merch.DataAccess.Npgsql.csproj", "src/Modules/Merch/Modsen.Portal.Merch.DataAccess.Npgsql/"]
COPY ["src/Modules/Notification/Modsen.Portal.Notification.DataAccess.Npgsql/Modsen.Portal.Notification.DataAccess.Npgsql.csproj", "src/Modules/Notification/Modsen.Portal.Notification.DataAccess.Npgsql/"]
COPY ["src/Modules/Storage/Modsen.Portal.Storage.DataAccess.Npgsql/Modsen.Portal.Storage.DataAccess.Npgsql.csproj", "src/Modules/Storage/Modsen.Portal.Storage.DataAccess.Npgsql/"]
COPY ["src/Modules/TeamBoard/Modsen.Portal.TeamBoard.DataAccess.Npgsql/Modsen.Portal.TeamBoard.DataAccess.Npgsql.csproj", "src/Modules/TeamBoard/Modsen.Portal.TeamBoard.DataAccess.Npgsql/"]
COPY ["src/Modules/Booking/Modsen.Portal.Booking.Controllers/Modsen.Portal.Booking.Controllers.csproj", "src/Modules/Booking/Modsen.Portal.Booking.Controllers/"]
COPY ["src/Modules/Booking/Modsen.Portal.Booking.Requests/Modsen.Portal.Booking.Requests.csproj", "src/Modules/Booking/Modsen.Portal.Booking.Requests/"]
COPY ["src/Modules/Booking/Modsen.Portal.Booking.Contract/Modsen.Portal.Booking.Contract.csproj", "src/Modules/Booking/Modsen.Portal.Booking.Contract/"]
COPY ["src/Modules/Certificate/Modsen.Portal.Certificate.Controllers/Modsen.Portal.Certificate.Controllers.csproj", "src/Modules/Certificate/Modsen.Portal.Certificate.Controllers/"]
COPY ["src/Modules/Certificate/Modsen.Portal.Certificate.UseCases/Modsen.Portal.Certificate.UseCases.csproj", "src/Modules/Certificate/Modsen.Portal.Certificate.UseCases/"]
COPY ["src/Modules/Cv/Modsen.Portal.Cv.Controllers/Modsen.Portal.Cv.Controllers.csproj", "src/Modules/Cv/Modsen.Portal.Cv.Controllers/"]
COPY ["src/Modules/Cv/Modsen.Portal.Cv.Requests/Modsen.Portal.Cv.Requests.csproj", "src/Modules/Cv/Modsen.Portal.Cv.Requests/"]
COPY ["src/Modules/Equipment/Modsen.Portal.Equipment.Controllers/Modsen.Portal.Equipment.Controllers.csproj", "src/Modules/Equipment/Modsen.Portal.Equipment.Controllers/"]
COPY ["src/Modules/Identity/Modsen.Portal.Identity.Controllers/Modsen.Portal.Identity.Controllers.csproj", "src/Modules/Identity/Modsen.Portal.Identity.Controllers/"]
COPY ["src/Modules/Identity/Modsen.Portal.Identity.Requests/Modsen.Portal.Identity.Requests.csproj", "src/Modules/Identity/Modsen.Portal.Identity.Requests/"]
COPY ["src/Modules/Inquiries/Modsen.Portal.Inquiries.Contract/Modsen.Portal.Inquiries.Contract.csproj", "src/Modules/Inquiries/Modsen.Portal.Inquiries.Contract/"]
COPY ["src/Modules/Inquiries/Modsen.Portal.Inquiries.Controllers/Modsen.Portal.Inquiries.Controllers.csproj", "src/Modules/Inquiries/Modsen.Portal.Inquiries.Controllers/"]
COPY ["src/Modules/Inquiries/Modsen.Portal.Inquiries.Requests/Modsen.Portal.Inquiries.Requests.csproj", "src/Modules/Inquiries/Modsen.Portal.Inquiries.Requests/"]
COPY ["src/Modules/KnowledgeBase/Modsen.Portal.KnowledgeBase.Controllers/Modsen.Portal.KnowledgeBase.Controllers.csproj", "src/Modules/KnowledgeBase/Modsen.Portal.KnowledgeBase.Controllers/"]
COPY ["src/Modules/KnowledgeBase/Modsen.Portal.KnowledgeBase.Requests/Modsen.Portal.KnowledgeBase.Requests.csproj", "src/Modules/KnowledgeBase/Modsen.Portal.KnowledgeBase.Requests/"]
COPY ["src/Modules/Merch/Modsen.Portal.Merch.Controllers/Modsen.Portal.Merch.Controllers.csproj", "src/Modules/Merch/Modsen.Portal.Merch.Controllers/"]
COPY ["src/Modules/Merch/Modsen.Portal.Merch.Requests/Modsen.Portal.Merch.Requests.csproj", "src/Modules/Merch/Modsen.Portal.Merch.Requests/"]
COPY ["src/Modules/Merch/Modsen.Portal.Merch.Contract/Modsen.Portal.Merch.Contract.csproj", "src/Modules/Merch/Modsen.Portal.Merch.Contract/"]
COPY ["src/Modules/Notification/Modsen.Portal.Notification.Controllers/Modsen.Portal.Notification.Controllers.csproj", "src/Modules/Notification/Modsen.Portal.Notification.Controllers/"]
COPY ["src/Modules/Notification/Modsen.Portal.Notification.UseCases/Modsen.Portal.Notification.UseCases.csproj", "src/Modules/Notification/Modsen.Portal.Notification.UseCases/"]
COPY ["src/Modules/Utilities/Modsen.Portal.Utilities.Controllers/Modsen.Portal.Utilities.Controllers.csproj", "src/Modules/Utilities/Modsen.Portal.Utilities.Controllers/"]
COPY ["src/Modules/Storage/Modsen.Portal.Storage.Controllers/Modsen.Portal.Storage.Controllers.csproj", "src/Modules/Storage/Modsen.Portal.Storage.Controllers/"]
COPY ["src/Modules/Storage/Modsen.Portal.Storage.Requests/Modsen.Portal.Storage.Requests.csproj", "src/Modules/Storage/Modsen.Portal.Storage.Requests/"]
COPY ["src/Modules/TeamBoard/Modsen.Portal.TeamBoard.Controllers/Modsen.Portal.TeamBoard.Controllers.csproj", "src/Modules/TeamBoard/Modsen.Portal.TeamBoard.Controllers/"]
COPY ["src/Modules/TeamBoard/Modsen.Portal.TeamBoard.Requests/Modsen.Portal.TeamBoard.Requests.csproj", "src/Modules/TeamBoard/Modsen.Portal.TeamBoard.Requests/"]
COPY ["src/Modules/TeamWork/Modsen.Portal.TeamWork.Controllers/Modsen.Portal.TeamWork.Controllers.csproj", "src/Modules/TeamWork/Modsen.Portal.TeamWork.Controllers/"]
COPY ["src/Modules/TeamWork/Modsen.Portal.TeamWork.UseCases/Modsen.Portal.TeamWork.UseCases.csproj", "src/Modules/TeamWork/Modsen.Portal.TeamWork.UseCases/"]
COPY ["src/Modules/TimeTrack/Modsen.Portal.TimeTrack.Controllers/Modsen.Portal.TimeTrack.Controllers.csproj", "src/Modules/TimeTrack/Modsen.Portal.TimeTrack.Controllers/"]
COPY ["src/Modules/Attendance/Modsen.Portal.Attendance.DataAccess.Npgsql/Modsen.Portal.Attendance.DataAccess.Npgsql.csproj", "src/Modules/Attendance/Modsen.Portal.Attendance.DataAccess.Npgsql/"]
COPY ["src/Modules/Attendance/Modsen.Portal.Attendance.Requests/Modsen.Portal.Attendance.Requests.csproj", "src/Modules/Attendance/Modsen.Portal.Attendance.Requests/"]
COPY ["src/Modules/Attendance/Modsen.Portal.Attendance.Controllers/Modsen.Portal.Attendance.Controllers.csproj", "src/Modules/Attendance/Modsen.Portal.Attendance.Controllers/"]
COPY ["src/Modules/Finance/Modsen.Portal.Finance.DataAccess.Npgsql/Modsen.Portal.Finance.DataAccess.Npgsql.csproj", "src/Modules/Finance/Modsen.Portal.Finance.DataAccess.Npgsql/"]
COPY ["src/Modules/Finance/Modsen.Portal.Finance.Requests/Modsen.Portal.Finance.Requests.csproj", "src/Modules/Finance/Modsen.Portal.Finance.Requests/"]
COPY ["src/Modules/Finance/Modsen.Portal.Finance.Controllers/Modsen.Portal.Finance.Controllers.csproj", "src/Modules/Finance/Modsen.Portal.Finance.Controllers/"]
COPY ["src/Modules/Finance/Modsen.Portal.Finance.Contract/Modsen.Portal.Finance.Contract.csproj", "src/Modules/Finance/Modsen.Portal.Finance.Contract/"]

COPY ["src/Modsen.Portal.Api/Modsen.Portal.Api.csproj", "src/Modsen.Portal.Api/"]
COPY ["src/Modsen.Portal.Api/packages.lock.json", "src/Modsen.Portal.Api/"]

RUN dotnet restore "./src/Modsen.Portal.Api/Modsen.Portal.Api.csproj" --locked-mode
COPY . .
WORKDIR "/src/src/Modsen.Portal.Api"
RUN dotnet build "Modsen.Portal.Api.csproj" \
    -c Release \
    -o /app/build \
    --no-restore \
    --nologo \
    -v q \
    --property WarningLevel=0 \
    /clp:ErrorsOnly

FROM build AS publish
ARG INFORMATIONAL_VERSION
RUN dotnet publish "Modsen.Portal.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:InformationalVersion="$INFORMATIONAL_VERSION" \
    --no-restore \
    --nologo \
    -v q \
    --property WarningLevel=0 \
    /clp:ErrorsOnly

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Modsen.Portal.Api.dll"]