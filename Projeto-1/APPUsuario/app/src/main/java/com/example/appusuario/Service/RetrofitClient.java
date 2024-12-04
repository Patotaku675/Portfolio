package com.example.appusuario.Service;

import android.os.Build;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class RetrofitClient {
    // URL do servidor para emulador
    private static final String EMULATOR_BASE_URL = "http://10.0.2.2:5245/api/";
    // Instância do Retrofit
    private static Retrofit retrofit;

    /**
     * Método que determina a URL base dependendo se está rodando em um dispositivo físico ou emulador
     * @return A URL base correta (emulador ou IP local)
     */
    private static String getBaseUrl() {
        // Verifica se está rodando em um emulador
        if (Build.FINGERPRINT.contains("generic")) {
            return EMULATOR_BASE_URL;  // Retorna a URL do emulador
        } else {
            // Retorna o IP local da máquina para dispositivos físicos
            return "http://192.168.15.6:5245/api/";
        }
    }

    /**
     * Método que configura e retorna a instância do Retrofit
     * @return Instância do Retrofit configurada
     */
    public static Retrofit getRetrofitInstance() {
        if (retrofit == null) {
            // Configura o interceptor para logar as requisições e respostas
            HttpLoggingInterceptor loggingInterceptor = new HttpLoggingInterceptor();
            loggingInterceptor.setLevel(HttpLoggingInterceptor.Level.BODY);  // Configura para logar o corpo da requisição/resposta

            // Criação de um cliente OkHttp com interceptors para logging e modificação de requisições DELETE
            OkHttpClient client = new OkHttpClient.Builder()
                    .addInterceptor(loggingInterceptor)  // Adiciona o interceptor de logging
                    .addInterceptor(chain -> {
                        Request original = chain.request();

                        // Modifica a requisição se for do tipo DELETE para adicionar corpo
                        if (original.method().equals("DELETE")) {
                            Request newRequest = original.newBuilder()
                                    .method("DELETE", original.body()) // Mantém o corpo na requisição DELETE
                                    .build();
                            return chain.proceed(newRequest);  // Envia a requisição modificada
                        }

                        // Para outros métodos, apenas passa a requisição original
                        return chain.proceed(original);
                    })
                    .build();

            // Cria a instância do Retrofit com a URL base e o cliente OkHttp configurado
            retrofit = new Retrofit.Builder()
                    .baseUrl(getBaseUrl())  // Define a URL base, dependendo do dispositivo
                    .client(client)          // Configura o cliente OkHttp com interceptors
                    .addConverterFactory(GsonConverterFactory.create())  // Usando o Gson para converter JSON
                    .build();
        }
        return retrofit;
    }

    /**
     * Método para obter a instância da interface ApiService
     * @return Instância da ApiService configurada
     */
    public static ApiService getApiService() {
        return getRetrofitInstance().create(ApiService.class);
    }
}
