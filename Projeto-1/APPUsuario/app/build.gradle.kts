plugins {
    alias(libs.plugins.android.application)  // Aplica o plugin Android para criar o aplicativo
}

android {
    namespace = "com.example.appusuario"  // Define o namespace do aplicativo, utilizado para o pacote do app
    compileSdk = 34  // Define a versão do SDK de compilação para o projeto

    defaultConfig {
        applicationId = "com.example.appusuario"  // ID único do aplicativo
        minSdk = 21  // Define a versão mínima do SDK que o aplicativo suporta
        targetSdk = 34  // Define a versão do SDK alvo para o aplicativo
        versionCode = 1  // Código da versão do aplicativo, utilizado em atualizações
        versionName = "1.0"  // Nome da versão do aplicativo, visível para o usuário

        testInstrumentationRunner = "androidx.test.runner.AndroidJUnitRunner"  // Define o runner de testes para o projeto
    }

    signingConfigs {
        // Configuração de assinatura para versão release
        create("release") {
            storeFile = file("E:/0_SODTMT/Projetos/Projeto_1-API-MAUI-JAVA/JavaAPP/APPUsuario/my-local-key.jks")  // Caminho da keystore
            storePassword = "123456"  // Senha da keystore
            keyAlias = "local-key"  // Alias da chave usada para assinatura
            keyPassword = "123456"  // Senha da chave
        }
    }

    buildTypes {
        getByName("release") {
            isMinifyEnabled = false  // Desativa a minificação de código para a versão release
            isShrinkResources = false  // Desativa a remoção de recursos não utilizados na versão release
            proguardFiles(
                getDefaultProguardFile("proguard-android-optimize.txt"),  // Configuração padrão do ProGuard
                "proguard-rules.pro"  // Arquivo adicional de regras do ProGuard
            )
            signingConfig = signingConfigs.getByName("release")  // Configura a chave de assinatura para a versão release
        }
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_11  // Define a versão do Java utilizada no código-fonte
        targetCompatibility = JavaVersion.VERSION_11  // Define a versão do Java para o bytecode gerado
    }

}

dependencies {
    // Retrofit para integração com APIs
    implementation("com.squareup.retrofit2:retrofit:2.9.0")  // Biblioteca principal do Retrofit
    implementation("com.squareup.retrofit2:converter-gson:2.9.0")  // Conversor Gson para o Retrofit, utilizado para converter JSON em objetos Java

    // Glide para carregamento de imagens
    implementation("com.github.bumptech.glide:glide:4.15.1")  // Biblioteca para carregamento eficiente de imagens
    annotationProcessor("com.github.bumptech.glide:compiler:4.15.1")  // Processador de anotações do Glide

    // OkHttp para gerenciamento de requisições HTTP
    implementation("com.squareup.okhttp3:logging-interceptor:4.10.0")  // Interceptor para log de requisições HTTP
    implementation("com.squareup.okhttp3:okhttp:4.10.0")  // Biblioteca principal do OkHttp para requisições HTTP

    // Dependências do Media3 (para reprodução de mídia)
    implementation("androidx.media3:media3-exoplayer:1.0.0")  // Biblioteca para exibição e controle de vídeos
    implementation("androidx.media3:media3-session:1.0.0")  // Biblioteca para controle de sessões de mídia

    // Dependência para a interface de usuário do Media3
    implementation("androidx.media3:media3-ui:1.0.0")  // Biblioteca para a interface de usuário do Media3

    // Outras dependências para compatibilidade com UI
    implementation(libs.appcompat)  // Biblioteca para compatibilidade com versões anteriores do Android
    implementation(libs.material)  // Biblioteca de componentes de Material Design
    implementation(libs.activity)  // Biblioteca para atividades (Activity) no Android
    implementation(libs.constraintlayout)  // Layout baseado em restrições, útil para layouts complexos

    // Dependências de testes
    testImplementation(libs.junit)  // JUnit para testes unitários
    androidTestImplementation(libs.ext.junit)  // JUnit para testes Android
    androidTestImplementation(libs.espresso.core)  // Espresso para testes de UI no Android
}

repositories {
    google()  // Repositório do Google, necessário para dependências do Android
    mavenCentral()  // Repositório central para outras dependências
}
