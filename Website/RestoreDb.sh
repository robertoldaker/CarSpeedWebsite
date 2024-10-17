HOST=$1
FILE=$2
echo "$HOST"
echo "$FILE"
psql -h "$HOST" -U car_speed < "$FILE"