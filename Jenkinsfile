pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git branch: 'master', url: 'https://github.com/zyond26/Web_Restaurant_host.git'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet restore'
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release --output "C:\\publish\\WebRestaurant"'
            }
        }

        stage('Deploy to IIS') {
            steps {
                bat '''
                
                # Copy files publish sang thư mục IIS
                robocopy "C:\\publish\\WebRestaurant" "C:\\inetpub\\wwwroot\\WebRestaurant" /MIR

                # Start app pool
                Start-WebAppPool -Name "WebRestaurantAppPool"
                '''
            }
        }
    }
}