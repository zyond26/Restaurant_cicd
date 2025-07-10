// pipeline {
//     agent any
//     environment {
//         RENDER_API_KEY = credentials('render-api-key') // ID của Render API Key
//         SERVICE_ID = 'srv-d1bd0hqdbo4c73cdeif0' // Service ID của bạn
//     }
//     stages {
//         stage('Restore') {
//             steps {
//                 bat 'dotnet restore Web_Restaurant.csproj'
//             }
//         }
//         stage('Build') {
//             steps {
//                 bat 'dotnet build Web_Restaurant.csproj -c Release --no-restore'
//             }
//         }
//         stage('Test') {
//             steps {
//                 echo 'Chạy unit tests (thêm lệnh test nếu có)'
//                 // Nếu có project test: bat 'dotnet test tests\\YourTestProject.csproj'
//             }
//         }
//         stage('Deploy to Render') {
//             steps {
//                 script {
//                     def payload = '''{
//                         "clearCache": true
//                     }'''
//                     bat """
//                         curl -X POST ^
//                         -H "Authorization: Bearer %RENDER_API_KEY%" ^
//                         -H "Content-Type: application/json" ^
//                         -d "%payload%" ^
//                         https://api.render.com/v1/services/%SERVICE_ID%/deploys
//                     """
//                 }
//             }
//         }
//     }
//     post {
//         always {
//             echo 'Pipeline hoàn thành!'
//         }
//         success {
//             echo 'Triển khai thành công lên Render!'
//         }
//         failure {
//             echo 'Pipeline thất bại. Kiểm tra log để biết chi tiết.'
//         }
//     }
// }

pipeline {
    agent any
      environment {
        LANG = 'en_US.UTF-8'
        LC_ALL = 'en_US.UTF-8'
		DOCKERHUB_CREDENTIALS = 'a8043e21-320b-4f12-b72e-612d7a93c553'  // ID credentials
        IMAGE_NAME = 'zyond/cicd'  // name of image on Docker Hub -- create repo on hub.docker
		DOCKER_IMAGE_NAME = 'zyond/cicd'  //  Docker image name
        DOCKER_TAG = 'latest'  // Tag cho Docker image
    }
    stages {
        stage('Clone') {
            steps {
                echo 'Cloning source code'
                git branch: 'master', url: 'https://github.com/zyond26/Restaurant_cicd.git'
            }
        }

        stage('Restore Packages') {
            steps {
                echo 'Restoring NuGet packages...'
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                echo 'Building the project...'
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Run Tests') {
            steps {
                echo 'Running unit tests...'
                bat 'dotnet test --no-build --verbosity normal'
            }
        }

        stage('Publish to Folder') {
            steps {
                echo 'Cleaning old publish folder...'
                bat 'if exist "%WORKSPACE%\\publish" rd /s /q "%WORKSPACE%\\publish"'
                
                echo 'Publishing to temporary folder...'
                bat 'dotnet publish -c Release -o "%WORKSPACE%\\publish"'
            }
        }

// automated deployment to Docker Hub 
        stage('Build Docker Image') {
            steps {
                script {
                    // Build Docker image from Dockerfile
                    docker.build("${DOCKER_IMAGE_NAME}:latest")
                }
            }
        }
		

        stage('Login to Docker Hub') {
            steps {
                script {
                    // login Docker Hub to push image
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        // login Docker Hub credentials
                    }
                }
            }
        }
		 
        stage('Push Docker Image') {
            steps {
				 
                script {
                    // push Docker image to Docker Hub
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        docker.image("${DOCKER_IMAGE_NAME}:${DOCKER_TAG}").push()
                    }
                }
            }
        }

// automated deployment to IIS
        stage('Copy to IIS Folder') {
            steps {
                echo 'Stopping IIS...'
                bat 'iisreset /stop'

                echo 'Cleaning existing deploy folder...'
                bat 'if exist C:\\WOR_cicd rd /s /q C:\\WOR_cicd'

                echo 'Creating IIS folder...'
                bat 'mkdir C:\\WOR_cicd'

                echo 'Copying to IIS folder...'
                bat 'xcopy /E /Y /I /R "%WORKSPACE%\\publish\\*" "C:\\WOR_cicd\\"'

                echo 'Starting IIS again...'
                bat 'iisreset /start'
            }
        }

        stage('Ensure IIS Site Exists') {
            steps {
                powershell '''
                    Import-Module WebAdministration

                    $siteName = "WOR_cicd"
                    $sitePath = "C:\\WOR_cicd"
                    $sitePort = 8089

                    if (-not (Test-Path "IIS:\\Sites\\$siteName")) {
                        New-Website -Name $siteName -Port $sitePort -PhysicalPath $sitePath -Force
                    } else {
                        Write-Host "Website $siteName already exists"
                    }
                '''
            }
        }
    }
}
