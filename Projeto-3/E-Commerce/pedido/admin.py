from django.contrib import admin
from .models import ItemPedido, Pedido

class ItemPedidoInLine(admin.TabularInline):
    model = ItemPedido
    extra = 1
    
class PedidoAdmin(admin.ModelAdmin):
    inlines = [
        ItemPedidoInLine,
    ]

admin.site.register(ItemPedido)
admin.site.register(Pedido, PedidoAdmin)

