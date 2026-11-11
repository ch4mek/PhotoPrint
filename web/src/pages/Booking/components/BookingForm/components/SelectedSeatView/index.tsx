import { Typography } from '@mms/mms-ui-library/components/Typography';
import { MapPinIcon } from '@mms/mms-ui-library/icons/base/MapPinIcon';
import { useTranslation } from 'react-i18next';

import { SelectedBookingItem, SelectedBookingItemType } from '@/context/types';
import { getDeskName } from '@/pages/Booking/utils';

import { SelectedSeatViewWrapper } from './styled';

interface SelectedSeatViewProps {
	item: SelectedBookingItem;
}

export function SelectedItemView({ item }: SelectedSeatViewProps) {
	const { t } = useTranslation('booking');

	return (
		<SelectedSeatViewWrapper>
			<MapPinIcon />
			<span>
				<Typography variant="m-600">
					{`${
						item.type === SelectedBookingItemType.Desk
							? t('labels.desk')
							: t('labels.room')
					}: `}
				</Typography>
				<Typography variant="m-400">
					{item.type === SelectedBookingItemType.Desk
						? getDeskName(item.abbreviation, item.displayNumber)
						: item.name}
				</Typography>
			</span>
		</SelectedSeatViewWrapper>
	);
}
