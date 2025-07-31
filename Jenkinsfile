// // Jenkinsfile for CI/CD Pipeline //
// // This Jenkinsfile defines a CI/CD pipeline for a .NET application that builds, tests, and deploys the application to Docker Hub and IIS.

// pipeline {
//     agent any
//       environment {
//         // docker environment variables
//         LANG = 'en_US.UTF-8'
//         LC_ALL = 'en_US.UTF-8'
// 		DOCKERHUB_CREDENTIALS = 'a8043e21-320b-4f12-b72e-612d7a93c553'  // ID credentials from jenkins
//         IMAGE_NAME = 'zyond/cicd'  // name of image on Docker Hub -- create repo on hub.docker
// 		DOCKER_IMAGE_NAME = 'zyond/cicd'  //  Docker image name
//         DOCKER_TAG = 'latest'  // Tag cho Docker image
        
// }
//     stages {

//         // Clone the source code from GitHub
//         stage('Clone') {
//             steps {
//                 echo 'Cloning source code'
//                 git branch: 'master', url: 'https://github.com/zyond26/Restaurant_cicd.git'
//             }
//         }
//         // Restore NuGet packages 
//         stage('Restore Packages') {
//             steps {
//                 echo 'Restoring NuGet packages...'
//                 bat 'dotnet restore'
//             }
//         }
//         // Build the project
//         stage('Build') {
//             steps {
//                 echo 'Building the project...'
//                 bat 'dotnet build --configuration Release'
//             }
//         }
//         // run tests
//         stage('Run Tests') {
//             steps {
//                 echo 'Running unit tests...'
//                 bat 'dotnet test --no-build --verbosity normal'
//             }
//         }
//         // Publish   to a folder
//         stage('Publish to Folder') {
//             steps {
//                 echo 'Cleaning old publish folder...'
//                 bat 'if exist "%WORKSPACE%\\publish" rd /s /q "%WORKSPACE%\\publish"'
                
//                 echo 'Publishing to temporary folder...'
//                 bat 'dotnet publish -c Release -o "%WORKSPACE%\\publish"'
//             }
//         }

//         //--------------------   automated deployment to Docker Hub  -------------------------

//         // Build Docker Image
//         stage('Build Docker Image') {
//             steps {
//                 script {
//                     docker.build("${DOCKER_IMAGE_NAME}:latest")
//                 }
//             }
//         }
		
//         // Login to Docker Hub
//         stage('Login to Docker Hub') {
//             steps {
//                 script {
//                     docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
//                         // login Docker Hub credentials
//                     }
//                 }
//             }
//         }
//         // Push Docker Image to Docker Hub
//         stage('Push Docker Image') {
//             steps {
				 
//                 script {
//                     docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
//                         docker.image("${DOCKER_IMAGE_NAME}:${DOCKER_TAG}").push()
//                     }
//                 }
//             }
//         }

//         //---------------------  upload to minio  ------------------------

//         stage('Tạo file test') {
//             steps {
//                 bat 'echo Build thành công > build-log.txt'
//             }
//         }

//         stage('Cấu hình AWS CLI cho MinIO') {
//             steps {
//                 bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" configure set aws_access_key_id admin'
//                 bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" configure set aws_secret_access_key 12345678'
//                 // bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" --endpoint-url http://localhost:9000 s3 cp build-log.txt s3://order-files/build-log.txt'

//             }
//         }

//         stage('Upload file lên MinIO') {
//             steps {
//                 bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" --endpoint-url http://localhost:9000 s3 cp build-log.txt s3://order-files/build-log.txt'
//             }
//         }

 
//      //---------------------  automated deployment to IIS -------------------------

//         // Copy to IIS Folder
//         stage('Copy to IIS Folder') {
//             steps {
//                 echo 'Stopping IIS...'
//                 bat 'iisreset /stop'

//                 echo 'Cleaning existing deploy folder...'
//                 bat 'if exist C:\\WOR_cicd rd /s /q C:\\WOR_cicd'

//                 echo 'Creating IIS folder...'
//                 bat 'mkdir C:\\WOR_cicd'

//                 echo 'Copying to IIS folder...'
//                 bat 'xcopy /E /Y /I /R "%WORKSPACE%\\publish\\*" "C:\\WOR_cicd\\"'

//                 echo 'Starting IIS again...'
//                 bat 'iisreset /start'
//             }
//         }
//         // Ensure IIS Site Exists
//         stage('Ensure IIS Site Exists') {
//             steps {
//                 powershell '''
//                     Import-Module WebAdministration

//                     $siteName = "WOR_cicd"
//                     $sitePath = "C:\\WOR_cicd"
//                     $sitePort = 8089

//                     if (-not (Test-Path "IIS:\\Sites\\$siteName")) {
//                         New-Website -Name $siteName -Port $sitePort -PhysicalPath $sitePath -Force
//                     } else {
//                         Write-Host "Website $siteName already exists"
//                     }
//                 '''
//             }
//         }

//     }
// }okee



pipeline {
    agent any
    environment {
        LANG = 'en_US.UTF-8'
        LC_ALL = 'en_US.UTF-8'
        DOCKERHUB_CREDENTIALS = 'a8043e21-320b-4f12-b72e-612d7a93c553'
        IMAGE_NAME = 'zyond/cicd'
        DOCKER_IMAGE_NAME = 'zyond/cicd'
        DOCKER_TAG = 'latest'
        MINIO_CREDENTIALS = 'ec062030-09a1-4183-8f4f-81e593dacae3'  // ID của credentials MinIO trong Jenkins
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
        stage('Build Docker Image') {
            steps {
                script {
                    docker.build("${DOCKER_IMAGE_NAME}:latest")
                }
            }
        }
        stage('Login to Docker Hub') {
            steps {
                script {
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        // login Docker Hub credentials
                    }
                }
            }
        }
        stage('Push Docker Image') {
            steps {
                script {
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        docker.image("${DOCKER_IMAGE_NAME}:${DOCKER_TAG}").push()
                    }
                }
            }
        }
        
        // -      mino -----------------------
        stage('Tạo file test') {
            steps {
                bat 'echo Build thành công > build-log.txt'
            }
        }
        stage('Cấu hình AWS CLI cho MinIO') {
            steps {
                bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" configure set aws_access_key_id admin'
                bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" configure set aws_secret_access_key 12345678'

            }
        }

        stage('Upload file lên MinIO') {
            steps {
                bat '"C:\\Program Files\\Amazon\\AWSCLIV2\\aws.exe" --endpoint-url http://minio.localhost s3 cp WebRestaurant12_autobackup_629062_2025-07-28T10-16-35.BAK s3://order-files/WebRestaurant12_autobackup_629062_2025-07-28T10-16-35.BAK'
            }
        }

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