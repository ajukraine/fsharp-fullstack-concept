FROM mcr.microsoft.com/mssql/server:2017-CU8-ubuntu

ARG SA_PASSWORD

ENV ACCEPT_EULA Y
ENV MSSQL_PID Developer

RUN mkdir -p /usr/src/app
COPY ./run-init.sh /usr/src/app
COPY ./init.sql /usr/src/app

RUN  /bin/bash /usr/src/app/run-init.sh

CMD /opt/mssql/bin/sqlservr