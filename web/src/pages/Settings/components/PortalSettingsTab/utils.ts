import { splitValueByCapitalLetters } from '../../utils';

export const splitSettingsName = (settingsName: string) =>
	settingsName
		.split('.')
		.map((settingsKeyValue) => splitValueByCapitalLetters(settingsKeyValue))
		.join(': ');
