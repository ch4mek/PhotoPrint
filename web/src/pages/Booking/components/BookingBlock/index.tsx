import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { bookingBlockTabsKeys } from '../../constants';
import { extractHighlightedDaysFromResponse } from '../../utils';

import { TabPanel } from '@/components/TabPanel';
import { useAuth } from '@/context/AuthContext';
import { useBookingContext } from '@/context/Booking';
import { BookingList, BookingManagement } from '@/pages/Booking/components';

import { BookingBlockHeader } from './components/BookingBlockHeader';
import {
	BookingsBlockContent,
	BookingsBody,
	BookingBlockWrapper,
} from './styled';

export function BookingBlock() {
	const [activeTabIndex, setActiveTabIndex] = useState(0);

	const { id } = useAuth();
	const { setHighlightedDays, office, bookingsState, isBookingsDataLoading } =
		useBookingContext();
	const { timeZone } = office;
	const { t } = useTranslation('booking');

	useEffect(() => {
		if (bookingsState.desks && bookingsState.rooms) {
			setHighlightedDays(
				extractHighlightedDaysFromResponse(
					bookingsState.desks,
					bookingsState.rooms,
					id,
					timeZone
				)
			);
		}
	}, [bookingsState, id, timeZone]);

	const bookingBlockTabs = useMemo(
		() =>
			bookingBlockTabsKeys.map((tab) => ({
				...tab,
				text: t(tab.text),
			})),
		[t]
	);

	return (
		<BookingBlockWrapper>
			<BookingBlockHeader />
			<BookingsBlockContent>
				<BookingsBody>
					<Tabs
						activeTab={activeTabIndex}
						onChangeTab={setActiveTabIndex}
						data={bookingBlockTabs}
					/>
					<TabPanel value={activeTabIndex} index={0}>
						<BookingManagement />
					</TabPanel>
					<TabPanel value={activeTabIndex} index={1}>
						<BookingList
							showLoader={isBookingsDataLoading}
							deskData={bookingsState.desks}
							roomsData={bookingsState.rooms}
						/>
					</TabPanel>
				</BookingsBody>
			</BookingsBlockContent>
		</BookingBlockWrapper>
	);
}
