import csv, json

def convertTypes(file):
	output = {'Types' : {}}
	with open(file) as csv_file:
		for row in csv.DictReader(csv_file):
			output['Types'].update({row['Type']: {
					'Effective': row['Effective'],
					'Ineffective': row['Ineffective'],
					'Nullified': row['Nullified'],
					'Weak': row['Weak'],
					'Resistant': row['Resistant'],
					'Immune': row['Immune']
				}})
	return output

def convertMoves(file):
	output = {'Moves' : {}}
	with open(file) as csv_file:
		for row in csv.DictReader(csv_file, delimiter='|'):
			output['Moves'].update({row['Name']: {
					'ID': row['MoveIndex'],
					'TM': row['TMIndex'],
					'HM': row['HMIndex'],
					'Type': row['Type'],
					'Category': row['Category'],
					'Power': row['Power'],
					'Accuracy': row['Accuracy'],
					'PP': row['PP'],
					'PPMax': row['PPMax'],
					'Effect': row['Effect'],
					'Contact': row['MakesContact'],
					'Description': row['Description']
				}})
	return output

def convertMoves2(file):
	output = {'Moves' : {}}
	with open(file) as csv_file:
		for row in csv.DictReader(csv_file, delimiter='|'):
			output['Moves'].update({row['Name']: {
					'Type': row['Type'],
					'Class': row['Class'],
					'Power': row['Power'],
					'Accuracy': row['Accuracy'],
					'PP': row['PP'],
					'Effect': row['Effect'],
					'Contact': row['Contact'],
					'Description': row['Description']
				}})
	return output

def convertPokemon(file):
	output = {'Pokemon' : {}}
	with open(file) as csv_file:
		for row in csv.DictReader(csv_file, delimiter='|'):
			content = {'NationalPokedexNumber': row['NationalPokedexNumber'],
					'Type1': row['Type1'].upper(),
					'Type2': row['Type2'].upper(),
					'Height': row['Height'],
					'Weight': row['Weight'],
					'Description': row['Description'],
					'EvolveLevel': row['EvolveLevel'],
					'EvolveInto': row['EvolveInto'],
					'EvolveStone': row['EvolveStone'],
					'CatchRate': row['CatchRate'],
					'BaseExp': row['BaseExp'],
					'ExperienceGroup': row['ExperienceGroup'],
					'BaseHP': row['BaseHP'],
					'BaseAttack': row['BaseAttack'],
					'BaseDefence': row['BaseDefence'],
					'BaseSpAttack': row['BaseSpAttack'],
					'BaseSpDefence': row['BaseSpDefence'],
					'BaseSpeed': row['BaseSpeed'],
					'EVGainHP': row['EvGainHP'],
					'EVGainAtk': row['EvGainAtk'],
					'EVGainDef': row['EvGainDef'],
					'EVGainSpAtk': row['EvGainSpAtk'],
					'EVGainSpDef': row['EvGainSpDef'],
					'EVGainSpeed': row['EvGainSpeed']
				}
			moves = {}
			for move in row['Moves'].split(';'):
				if move.strip() == '':
					continue
				data = move.split(':')
				if len(data) < 2:
					continue
				moves.update({data[0] : data[1].replace('(s)', '')})
			content['Moves'] = moves

			learnset = []
			for learn in row['CanLearn'].split(';'):
				if learn.strip() == '':
					continue
				learnset.append(learn.replace('(s)', ''))
			content['Learnset'] = learnset

			add = {row['Name'] : content}

			output['Pokemon'].update(add)
			print row['Name']
	return output

def convertPokemon2(file):
	output = {'Pokemon' : {}}
	with open(file) as csv_file:
		for row in csv.DictReader(csv_file, delimiter='|'):
			content = {'NationalPokedexNumber': row['NationalPokedexNumber'],
					'Type': row['Type'].upper(),
					'Ability': row['Ability'].split(';'),
					'HiddenAbility': row['HiddenAbility'],
					'Height': row['Height'],
					'Weight': row['Weight'],
					'MalePerc': row['MalePerc'],
					'Description': row['Description'],
					'EvolveLevel': row['EvolveLevel'],
					'EvolveInto': row['EvolvesInto'],
					'EvolveCondition': row['EvolveCondition'],
					'CatchRate': row['CatchRate'],
					'BaseExp': row['BaseExp'],
					'ExperienceGroup': row['ExperienceGroup'],
					'BaseHP': row['BaseHP'],
					'BaseAtk': row['BaseAtk'],
					'BaseDef': row['BaseDef'],
					'BaseSpAtk': row['BaseSpAtk'],
					'BaseSpDef': row['BaseSpDef'],
					'BaseSpeed': row['BaseSpeed'],
					'EVGainHP': row['EvGainHP'],
					'EVGainAtk': row['EvGainAtk'],
					'EVGainDef': row['EvGainDef'],
					'EVGainSpAtk': row['EvGainSpAtk'],
					'EVGainSpDef': row['EvGainSpDef'],
					'EVGainSpeed': row['EvGainSpeed']
				}
			moves = []
			for move in row['Moves'].split(';'):
				if move.strip() == '':
					continue
				moves.append(move)
			content['Moves'] = moves

			learnset = []
			for learn in row['CanLearn'].split(';'):
				if learn.strip() == '':
					continue
				learnset.append(learn)
			content['Learnset'] = learnset

			add = {row['Name'] : content}

			output['Pokemon'].update(add)
			print row['Name']
	return output

# out = {}
# out.update(convertTypes("./types.csv"))
# with open("./GenII-VTypes.json", 'w') as f:
# 	json.dump(out, f, sort_keys=True, indent=4)

# out = {}
# out.update(convertPokemon2("./gen1revision.csv"))
# with open("./Gen1Revision.json", 'w') as f:
# 	json.dump(out, f, sort_keys=True, indent=4)

out = {}
out.update(convertMoves2("./MovesNew.csv"))
with open("./MovesNew.json", 'w') as f:
	json.dump(out, f, sort_keys=True, indent=4)