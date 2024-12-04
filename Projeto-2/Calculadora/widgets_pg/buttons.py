import math
from typing import TYPE_CHECKING

from PySide6.QtCore import Slot
from PySide6.QtWidgets import QGridLayout, QPushButton

if TYPE_CHECKING:
    from widgets_pg.info import Info
    from widgets_pg.display import Display
    from widgets_pg.window import MainWindow

from componentes.constantes import MEDIUM_FONT
from componentes.utils import isEmpity, isNumorDot, isValidNumber


class Button(QPushButton):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)

        self.configStyle()
        
    def configStyle(self):
        font = self.font()
        font.setPixelSize(MEDIUM_FONT)
        font.setItalic(True)
        self.setFont(font)
        self.setMinimumSize(75, 75)

class ButtonsGrid(QGridLayout):
    def __init__(self, display: 'Display', info: 'Info', window: 'MainWindow', *args, **kwargs):
        super().__init__(*args, **kwargs)
        
        self._gridMask = [
            ['C', '←', '^', '/'],
            ['7', '8', '9', '*'],
            ['4', '5', '6', '-'],
            ['1', '2', '3', '+'],
            ['N', '0', '.', '='],
        ]
        
        self.display = display
        self.info = info
        self._equation = ''
        self._left = None
        self._right = None
        self._op = None
        self.window = window
        self._makeGrid()
        
    @property
    def equation(self):
        return self._equation
    
    @equation.setter
    def equation(self, value):
        self._equation = value
        self.info.setText(value)
        
    def _makeGrid(self):
        
        self.display.eqPressed.connect(self._eq)
        self.display.delPressed.connect(self._backspace)
        self.display.clearPressed.connect(self._clear)
        self.display.inputPressed.connect(self._insertToDisplay)
        self.display.operatorPressed.connect(self._configLeftOp)
        self.display.invertPressed.connect(self._invertNumber)
        
        
        
        for i,  row in enumerate(self._gridMask):
            for j, button_text in enumerate(row):
                btn = Button(button_text)
                if not isNumorDot(button_text) and not isEmpity(button_text):
                    btn.setProperty("cssClass", "specialButton")
                    self._configSpecialButton(btn)
                    
                self.addWidget(btn, i, j)
                slot = self._makeSlot(self._insertToDisplay, button_text )
                self._connectButtonClicked(btn, slot)
                
    
    def _connectButtonClicked(self, button, slot):
        button.clicked.connect(slot) #type: ignore
    
    @ Slot()
    def _makeSlot(self, func, *args, **kwargs):
        @ Slot(bool)
        def realSlot():
            func(*args, **kwargs)
        return realSlot
        
    def _configSpecialButton(self, button):
        text = button.text()
        
        if text == 'C':
            slot = self._makeSlot(self._clear)
            self._connectButtonClicked(button, slot)  
            
        if text == '←':
            self._connectButtonClicked(button, self._backspace)
            
        if text in '+-/*^':
            slot = self._makeSlot(self._configLeftOp, text)
            self._connectButtonClicked(button, slot)  
            
        if text == '=':
            self._connectButtonClicked(button, self._eq)
            
        if text == 'N':
            self._connectButtonClicked(button, self._invertNumber)
            
            
        
              
    @ Slot()          
    def _insertToDisplay(self, text):
        newDisplayValue = self.display.text() + text
        
        if not isValidNumber(newDisplayValue):
            self.display.setFocus()
            return
            
        self.display.insert(text)
        self.display.setFocus()

    @ Slot()
    def _invertNumber(self):
        displayText = self.display.text()
        
        if not isValidNumber(displayText):
            return
        
        number = float(displayText) * -1
        
        if number.is_integer():
            number = int(number)
        
        self.display.setText(str(number))
        self.display.setFocus()
    
    @ Slot()
    def _clear(self):
        self.display.clear()
        self._left = None
        self._right = None
        self._op = None
        self.equation = 'Conta'
        self.display.setFocus()
            
    @ Slot()    
    def _configLeftOp(self, text):
        
        displayText = self.display.text() # Numero _left
        self.display.clear()
        
        if not isValidNumber(displayText) and self._left is None:
            self._showError('Digite Algo.')
            return
        
        if self._left is None:
            self._left = float(displayText)
        
        self._op = text
        self.equation = f'{self._left} {self._op} ??'
        self.display.setFocus()
    
    @ Slot(bool)    
    def _eq(self):
        displayText = self.display.text()
        
        if not isValidNumber(displayText) or self._left is None:
            self._showError('Conta incompleta.')
            return
        
        
        self._right = float(displayText)
        self.equation = f'{self._left} {self._op} {self._right}'
        result = 'undefined'
        try:
            if '^' in self.equation and isinstance(self._left, float):
                result = math.pow(self._left, self._right)
            else:
                result = eval(self.equation)
        except ZeroDivisionError:
            self._showError('Não é possível dividir por zero[0]')
        except OverflowError:
            self._showError('Conta com um número Muito grande')
            
        self.display.clear()
        self.info.setText(f'{self.equation} = {result}')
        self._left = result
        self._right = None
        self.display.setFocus()
        
        if result == 'undefined':
                self._left = None
    
    
    @ Slot()
    def _backspace(self):
        self.display.backspace()
        self.display.setFocus()
    
                
    def _showError(self, text):
        msgBox = self.window.makeMsgBox()
        msgBox.setText(text)
        msgBox.setIcon(msgBox.Icon.Critical)
        msgBox.exec()
        self.display.setFocus()
        
    def _showInfo(self, text):
        msgBox = self.window.makeMsgBox()
        msgBox.setText(text)
        msgBox.setIcon(msgBox.Icon.Information)
        msgBox.exec()
        self.display.setFocus()