import { memo } from 'react';

import { ProtectedView } from '@/components/ProtectedView';
import { managerRolesWithGroupManager } from '@/constants/roles';
import { useBookingContext } from '@/context/Booking';

import { BookingCalendar } from './components/BookingCalendar';
import { BookingControls } from './components/BookingControls';
import { BookingGuestNameField } from './components/BookingGuestNameField';
import { BookingModeControls } from './components/BookingModeControls';
import { DatePickers } from './components/DatePickers';
import { TimeSeletors } from './components/TimeSelectors';
import { BookingUsersAutoselect } from './components/UsersAutoselect';
import { BookingFormWrapper } from './styled';

function BookingFormInner() {
	const { areMeetingRoomsSelected, isGuestMode } = useBookingContext();

	const isShowGuestNameField = !areMeetingRoomsSelected && isGuestMode;

	return (
		<BookingFormWrapper>
			<BookingCalendar />
			<ProtectedView roles={managerRolesWithGroupManager}>
				<>
					{!areMeetingRoomsSelected && <BookingModeControls />}

					{isShowGuestNameField ? (
						<BookingGuestNameField />
					) : (
						<BookingUsersAutoselect />
					)}
				</>
			</ProtectedView>
			<DatePickers />
			{areMeetingRoomsSelected && <TimeSeletors />}
			<BookingControls />
		</BookingFormWrapper>
	);
}

export const BookingForm = memo(BookingFormInner);
