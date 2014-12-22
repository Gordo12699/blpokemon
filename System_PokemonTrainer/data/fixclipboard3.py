# -*- coding: utf-8 -*-
import pyperclip

data = pyperclip.paste()
print(data.encode('utf-8'))
data = data.replace(u' Leveling\r\n\r\n Level\r\n\t Move\r\n\t Power\r\n\t Accuracy\r\n\t PP\r\n\t Type\r\n\t Cat.\r\n\t Contest Cat.\r\n\t Appeal\r\n\r\n', '')
data = data.replace(u'\r\n Bold indicates this Pokémon receives STAB from this move.Italic indicates an evolved or alternate form of this Pokémon receives STAB from this move.\r\n', '')
datas = data.split(u'\r\n')
set = []
for i in range(len(datas)):
	stri = datas[i][1:]
	print((str(i) + u': ' + stri).encode('utf-8'))
	try:
		if stri[0] != '\t' and stri[0] != ' ':
			print stri[0]
			set.append(stri.replace(u'\r\n', u'') + u' ' + datas[i+1].replace(u'\r\n', u'').replace(u'\t ', u''))
			print set
	except:
		continue

set = ';'.join(set)
print "asdf"
print(set.encode('utf-8'))


pyperclip.copy(set)