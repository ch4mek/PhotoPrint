import { useTheme } from '@emotion/react';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { CalendarIcon } from '@mms/mms-ui-library/icons/base/CalendarIcon';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { MapPinIcon } from '@mms/mms-ui-library/icons/base/MapPinIcon';
import { memo } from 'react';
import { useTranslation } from 'react-i18next';

import { BOOKING_LIST_TYPOGRAPHY_CLASS } from '../constants';

import { BOOKING_ITEM_TIME_FORMAT_WITH_COMMA } from '@/constants/booking';
import { isManagerRoleOrGroupManager } from '@/constants/roles';
import { createPeriodInText, getBookingPlaceType } from '@/helpers/booking';
import { formatDisplayDate } from '@/helpers/bookingFormatters';
import { getFullName } from '@/helpers/formatName';
import {
	BookedCreatorTypography,
	BookingListForGuest,
	BookingListItem,
	BookingListTypography,
	BookingUserName,
	Divider,
	IconCloseWrapper,
	IconWrapper,
	ItemRowWithIcon,
	ListItemContent,
} from '@/pages/Booking/components/BookingList/styles';
import { BOOKING_ITEM_TEST_ID } from '@/pages/Booking/constants';
import { getDeskName, getSpaceAbbreviation } from '@/pages/Booking/utils';

import type { ItemInnerProps } from './types';

function ItemInner({
	currentUserId,
	userRole,
	userId,
	isBlock,
	lastName,
	firstName,
	weeksInInterval,
	dateList,
	workplaceNumber,
	handleDelete,
	office,
	workspaceId,
	isForGuest,
	guestName,
	handleClickName,
	isNameClickable,
	bookedBy,
}: ItemInnerProps) {
	const theme = useTheme();
	const { t } = useTranslation('common');
	const { t: tBooking } = useTranslation('booking');

	const formattedDate = formatDisplayDate(
		dateList[0],
		BOOKING_ITEM_TIME_FORMAT_WITH_COMMA
	);

	return (
		<>
			<BookingListItem
				data-testid={`${BOOKING_ITEM_TEST_ID}-${
					isForGuest ? guestName : getFullName(firstName, lastName)
				}`}
				type={getBookingPlaceType(currentUserId, userId, isBlock, isForGuest)}
				isNeedUnderline={isNameClickable}
			>
				<ListItemContent>
					{isForGuest ? (
						<BookingListForGuest>
							<Tooltip text={guestName} useIfTextCut>
								<BookingListTypography
									Component="p"
									variant="m-600"
									color={theme.palette['text-black']}
									isBookedBy
								>
									{guestName}
								</BookingListTypography>
							</Tooltip>

							<Tooltip
								text={tBooking('tooltips.bookedBy', { name: bookedBy })}
								useIfTextCut
							>
								<BookedCreatorTypography variant="s-400">
									{tBooking('tooltips.bookedBy', { name: bookedBy })}
								</BookedCreatorTypography>
							</Tooltip>
						</BookingListForGuest>
					) : (
						lastName &&
						firstName && (
							<BookingListTypography
								Component="p"
								variant="m-600"
								color={theme.palette['text-black']}
							>
								<BookingUserName
									onClick={handleClickName(userId, isNameClickable)}
									className={BOOKING_LIST_TYPOGRAPHY_CLASS}
								>
									{getFullName(firstName, lastName)}
								</BookingUserName>
							</BookingListTypography>
						)
					)}

					<ItemRowWithIcon>
						<IconWrapper>
							<CalendarIcon />
						</IconWrapper>
						{isBlock ? (
							<BookingListTypography
								Component="p"
								variant="m-400"
								color={theme.palette['text-black']}
							>
								{t('placeholders.from')} {formattedDate}
							</BookingListTypography>
						) : (
							createPeriodInText(weeksInInterval, dateList, t)
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
								))
						)}
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
							{getDeskName(
								getSpaceAbbreviation(office.id, workspaceId),
								workplaceNumber
							)}
						</BookingListTypography>
					</ItemRowWithIcon>
				</ListItemContent>
				<IconCloseWrapper>
					{!(isBlock && !isManagerRoleOrGroupManager(userRole)) && (
						<ActionButton onClick={handleDelete}>
							<CrossIcon />
						</ActionButton>
					)}
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

export const DeskBookingListItem = memo(ItemInner, areItemPropsEqual);
