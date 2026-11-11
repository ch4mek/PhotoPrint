const DATE_LOCALE_EN = 'en-US';
const DATE_LOCALE_RU = 'ru-RU';

export const getDateLocale = (language: string) =>
	language === 'ru' ? DATE_LOCALE_RU : DATE_LOCALE_EN;
