pipeline {
    agent any
    stages {
        stage('Clone') {
            steps {
                git branch: 'master', url: 'https://github.com/zyond26/Web_Restaurant_host.git'
            }
        }
        
       stage ('Publish') {
		steps {
			echo 'public 2 runnig folder'
		//iisreset /stop // stop iis de ghi de file 
			bat 'xcopy "%WORKSPACE%" /E /Y /I /R "c:\\OrderRestaurant"'
 		} 
        }
    }
}