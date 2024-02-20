#!/bin/bash
# wait-db.sh
set -e

host="$1"
shift
user="$1"
shift
password="$1"
shift
cmd="$@"

until /opt/mssql-tools/bin/sqlcmd -S "$host" -U "$user" -P "$password" -Q "SELECT 1" >/dev/null 2>&1; do
  echo "Waiting for SQL Server at $host - sleeping"
  sleep 1
done

>&2 echo "SQL Server at $host is up - executing command"
exec $cmd
