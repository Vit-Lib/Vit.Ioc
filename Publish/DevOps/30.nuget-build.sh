set -e

# bash 30.nuget-build.sh



#(x.1)当前路径 
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/../..
codePath=$PWD
# codePath=/root/docker/jenkins/workspace/Vit.Ioc/svn

# export NUGET_SERVER=https://api.nuget.org/v3/index.json
# export NUGET_SERVER=http://nuget.sers.cloud:10008
# export NUGET_KEY=xxxxxxxxxx
export name=Vit.Ioc


echo "(x.2)get version" 
version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -o '[0-9][0-9\.]\+'`
# echo $version





 

#----------------------------------------------
echo "(x.2)构建nuget包并推送到nuget server"
docker run -i --rm \
--env LANG=C.UTF-8 \
-v $codePath:/root/code \
serset/dotnet:6.0-sdk \
bash -c "cd /root/code/Vit.Ioc;
dotnet build --configuration Release;
dotnet pack --configuration Release --output '/root/code/Publish/nuget';

# push to nuget server
for file in /root/code/Publish/nuget/*.nupkg ; 
do
    dotnet nuget push \$file -k ${NUGET_KEY} -s ${NUGET_SERVER};
done
" 






#(x.5)
cd $curWorkDir

 
