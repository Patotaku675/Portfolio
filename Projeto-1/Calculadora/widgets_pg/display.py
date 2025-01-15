from PySide6.QtCore import Qt, Signal
from PySide6.QtGui import QKeyEvent
from PySide6.QtWidgets import QLineEdit

from componentes.constantes import BIG_FONT, MINIMUM_WIDTH, TEXT_MARGIN
from componentes.utils import isEmpity, isNumorDot


class Display(QLineEdit):
    eqPressed = Signal()
    delPressed = Signal()
    clearPressed = Signal()
    inputPressed = Signal(str)
    operatorPressed = Signal(str)
    invertPressed = Signal()
    
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.configStyle()
        
    def configStyle(self):
        margins = [TEXT_MARGIN for _ in range(4)]
        self.setStyleSheet(f'font-size: {BIG_FONT}px;')
        self.setMinimumHeight(BIG_FONT * 2)
        self.setMinimumWidth(MINIMUM_WIDTH)
        self.setAlignment(Qt.AlignmentFlag.AlignRight)
        self.setTextMargins(*margins)
    
        
    def keyPressEvent(self, event: QKeyEvent) -> None:  # type: ignore
        text = event.text().strip()
        key = event.key()
        KEYS = Qt.Key
        
        isEnter = key in [KEYS.Key_Enter, KEYS.Key_Return]
        isDelete = key in [KEYS.Key_Backspace, KEYS.Key_Delete]
        isEsc = key in [KEYS.Key_Escape, KEYS.Key_C]
        isInverter = key in [KEYS.Key_N]
        isOperator = key in [
            KEYS.Key_Plus, 
            KEYS.Key_Minus,
            KEYS.Key_Slash,
            KEYS.Key_Asterisk,
            KEYS.Key_P
            ]
        
        
        
        if isEnter:
            self.eqPressed.emit()
            return event.ignore()
        
        if isDelete:
            self.delPressed.emit()
            return event.ignore()
        
        if isEsc:
            self.clearPressed.emit()
            return event.ignore()
        
        if isOperator:
            if text.lower() == 'p':
                text = '^'
            self.operatorPressed.emit(text)
            return event.ignore()
        
        if isEmpity(text):
            return event.ignore()
        
        if isNumorDot(text):
            self.inputPressed.emit(text)
            return event.ignore()
        
        if isInverter:
            self.invertPressed.emit()
            return event.ignore()
            
        
        
        