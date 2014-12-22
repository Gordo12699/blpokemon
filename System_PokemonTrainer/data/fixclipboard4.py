# -*- coding: utf-8 -*-
import pyperclip

data = pyperclip.paste()
print(data.encode('utf-8'))
data = data.replace(u' TMs/HMs\r\n\r\n Move\r\n\r\n ', '')
data = data.replace(u'\r\n\r\n Bold indicates this Pokémon receives STAB from this move.Italic indicates an evolved or alternate form of this Pokémon receives STAB from this move.\r\n', '')
data = data.replace(u'\r\n\r\n ', ';')
print(data.encode('utf-8'))


pyperclip.copy(data)