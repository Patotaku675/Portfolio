// Arquivo de build de nível superior onde você pode adicionar opções de configuração comuns a todos os subprojetos/módulos.
plugins {
    alias(libs.plugins.android.application) apply false  // Configuração do plugin Android para aplicação. Define que o plugin será usado, mas não aplicado diretamente aqui.
    // Caso você precise de mais plugins, você pode adicioná-los aqui na lista.
}

buildscript {
    repositories {
        google()  // Repositório necessário para baixar dependências do Android, como o plugin Android e o Android SDK.
        gradlePluginPortal()  // Repositório para o Gradle, usado para plugins de build e configuração.
        mavenCentral()  // Repositório necessário para outras dependências de bibliotecas, como Glide, ExoPlayer, etc.
    }
    dependencies {
        classpath("com.android.tools.build:gradle:7.4.1")  // Define a versão do plugin do Android. Certifique-se de atualizar para a versão mais recente, conforme o seu projeto.
        // Adicione outras dependências de classe necessárias, como plugins adicionais, se necessário.
    }
}
