import { useTheme } from '@emotion/react';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { CalendarIcon } from '@mms/mms-ui-library/icons/base/CalendarIcon';
import { ClockIcon } from '@mms/mms-ui-library/icons/base/ClockIcon';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { MapPinIcon } from '@mms/mms-ui-library/icons/base/MapPinIcon';
import { memo, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { BOOKING_LIST_TYPOGRAPHY_CLASS } from '../constants';
import { getRoomBookingEdgeDates } from '../utils';

import { createPeriodInText } from '@/helpers/booking';
import { getFormattedTimePeriod } from '@/helpers/bookingFormatters';
import { getFullName } from '@/helpers/formatName';
import {
	BookingListItem,
	BookingListTypography,
	BookingUserName,
	Divider,
	IconCloseWrapper,
	IconWrapper,
	ItemRowWithIcon,
	ListItemContent,
} from '@/pages/Booking/components/BookingList/styles';
import { getMeetingRoomDateList } from '@/pages/Booking/utils';

import type { RoomBookingListItemProps } from './types';

function ItemInner(props: RoomBookingListItemProps) {
	const {
		weeksInInterval,
		handleDelete,
		userFirstName,
		userLastName,
		reservationFromOfficeTime,
		reservationToOfficeTime,
		meetingRoomName,
		isRepetitiveBooking,
		reservations,
		handleClickName,
		userId,
		isNameClickable,
	} = props;
	const { t } = useTranslation('common');

	const theme = useTheme();

	const bookingTime = useMemo(
		() =>
			getFormattedTimePeriod(
				reservationFromOfficeTime,
				reservationToOfficeTime,
				true,
				t
			),
		[reservationFromOfficeTime, reservationToOfficeTime]
	);

	const bookingDateList = useMemo(() => {
		const { firstDate, lastDate } = getRoomBookingEdgeDates(reservations);
		const info = getMeetingRoomDateList(
			firstDate,
			lastDate,
			isRepetitiveBooking
		);

		return info;
	}, [reservations, isRepetitiveBooking]);

	return (
		<>
			<BookingListItem type="meetingRoom" isNeedUnderline={isNameClickable}>
				<ListItemContent>
					{userFirstName && userLastName && (
						<BookingListTypography
							Component="p"
							variant="m-600"
							color={theme.palette['text-black']}
						>
							<BookingUserName
								className={BOOKING_LIST_TYPOGRAPHY_CLASS}
								onClick={handleClickName(userId, isNameClickable)}
							>
								{getFullName(userFirstName, userLastName)}
							</BookingUserName>
						</BookingListTypography>
					)}
					<ItemRowWithIcon>
						<IconWrapper>
							<CalendarIcon />
						</IconWrapper>
						{createPeriodInText(weeksInInterval, bookingDateList, t)
							?.split('\n')
							.map((line, index) => (
								<BookingListTypography
									key={index}
									Component="p"
									variant="m-400"
									color={theme.palette['text-black']}
								>
									{line}
								</BookingListTypography>
							))}
					</ItemRowWithIcon>
					<ItemRowWithIcon>
						<IconWrapper>
							<MapPinIcon />
						</IconWrapper>
						<BookingListTypography
							Component="p"
							variant="m-400"
							color={theme.palette['text-black']}
						>
							{meetingRoomName}
						</BookingListTypography>
					</ItemRowWithIcon>
					<ItemRowWithIcon>
						<IconWrapper>
							<ClockIcon />
						</IconWrapper>
						<BookingListTypography
							Component="p"
							variant="m-400"
							color={theme.palette['text-black']}
						>
							{bookingTime}
						</BookingListTypography>
					</ItemRowWithIcon>
				</ListItemContent>
				<IconCloseWrapper>
					<ActionButton onClick={handleDelete}>
						<CrossIcon />
					</ActionButton>
				</IconCloseWrapper>
			</BookingListItem>
			<Divider />
		</>
	);
}

const areItemPropsEqual = (
	prevProps: { userId: number },
	nextProps: { userId: number }
) => prevProps.userId === nextProps.userId;

export const RoomBookingListItem = memo(ItemInner, areItemPropsEqual);
