import { useTranslation } from 'react-i18next';

import { OfficeSelector } from '../OfficeSelector';

import {
	BookingsHeaderRightMenuWrapper,
	BookingsHeaderWrapper,
	BookingsTitle,
} from './styled';

export function BookingBlockHeader() {
	const { t } = useTranslation('booking');

	return (
		<BookingsHeaderWrapper>
			<BookingsTitle variant="small">{t('title')}</BookingsTitle>
			<BookingsHeaderRightMenuWrapper>
				<OfficeSelector />
			</BookingsHeaderRightMenuWrapper>
		</BookingsHeaderWrapper>
	);
}
