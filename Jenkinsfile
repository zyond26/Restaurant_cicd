// Jenkinsfile for CI/CD Pipeline
// This Jenkinsfile defines a CI/CD pipeline for a .NET application that builds, tests, and deploys the application to Docker Hub and IIS.

pipeline {
    agent any
      environment {
        // docker environment variables
        LANG = 'en_US.UTF-8'
        LC_ALL = 'en_US.UTF-8'
		DOCKERHUB_CREDENTIALS = 'a8043e21-320b-4f12-b72e-612d7a93c553'  // ID credentials from jenkins
        IMAGE_NAME = 'zyond/cicd'  // name of image on Docker Hub -- create repo on hub.docker
		DOCKER_IMAGE_NAME = 'zyond/cicd'  //  Docker image name
        DOCKER_TAG = 'latest'  // Tag cho Docker image


        // MinIO environment variables
        MINIO_ENDPOINT = 'http://minio-server:9000'
        MINIO_BUCKET = 'jenkins-artifacts'
}
    stages {

        // Clone the source code from GitHub
        stage('Clone') {
            steps {
                echo 'Cloning source code'
                git branch: 'master', url: 'https://github.com/zyond26/Restaurant_cicd.git'
            }
        }
        // Restore NuGet packages 
        stage('Restore Packages') {
            steps {
                echo 'Restoring NuGet packages...'
                bat 'dotnet restore'
            }
        }
        // Build the project
        stage('Build') {
            steps {
                echo 'Building the project...'
                bat 'dotnet build --configuration Release'
            }
        }
        // run tests
        stage('Run Tests') {
            steps {
                echo 'Running unit tests...'
                bat 'dotnet test --no-build --verbosity normal'
            }
        }
        // Publish   to a folder
        stage('Publish to Folder') {
            steps {
                echo 'Cleaning old publish folder...'
                bat 'if exist "%WORKSPACE%\\publish" rd /s /q "%WORKSPACE%\\publish"'
                
                echo 'Publishing to temporary folder...'
                bat 'dotnet publish -c Release -o "%WORKSPACE%\\publish"'
            }
        }

        //--------------------   automated deployment to Docker Hub  -------------------------

        // Build Docker Image
        stage('Build Docker Image') {
            steps {
                script {
                    docker.build("${DOCKER_IMAGE_NAME}:latest")
                }
            }
        }
		
        // Login to Docker Hub
        stage('Login to Docker Hub') {
            steps {
                script {
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        // login Docker Hub credentials
                    }
                }
            }
        }
        // Push Docker Image to Docker Hub
        stage('Push Docker Image') {
            steps {
				 
                script {
                    docker.withRegistry('https://index.docker.io/v1/', DOCKERHUB_CREDENTIALS) {
                        docker.image("${DOCKER_IMAGE_NAME}:${DOCKER_TAG}").push()
                    }
                }
            }
        }

 // ---------------------  automated deployment to MinIO -------------------------

        stage('Upload to MinIO') {
            steps {
                withCredentials([
                    string(credentialsId: 'minio-access-key', variable: 'MINIO_ACCESS_KEY'),
                    string(credentialsId: 'minio-secret-key', variable: 'MINIO_SECRET_KEY')
                ]) {
                    script {
                        bat 'curl https://dl.min.io/client/mc/release/linux-amd64/mc -o mc && chmod +x mc'
                        bat """./mc alias set minio ${MINIO_ENDPOINT} ${MINIO_ACCESS_KEY} ${MINIO_SECRET_KEY}"""
                        bat """./mc mb minio/${MINIO_BUCKET} || true"""
                        bat """./mc cp --recursive "${WORKSPACE}/publish/" minio/${MINIO_BUCKET}/${JOB_NAME}/${BUILD_NUMBER}/"""
                    }
                }
            }
        }
 // ---------------------  automated deployment to Kubernetes -------------------------
        // stage('Deploy to Kubernetes') {
        //     steps {
        //         withCredentials([file(credentialsId: 'kubeconfig', variable: 'KUBECONFIG')]) {
        //             script {
        //                 bat "sed -i 's|zyond/cicd:latest|zyond/cicd:${BUILD_NUMBER}|g' k8s/deployment.yaml"
        //                 bat "kubectl apply -f k8s/"
        //                 bat "kubectl rollout status deployment/restaurant-app --timeout=300s"
        //             }
        //         }
        //     }
        // }

     //---------------------  automated deployment to IIS -------------------------

        // Copy to IIS Folder
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
        // Ensure IIS Site Exists
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
