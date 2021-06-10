set -e


#---------------------------------------------------------------------
#(x.1)����
args_="

export codePath=/root/docker/jenkins/workspace/sqler/svn 



export version=`grep '<Version>' ${codePath} -r --include *.csproj | grep -oP '>(.*)<' | tr -d '<>'`

export name=Vit.Ioc

# "




#---------------------------------------------------------------------
#(x.2)��ʼ��github release��������
# releaseFile=$codePath/Publish/git/${name}-${version}.zip

filePath="$codePath/Publish/git/${name}-${version}.zip"
#name=Vit.Library
#version=2.5



fileType="${filePath##*.}"
echo "release_name=${name}-${version}" >> $GITHUB_ENV
echo "release_tag=${version}" >> $GITHUB_ENV

echo "release_draft=false" >> $GITHUB_ENV
echo "release_prerelease=false" >> $GITHUB_ENV

echo "release_body=" >> $GITHUB_ENV

echo "release_assetPath=${filePath}" >> $GITHUB_ENV
echo "release_assetName=${name}-${version}.${fileType}" >> $GITHUB_ENV
echo "release_contentType=application/${fileType}" >> $GITHUB_ENV




 