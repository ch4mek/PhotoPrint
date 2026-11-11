// TODO: jsx should'n be in @/helpers

import { TFunction } from 'i18next';

import { TextWithStrongInside } from '@/components/TextWithStrongInside';
import { TemporaryGetDialogTextComponentParams } from '@/types/helpers';

import { getFullName } from './formatName';

export const getDialogTextComponent = (
	{
		isBlock,
		userId,
		firstName,
		lastName,
		isForGuest,
		guestName,
	}: TemporaryGetDialogTextComponentParams,
	currentUserId: number,
	t: TFunction<'booking'>
) => {
	if (isBlock && !isForGuest) {
		return null;
	}

	const bookingName =
		isForGuest && guestName ? guestName : getFullName(firstName, lastName);

	return userId !== currentUserId || isForGuest ? (
		<TextWithStrongInside
			before={t('dialogs.cancel.confirmOtherBefore')}
			strong={bookingName}
			after={t('dialogs.cancel.confirmOtherAfter')}
		/>
	) : null;
};
