
from PySide6.QtCore import Slot
from PySide6.QtGui import QIcon
from PySide6.QtWidgets import QMainWindow, QMessageBox, QVBoxLayout, QWidget

from componentes.constantes import PNG_ICON
from widgets_pg.display import Display
from widgets_pg.info import Info


class MainWindow(QMainWindow):
    def __init__(self, parent: QWidget | None = None, *args, **kwargs):
        super().__init__(parent, *args, **kwargs)
        
        #Gerando Janela
        self.setWindowTitle('Calculadora')
        icon = QIcon(str(PNG_ICON))
        self.setWindowIcon(icon)
        self.cw = QWidget()
        self.setCentralWidget(self.cw)
        self.vlayout = QVBoxLayout()
        self.cw.setLayout(self.vlayout)
        #Gerando Janela
        
        # Info - Label
        self.info = Info('Conta')
        
        # Display - user input
        self.display = Display()
        self.addWidgetL(self.info)
        self.addWidgetL(self.display)
        
        
    
    def fixedSize(self):  
        #Ultima coisa a ser executada
        self.adjustSize()
        self.setFixedSize(self.width(), self.height())

    def addWidgetL(self, widget: QWidget):
        #Adiciona Widget ao Layout
        self.vlayout.addWidget(widget)

    def makeMsgBox(self):
        return QMessageBox(self)