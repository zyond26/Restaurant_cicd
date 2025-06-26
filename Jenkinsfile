pipeline {
    agent any

    stages {
        stage('Clean Workspace') {
            steps {
                cleanWs()
                bat 'if exist "c:\\wwwroot\\WebRestaurant" rmdir /s /q "c:\\wwwroot\\WebRestaurant"'
            }
        }

        stage('Setup .NET') {
            steps {
                bat 'dotnet --version'
                // Thêm cài đặt workload nếu cần
                bat 'dotnet workload restore'
            }
        }

        stage('Build') {
            steps {
                bat '''
                dotnet clean
                dotnet restore
                dotnet build -c Release -p:UseAppHost=false
                '''
            }
        }

        stage('Publish') {
            steps {
                bat '''
                dotnet publish -c Release -o ./publish --runtime win-x64 --self-contained false /p:UseAppHost=false
                '''
            }
        }

        stage('Deploy') {
            steps {
                script {
                    // Dừng IIS tạm thời
                    bat 'net stop was /y'
                    
                    // Xóa thư mục cũ nếu tồn tại
                    bat 'if exist "c:\\wwwroot\\WebRestaurant" rmdir /s /q "c:\\wwwroot\\WebRestaurant"'
                    
                    // Copy file mới
                    bat 'xcopy ".\\publish" "c:\\wwwroot\\WebRestaurant" /E /Y /I /Q'
                    
                    // Khởi động lại IIS
                    bat 'net start w3svc'
                }

                powershell '''
                $siteName = "MySite"
                $appPool = "MyAppPool"
                
                Import-Module WebAdministration
                
                # Tạo AppPool nếu chưa có
                if (-not (Test-Path "IIS:\\AppPools\\$appPool")) {
                    New-WebAppPool -Name $appPool
                    Set-ItemProperty "IIS:\\AppPools\\$appPool" -Name managedRuntimeVersion -Value ""
                    Set-ItemProperty "IIS:\\AppPools\\$appPool" -Name processModel.identityType -Value 4
                }
                
                # Tạo/update website
                if (-not (Test-Path "IIS:\\Sites\\$siteName")) {
                    New-Website -Name $siteName -Port 26 -PhysicalPath "c:\\wwwroot\\WebRestaurant" -ApplicationPool $appPool
                } else {
                    Set-ItemProperty "IIS:\\Sites\\$siteName" -Name physicalPath -Value "c:\\wwwroot\\WebRestaurant"
                    Set-ItemProperty "IIS:\\Sites\\$siteName" -Name applicationPool -Value $appPool
                }
                
                # Cấp quyền
                icacls "c:\\wwwroot\\WebRestaurant" /grant "IIS AppPool\\$appPool:(OI)(CI)(RX)"
                '''
            }
        }
    }
}