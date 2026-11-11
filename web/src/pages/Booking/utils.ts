import { TFunction } from 'i18next';

import { Timezone } from '@/components/Timeline/types';
import {
	DEFAULT_MINSK_SPACE_ABBREVIATION,
	MINSK_SPACE_ABBREVIATIONS,
	WARSAW_SPACE_ABBREVIATION,
} from '@/constants/booking';
import { HighlightedDays } from '@/context/types';
import {
	convertDeskDateListToDayjs,
	getRequestBookingDate,
} from '@/helpers/bookingTime';
import { isMeetingRoom } from '@/helpers/isMeetingRoom';
import dayjs from '@/lib/dayjs';
import {
	GetBookingResponse,
	GetRoomsBookingResponse,
} from '@/queries/booking/types';
import { DeskBooking, MeetingRoomBooking } from '@/types/booking';

import {
	getDateRange,
	getRepetitiveWeekDays,
} from './components/BookingList/utils';
import { BookingOfficesId, ParsedBookingItem } from './types';

const convertOfficeDateToISOSFormat = (officeDate: string) =>
	`${officeDate}T00:00:00.000Z`;

export const getMeetingRoomDateList = (
	reservationPeriodFromOfficeDate: string,
	reservationPeriodToOfficeDate: string,
	isRepetitiveBooking: boolean
) => {
	if (isRepetitiveBooking) {
		return [reservationPeriodFromOfficeDate, reservationPeriodToOfficeDate].map(
			(date) =>
				dayjs(convertOfficeDateToISOSFormat(date)).utc(false).toISOString()
		);
	}

	if (reservationPeriodFromOfficeDate === reservationPeriodToOfficeDate) {
		return [convertOfficeDateToISOSFormat(reservationPeriodToOfficeDate)];
	}

	return [
		convertOfficeDateToISOSFormat(reservationPeriodFromOfficeDate),
		convertOfficeDateToISOSFormat(reservationPeriodToOfficeDate),
	];
};

export const getBlockedMonthDays = () =>
	new Array(31).fill(0).map((_, index) => index + 1);

const getParsedBookingItem = (
	item: DeskBooking | MeetingRoomBooking
): ParsedBookingItem => {
	if (isMeetingRoom(item)) {
		const {
			reservationPeriodFromOfficeDate,
			reservationPeriodToOfficeDate,
			weeksInInterval,
			userId,
			isRepetitiveBooking,
			reservations,
		} = item;

		const reservationEndOfficeDate =
			reservations.at(-1)?.reservationToOfficeDateTime ??
			reservationPeriodToOfficeDate;

		return {
			dateList: getMeetingRoomDateList(
				reservationPeriodFromOfficeDate,
				getRequestBookingDate(dayjs(reservationEndOfficeDate)),
				isRepetitiveBooking
			),
			weeksInInterval,
			isBlock: false,
			userId,
		};
	}

	return item;
};

const isDeskBooking = (item: ParsedBookingItem): item is DeskBooking =>
	'isForGuest' in item;

const isGuestBooking = (item: ParsedBookingItem): boolean =>
	isDeskBooking(item) && item.isForGuest;

export const extractHighlightedDaysFromResponse = (
	desksData: GetBookingResponse,
	roomsData: GetRoomsBookingResponse,
	id: number,
	timezone: Timezone
) => {
	const deskAndMeetingRoomsBooking = [
		...desksData.bookings,
		...roomsData.map(getParsedBookingItem),
	];
	const days = deskAndMeetingRoomsBooking.reduce(
		(acc: HighlightedDays, item) => {
			const { dateList, isBlock, userId, weeksInInterval } = item;
			const dayjsDateList = convertDeskDateListToDayjs(dateList, timezone);

			if (userId === id && !isGuestBooking(item)) {
				if (weeksInInterval > 0) {
					const repetitiveWeekDays = getRepetitiveWeekDays(
						dayjsDateList,
						weeksInInterval
					);
					acc.yourList?.push(...repetitiveWeekDays);

					return acc;
				}

				if (dayjsDateList.length === 1) {
					acc.yourList?.push(...dayjsDateList);

					return acc;
				}

				const [rangeStart, rangeEnd] = dayjsDateList;

				if (isBlock) {
					acc.blockStartDate = rangeStart;
				} else {
					const dateRange = getDateRange(rangeStart, rangeEnd);

					acc.yourList?.push(...dateRange);
				}

				return acc;
			}

			acc.bookedList?.push(...dayjsDateList);

			return acc;
		},
		{
			bookedList: [],
			blockStartDate: null,
			yourList: [],
		}
	);

	return days;
};

export const getDeskName = (abbreviation: string, deskNumber: number) =>
	`${abbreviation} - ${deskNumber}`;

export const getBookingDeletingToastMessage = (
	isBlock: boolean,
	isOwnBooking: boolean,
	t: TFunction<'booking'>
) => {
	if (isBlock) {
		return t('toasts.deskUnblocked');
	}

	if (isOwnBooking) {
		return t('toasts.successBookingCanceled');
	}

	return t('toasts.bookingCanceled');
};

export const getSpaceAbbreviation = (
	officeId: number,
	spaceId: number | null
) => {
	if (officeId === BookingOfficesId.Minsk) {
		return spaceId
			? MINSK_SPACE_ABBREVIATIONS[spaceId] || DEFAULT_MINSK_SPACE_ABBREVIATION
			: DEFAULT_MINSK_SPACE_ABBREVIATION;
	}

	return WARSAW_SPACE_ABBREVIATION;
};
