pipeline {
    agent any
    stages {
        stage('Clone') {
            steps {
                git branch: 'master', url: 'https://github.com/zyond26/Web_Restaurant_host.git'
            }
        }
        
        stage('Build') {
            steps {
                bat 'dotnet publish --configuration Release --output ./publish'
            }
        }
        
        stage('Deploy to IIS') {
            steps {
                // Dừng Application Pool
                bat 'appcmd stop apppool /apppool.name:"DefaultAppPool"'
                
                // Xóa thư mục cũ
                bat 'if exist "C:\\WebSites\\WebRestaurant" rmdir /s /q "C:\\WebSites\\WebRestaurant"'
                
                // Tạo thư mục mới và copy file
                bat 'mkdir "C:\\WebSites\\WebRestaurant"'
                bat 'xcopy ".\\publish\\*" "C:\\WebSites\\WebRestaurant" /E /Y /I'
                
                // Khởi động lại IIS
                bat 'iisreset /restart'
            }
        }
    }
}