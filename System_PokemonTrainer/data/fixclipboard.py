# -*- coding: utf-8 -*-
import pyperclip

data = pyperclip.paste()
print(data.encode('utf-8'))
data = data.replace(u'TM and HM Generation IV\r\n\r\nMove\r\n\r\n', '')
data = data.replace(u'\r\n\r\n\r\nBold indicates this Pokémon receives STAB from this move. Italic indicates an evolved or alternate form of this Pokémon receives STAB from this move.\r\n\r\n', '')
data = data.replace(u'\r\n\r\n', ';')
print(data.encode('utf-8'))


pyperclip.copy(data)