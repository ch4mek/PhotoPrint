import { TFunction } from 'i18next';

export const getWeeksWord = (
	value: number,
	t: TFunction<'booking', 'form.repetitive'>
) => t('week', { count: value });
