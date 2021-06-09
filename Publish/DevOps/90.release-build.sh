set -e

# bash 90.release-build.sh



#(x.1)当前路径 
curWorkDir=$PWD
curPath=$(dirname $0)

cd $curPath/../..
codePath=$PWD
# codePath=/root/docker/jenkins/workspace/Vit.Ioc/svn


# export GIT_SSH_SECRET=xxxxxx
export name=Vit.Ioc

echo "(x.2)get version" 
version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -o '[0-9][0-9\.]\+'`
# echo $version




#----------------------------------------------
echo "(x.2)构建最终文件夹"
mkdir -p $codePath/Publish/git

cp -rf  $codePath/Publish/nuget $codePath/Publish/release
 
docker run --rm -i \
-v $codePath/Publish:/root/file \
serset/filezip dotnet FileZip.dll zip -i /root/file/release -o /root/file/git/${name}${version}.zip

 

#----------------------------------------------
echo "(x.3)提交release文件到github"
# releaseFile=$codePath/Publish/git/${name}${version}.zip

#复制ssh key
cd $codePath/Publish
echo "${GIT_SSH_SECRET}" > $codePath/Publish/git/serset
chmod 600 $codePath/Publish/git/serset

#推送到github
docker run -i --rm -v $PWD/git:/root/git serset/git-client bash -c " \
set -e
ssh-agent bash -c \"
ssh-add /root/git/serset
ssh -T git@github.com -o StrictHostKeyChecking=no
git config --global user.email 'serset@yeah.com'
git config --global user.name 'lith'
mkdir -p /root/code
cd /root/code
git clone git@github.com:serset/release.git /root/code
mkdir -p /root/code/file/${name}
cp /root/git/${name}${version}.zip /root/code/file/${name}
git add file/${name}/${name}${version}.zip
git commit  -m  'auto commit ${version}'
git push -u origin master \" "




#(x.5)
cd $curWorkDir

 
