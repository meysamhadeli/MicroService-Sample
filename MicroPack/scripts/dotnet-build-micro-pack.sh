#!/bin/bash
MYGET_ENV=""
case "${Branch_Name}" in
  "develop")
    MYGET_ENV="-dev"
    ;;
esac

dotnet build -c Release --no-cache