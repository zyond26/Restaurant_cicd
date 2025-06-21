pipeline {
    agent any
    environment {
        RENDER_API_KEY = credentials('render-api-key') // ID của Render API Key
        SERVICE_ID = 'srv-d1bd0hqdbo4c73cdeif0' // Thay bằng Service ID của bạn
        DOTNET_CLI_HOME = "/tmp/dotnet"
    }
    stages {
        stage('Clone') {
            steps {
                git branch: 'master', url: 'https://github.com/zyond26/Web_Restaurant_host.git'
            }
        }
        stage('Restore') {
            steps {
                sh 'dotnet restore src/Web_Restaurant.csproj'
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build src/Web_Restaurant.csproj -c Release --no-restore'
            }
        }
        stage('Test') {
            steps {
                echo 'Chạy unit tests (thêm lệnh test nếu có)'
                // Nếu có project test, thêm: sh 'dotnet test tests/YourTestProject.csproj'
            }
        }
        stage('Deploy to Render') {
            steps {
                script {
                    def payload = '''{
                        "clearCache": true
                    }'''
                    sh """
                        curl -X POST \
                        -H "Authorization: Bearer ${RENDER_API_KEY}" \
                        -H "Content-Type: application/json" \
                        -d '${payload}' \
                        https://api.render.com/v1/services/${SERVICE_ID}/deploys
                    """
                }
            }
        }
    }
       stage ('Publish') {
		steps {
			echo 'public 2 runnig folder'
		//iisreset /stop // stop iis de ghi de file 
			bat 'xcopy "%WORKSPACE%" /E /Y /I /R "c:\\OrderRestaurant"'
 		} 
        }
     post {
        always {
            echo 'Pipeline hoàn thành!'
        }
        success {
            echo 'Triển khai thành công lên Render!'
        }
        failure {
            echo 'Pipeline thất bại. Kiểm tra log để biết chi tiết.'
        }
    }
}