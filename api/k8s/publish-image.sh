#! /bin/sh

docker build --file $1 . -t $2
docker tag $2 $3
docker push $3
