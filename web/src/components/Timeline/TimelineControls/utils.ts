import { TFunction } from 'i18next';

export const getTimezoneLabel = (
	timezoneOffset: string,
	timezoneName: string,
	t: TFunction<'booking', 'timeline'>
) => `${t('timezone')}: ${timezoneOffset} (${timezoneName})`;
