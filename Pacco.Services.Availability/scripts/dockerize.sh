TAG=''
VERSION_TAG=

case "${Branch_Name}" in
  "master")
    TAG=latest
    VERSION_TAG=${Github_ID}
    ;;
  "develop")
    TAG=dev
    VERSION_TAG=${Github_ID}
    ;;    
esac

REPOSITORY=$DOCKER_USERNAME/pacco.services.availability

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD
docker build -t $REPOSITORY:$TAG -t $REPOSITORY:$VERSION_TAG .
docker push $REPOSITORY:$TAG
docker push $REPOSITORY:$VERSION_TAG