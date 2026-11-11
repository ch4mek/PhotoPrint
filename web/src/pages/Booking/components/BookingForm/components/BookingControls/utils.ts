import { TFunction } from 'i18next';

export const getBookingItemName = (
	areMeetingRoomsSelected: boolean,
	t: TFunction<'booking'>
) =>
	t('actions.book', {
		resource: t(
			areMeetingRoomsSelected
				? 'labels.roomAccusative'
				: 'labels.deskAccusative'
		),
	});
