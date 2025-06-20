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
                // Thay đổi: Publish project thay vì solution
                bat 'dotnet publish ./Web_Restaurant.csproj --configuration Release --output ./publish'
            }
        }
        
        stage('Deploy to IIS') {
            steps {
                // Sử dụng đường dẫn đầy đủ đến appcmd
                bat '%windir%\\System32\\inetsrv\\appcmd stop apppool /apppool.name:"DefaultAppPool"'
                
                // Xóa thư mục cũ
                bat 'if exist "C:\\WebSites\\WebRestaurant" rmdir /s /q "C:\\WebSites\\WebRestaurant"'
                
                // Tạo thư mục mới và copy file
                bat 'mkdir "C:\\WebSites\\WebRestaurant"'
                bat 'xcopy ".\\publish\\*" "C:\\WebSites\\WebRestaurant" /E /Y /I'
                
                // Sử dụng đường dẫn đầy đủ đến iisreset
                bat '%windir%\\System32\\iisreset /restart'
            }
        }
    }
}