@csmacnz.coveralls --opencover -i ./coverage.xml --repoToken %COVERALLS_REPO_TOKEN%

 rem --commitId %APPVEYOR_REPO_COMMIT%^
 rem --commitBranch %APPVEYOR_REPO_BRANCH%^
 rem --commitAuthor %APPVEYOR_REPO_COMMIT_AUTHOR%^
 rem --commitMessage %APPVEYOR_REPO_COMMIT_MESSAGE%^
 rem --jobId %APPVEYOR_JOB_ID%
