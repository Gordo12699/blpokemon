# -*- coding: utf-8 -*-
import pyperclip

data = pyperclip.paste()
print(data.encode('utf-8'))
data = data.replace(u'Leveling Generation IV\r\n\r\nLevel\r\n\tMove\r\n\tPower\r\n\tAccuracy\r\n\tPP\r\n\tType\r\n\t Category\r\n\t Contest Cat.\r\n\t Appeal\r\n\r\n', '')
data = data.replace(u'\r\nBold indicates this Pokémon receives STAB from this move. Italic indicates an evolved or alternate form of this Pokémon receives STAB from this move.\r\n\r\n', '')
datas = data.split(u'\r\n')
set = []
for i in range(len(datas)):
	stri = datas[i]
	print((str(i) + u': ' + stri).encode('utf-8'))
	try:
		if stri[0] != '\t':
			print stri[0]
			set.append(stri.replace(u'\r\n', u'') + u' ' + datas[i+1].replace(u'\r\n', u'').replace(u'\t', u''))
			print set
	except:
		continue

set = ';'.join(set)
print "asdf"
print(set.encode('utf-8'))


pyperclip.copy(set)