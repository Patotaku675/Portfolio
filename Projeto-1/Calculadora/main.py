
import sys

from PySide6.QtWidgets import QApplication

from componentes.styles import setupTheme
from widgets_pg.buttons import Button, ButtonsGrid
from widgets_pg.window import MainWindow

if __name__ == '__main__':
    # Criando Aplicação
    app = QApplication(sys.argv)
    setupTheme(app)
    window = MainWindow()
    
    #Layout dos botoes
    buttonsGrid = ButtonsGrid(window.display, window.info, window)
    window.vlayout.addLayout(buttonsGrid)

    
    # Executando Aplicação
    window.fixedSize()
    window.show()
    print('Executando')
    app.exec()