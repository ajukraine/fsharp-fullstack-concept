/opt/mssql/bin/sqlservr &

echo "[CUSTOM] Waiting for sql server to warm up..."

sleep 40s

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i /usr/src/app/init.sql