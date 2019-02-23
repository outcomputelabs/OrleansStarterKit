@csmacnz.coveralls^
 --opencover^
 -i ./coverage.xml^
 --repoToken %COVERALLS_REPO_TOKEN%^
 --commitId %APPVEYOR_REPO_COMMIT%^
 --commitBranch %APPVEYOR_REPO_BRANCH%^
 --commitAuthor %APPVEYOR_REPO_COMMIT_AUTHOR%^
 --commitEmail %APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL%^
 --commitMessage %APPVEYOR_REPO_COMMIT_MESSAGE%^
 --jobId %APPVEYOR_JOB_ID%