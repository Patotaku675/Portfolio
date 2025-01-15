from django.contrib import admin
from .models import Perfil


# class VariacaoInline(admin.TabularInline):
#     model = Variacao
#     extra = 1
    
# class ProdutoAdmin(admin.ModelAdmin):
#     inlines = [
#         VariacaoInline,
#     ]


admin.site.register(Perfil)
