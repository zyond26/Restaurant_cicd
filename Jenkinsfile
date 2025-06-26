   pipeline {
    agent any
    environment {
        RENDER_API_KEY = credentials('render-api-key') // ID của Render API Key
        SERVICE_ID = 'srv-d1bd0hqdbo4c73cdeif0' // Service ID của bạn
    }
    stages {
        stage('Restore') {
            steps {
                bat 'dotnet restore Web_Restaurant.csproj'
            }
        }
        stage('Build') {
            steps {
                bat 'dotnet build Web_Restaurant.csproj -c Release --no-restore'
            }
        }
        stage('Test') {
            steps {
                echo 'Chạy unit tests (thêm lệnh test nếu có)'
                // Nếu có project test: bat 'dotnet test tests\\YourTestProject.csproj'
            }
        }
        stage('Deploy to Render') {
            steps {
                script {
                    def payload = '''{
                        "clearCache": true
                    }'''
                    bat """
                        curl -X POST ^
                        -H "Authorization: Bearer %RENDER_API_KEY%" ^
                        -H "Content-Type: application/json" ^
                        -d "%payload%" ^
                        https://api.render.com/v1/services/%SERVICE_ID%/deploys
                    """
                }
            }
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
}//end pipeline
   