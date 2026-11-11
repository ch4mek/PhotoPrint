/* eslint-disable no-nested-ternary */
import i18next, { TFunction } from 'i18next';

import {
	BookPlaceType,
	BookType,
	BOOKING_ITEM_TIME_FORMAT_WITH_COMMA,
	spacesRestrictions,
} from '@/constants/booking';
import { DateFormat } from '@/constants/date';
import { Roles, isChiefRole } from '@/constants/roles';
import { SelectedBookingItem } from '@/context/types';
import { getDateLocale } from '@/i18n/locale';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { OfficeType } from '@/types/configuration';

import { formatDateParts, formatDisplayDate } from './bookingFormatters';
import { findById } from './findById';

const { Repetitive, Multiple, Single } = BookType;
const { Blocked, Selected, Booked, YourBlocked, Guest } = BookPlaceType;
export const UN_RESTRICTED_USER_EMAIL = 'marina.krivel@modsen-software.com';

export const getBookType = (
	weeksInInterval: number,
	dateList: Array<number | string>
) => (weeksInInterval ? Repetitive : dateList.length > 1 ? Multiple : Single);

export const getBookingPlaceType = (
	currentUserId: number,
	bookUserId: number,
	isBlock: boolean,
	isForGuest?: boolean
) => {
	if (isBlock && currentUserId === bookUserId) {
		return YourBlocked;
	}

	if (isForGuest) {
		return Guest;
	}

	if (isBlock) {
		return Blocked;
	}

	if (currentUserId === bookUserId) {
		return Selected;
	}

	return Booked;
};

const formatSingleDate = (date: Dayjs, isUtc: boolean) =>
	formatDisplayDate(date.utc(isUtc), BOOKING_ITEM_TIME_FORMAT_WITH_COMMA);

export const getTextFromMultipleDate = (
	dateList: Array<Dayjs | number | string>,
	isBlock = false,
	isUtc = false
) => {
	const start = dayjs(dateList[0]).utc(isUtc);
	const end = dayjs(dateList.at(-1)).utc(isUtc);
	const locale = getDateLocale(i18next.language);
	const startParts = formatDateParts(start.toDate(), locale);
	const endParts = formatDateParts(end.toDate(), locale);

	if (isBlock || start.isSame(end, 'day')) {
		return formatSingleDate(start, isUtc);
	}

	if (!start.isSame(end, 'year')) {
		return `${formatSingleDate(start, isUtc)} - ${formatSingleDate(
			end,
			isUtc
		)}`;
	}

	if (!start.isSame(end, 'month')) {
		return `${startParts.day} ${startParts.month} - ${endParts.day} ${endParts.month}, ${startParts.year}`;
	}

	if (!start.isSame(end, 'day')) {
		return `${startParts.day}-${endParts.day} ${startParts.month}, ${startParts.year}`;
	}

	return formatSingleDate(start, isUtc);
};

export const createPeriodInText = (
	weeksInInterval: number,
	dateList: Array<number | string>,
	t?: TFunction<'common'>,
	isBlock?: boolean,
	isUtc = false
) => {
	const date = dayjs(dateList[0]).utc(isUtc);
	switch (getBookType(weeksInInterval, dateList)) {
		case Single:
			return getTextFromMultipleDate([date], true, isUtc);
		case Multiple:
			return getTextFromMultipleDate(dateList, isBlock, isUtc);
		case Repetitive: {
			const weekdayKey = date.format('dddd').toLowerCase();
			const weekday = t && t(`periodWeekDays.${weekdayKey}` as any);

			return `${getTextFromMultipleDate(dateList, false, isUtc)} ${
				t ? t('recurrentOn', { weekday }) : `on ${date.format('dddd')}s`
			}`;
		}
		default:
			return null;
	}
};

export function getStartOfMonthForRequest(date: Dayjs | null) {
	return date?.clone().startOf('month').format(DateFormat.ISO);
}

export function getEndOfMonthForRequest(date: Dayjs | null) {
	return date
		?.clone()
		.endOf('month')
		.hour(0)
		.minute(0)
		.second(0)
		.millisecond(0)
		.format(DateFormat.ISO);
}

export const getDateAsResponseType = (date: Dayjs | string) => {
	if (typeof date === 'string') {
		return date;
	}

	return date.clone().startOf('day').format(DateFormat.ISO);
};

export const getDeleteDialogHeader = (
	isBlock: boolean,
	t: TFunction<'booking'>
) => t(isBlock ? 'dialogs.unblock.header' : 'dialogs.cancel.header');

export const getDeleteDialogText = (
	{
		isBlock,
		userId,
		isForGuest,
	}: {
		isBlock: boolean;
		userId: number;
		isForGuest?: boolean;
	},
	currentUserId: number,
	t: TFunction<'booking'>
) => {
	if (isForGuest) {
		return null;
	}

	if (isBlock) {
		return t('dialogs.unblock.confirm');
	}

	return userId === currentUserId ? t('dialogs.cancel.confirmOwn') : null;
};

export const getDeskBlockDialogSubtext = (
	firstDay: Dayjs,
	t: TFunction<'common', 'placeholders'>
) =>
	`${t('from')} ${formatDisplayDate(
		firstDay,
		BOOKING_ITEM_TIME_FORMAT_WITH_COMMA
	)}`;

export const getTooltipTitle = (
	lastDay: Dayjs | undefined,
	selected: SelectedBookingItem | null,
	areMeetingRoomsOpen: boolean,
	isWorkplaceBookAllowed: boolean,
	t: TFunction<'booking'>
) => {
	if (!isWorkplaceBookAllowed) {
		return t('tooltips.bookingNotAllowed');
	}

	if (!lastDay) {
		return t('errors.selectDate');
	}

	if (areMeetingRoomsOpen && !selected) {
		return t('errors.selectRoom');
	}

	if (!areMeetingRoomsOpen && !selected) {
		return t('errors.selectDesk');
	}

	return null;
};

export const isAllowToBook = (
	userRole: Roles,
	userDepartmentId: string | null,
	office: OfficeType,
	spaceId: number | null,
	email: string
) => {
	if (email === UN_RESTRICTED_USER_EMAIL) {
		return true;
	}

	if (isChiefRole(userRole)) {
		return true;
	}

	if (!spaceId) {
		return true;
	}

	const space = findById(office.workspaces, spaceId);

	if (!space) {
		return true;
	}

	const restrictions = spacesRestrictions[space.internalCode];

	if (!restrictions) {
		return true;
	}

	return restrictions.includes(userDepartmentId as string);
};
