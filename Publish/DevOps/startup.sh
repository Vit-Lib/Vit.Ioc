set -e

#----------------------------------------------
#(x.1)当前路径 
export curWorkDir=$PWD
export curPath=$(dirname $0)

cd $curPath/../..
export codePath=$PWD
cd $curWorkDir

#  export codePath=/root/docker/jenkins/workspace/sqler/svn


export name=Vit.Ioc
export projectPath=Vit.Ioc


#----------------------------------------------
echo "(x.2)get version" 
export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`
# echo $version


#----------------------------------------------
echo "(x.3)自动发布 $name-$version"

sh 20.docker-build.sh

sh 90.release-build.sh

sh 91.release-github.sh



 
#----------------------------------------------
#(x.9)
#cd $curWorkDir
