import i18next, { TFunction } from 'i18next';

import { MINUTES_IN_HOUR } from '@/components/Timeline/constants';
import { Timezone } from '@/components/Timeline/types';
import {
	BookPlaceType,
	BookType,
	NOT_WORKING_DAY_END_HOUR,
	spacesRestrictions,
	TIME_SELECTOR_STEP_IN_MINUTES,
	WORKING_DAY_END_HOUR,
	WORKING_DAY_START_HOUR,
} from '@/constants/booking';
import { DESK_BOOKING_IS_NOT_ALLOWED } from '@/constants/errors';
import { isChiefRole, Roles } from '@/constants/roles';
import { SelectedBookingItem, SelectedBookingItemType } from '@/context/types';
import {
	createPeriodInText,
	getDateAsResponseType,
	getDeleteDialogHeader,
	getDeleteDialogText,
	getDeskBlockDialogSubtext,
	getEndOfMonthForRequest,
	getStartOfMonthForRequest,
	getTextFromMultipleDate,
	getTooltipTitle,
	getBookType,
	getBookingPlaceType,
	isAllowToBook,
	UN_RESTRICTED_USER_EMAIL,
} from '@/helpers/booking';
import { getFormattedTime } from '@/helpers/bookingFormatters';
import {
	getDayStartHour,
	getInitialFromTime,
	getInitialTime,
} from '@/helpers/bookingTime';
import { findById } from '@/helpers/findById';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { BookingEntity, MinskSpacesInternalCodes } from '@/types/booking';
import { LocationTypeId, OfficeType } from '@/types/configuration';

import '@/i18n';

jest.mock('@/constants/roles', () => ({
	...jest.requireActual('@/constants/roles'),
	isChiefRole: jest.fn(),
}));
jest.mock('@/helpers/findById', () => ({
	findById: jest.fn(),
}));
jest.mock('@/constants/booking', () => ({
	...jest.requireActual('@/constants/booking'),
	spacesRestrictions: {},
}));

const { Repetitive, Multiple, Single } = BookType;
const { Blocked, Selected, Booked, YourBlocked } = BookPlaceType;
const BookingEntityBlocked: BookingEntity = {
	bookingId: 1,
	userId: 1,
	firstName: 'string',
	lastName: 'string',
	dateList: ['2023-08-08T00:00:00.000Z', '2023-08-15T00:00:00.000Z'],
	isBlock: true,
	workplaceNumber: 1,
	workplaceId: 1,
	weeksInInterval: 1,
};
const BookingEntityUnblocked: BookingEntity = {
	bookingId: 1,
	userId: 1,
	firstName: 'string',
	lastName: 'string',
	dateList: ['2023-08-08T00:00:00.000Z', '2023-08-15T00:00:00.000Z'],
	isBlock: false,
	workplaceNumber: 1,
	workplaceId: 1,
	weeksInInterval: 1,
};

const dayjsDate = dayjs('2023-08-08T00:00:00.000Z');

const minskTimezoneOffset = -180;
dayjs.tz.setDefault('UTC');

const padZero = (value: number) => String(value).padStart(2, '0');
const createISOTime = (hours: number, minutes: number) =>
	`2024-10-07T${padZero(hours)}:${padZero(minutes)}:00.000Z`;

const getOfficeTimeTimestampInUTC = (
	isoDate: string,
	officeTimezoneOffset: number
) => Date.parse(isoDate) + officeTimezoneOffset * 60 * 1000;

const mockIsChiefRole = isChiefRole as jest.MockedFunction<typeof isChiefRole>;
const mockFindById = findById as jest.MockedFunction<typeof findById>;
const bookingMock = jest.requireMock('@/constants/booking');

const SELECT_DESK = 'Please select a desk to continue';
const SELECT_ROOM = 'Please select a meeting room';
const SELECT_DATE = 'Please select a date';

const mockBookingT = ((key: string) => {
	const translations: Record<string, string> = {
		'dialogs.unblock.header': 'Desk unblocking',
		'dialogs.cancel.header': 'Booking cancelation',
		'dialogs.unblock.confirm': 'Are you sure you want to unblock the desk?',
		'dialogs.cancel.confirmOwn':
			'Are you sure you want to cancel your booking?',
		'tooltips.bookingNotAllowed': DESK_BOOKING_IS_NOT_ALLOWED,
		'errors.selectDesk': SELECT_DESK,
		'errors.selectRoom': SELECT_ROOM,
		'errors.selectDate': SELECT_DATE,
	};

	return translations[key] || key;
}) as TFunction<'booking'>;

const mockPlaceholdersT = ((key: string) =>
	key === 'from' ? 'From' : key) as TFunction<'common', 'placeholders'>;

describe('Helper function getBookType', () => {
	it('Should return Single when weeksInInterval is 0 and dateList.lenth is 0', () => {
		expect(getBookType(0, [])).toBe(Single);
	});

	it('Should return Single when weeksInInterval is 0 and dateList.lenth is 1', () => {
		expect(getBookType(0, ['2023-08-08T00:00:00.000Z'])).toBe(Single);
	});

	it('Should return Multiple when weeksInInterval is 0 and dateList.lenth more then 1', () => {
		expect(
			getBookType(0, ['2023-08-08T00:00:00.000Z', '2023-08-15T00:00:00.000Z'])
		).toBe(Multiple);
	});

	it('Should return Repetitive when weeksInInterval more then 0 and dateList.lenth more then 0', () => {
		expect(getBookType(1, ['2023-08-08T00:00:00.000Z'])).toBe(Repetitive);
	});
});

describe('Helper function getBookingPlaceType', () => {
	it("Should return Selected if currentUserId and bookUserId are equal and desk isn't blocked", () => {
		expect(getBookingPlaceType(1, 1, false)).toBe(Selected);
	});

	it('Should return Blocked if currentUserId and bookUserId are equal and desk is blocked', () => {
		expect(getBookingPlaceType(1, 1, true)).toBe(YourBlocked);
	});

	it("Should return Booked if currentUserId and bookUserId aren't equal and desk isn't blocked", () => {
		expect(getBookingPlaceType(0, 1, false)).toBe(Booked);
	});

	it("Should return Blocked if currentUserId and bookUserId aren't equal and desk is blocked", () => {
		expect(getBookingPlaceType(0, 1, true)).toBe(Blocked);
	});
});

describe('Helper function getTextFromMultipleDate', () => {
	beforeEach(() => {
		i18next.changeLanguage('en');
	});

	it('Should return one date if there are two same date string', () => {
		expect(
			getTextFromMultipleDate([
				'2023-08-08T00:00:00.000Z',
				'2023-08-08T00:00:00.000Z',
			])
		).toBe('08 Aug, 2023');
	});

	it('Should return correct date interval when there are only different monthes', () => {
		expect(
			getTextFromMultipleDate([
				'2023-08-08T00:00:00.000Z',
				'2023-09-08T00:00:00.000Z',
			])
		).toBe('08 Aug - 08 Sep, 2023');
	});

	it('Should return correct date interval when there are only different years ', () => {
		expect(
			getTextFromMultipleDate([
				'2023-08-08T00:00:00.000Z',
				'2024-08-08T00:00:00.000Z',
			])
		).toBe('08 Aug, 2023 - 08 Aug, 2024');
	});

	it('Should return correct date interval when there are only different days ', () => {
		expect(
			getTextFromMultipleDate([
				'2023-08-08T00:00:00.000Z',
				'2023-08-09T00:00:00.000Z',
			])
		).toBe('08-09 Aug, 2023');
	});
});

describe('Helper function createPeriodInText', () => {
	beforeEach(() => {
		i18next.changeLanguage('en');
	});

	it('Should return correct date string whan weeksInInterval is 0 and dateList.lenth more then 1', () => {
		expect(
			createPeriodInText(0, [
				'2023-08-08T00:00:00.000Z',
				'2023-08-09T00:00:00.000Z',
			])
		).toBe('08-09 Aug, 2023');
	});

	it('Should return correct date string whan weeksInInterval is 0 dates with equal years, but different months', () => {
		expect(
			createPeriodInText(0, [
				'2023-08-09T00:00:00.000Z',
				'2023-09-08T00:00:00.000Z',
			])
		).toBe('09 Aug - 08 Sep, 2023');
	});

	it('Should return correct date string whan weeksInInterval is 0 and dateList.lenth is 1', () => {
		expect(createPeriodInText(0, ['2023-08-08T00:00:00.000Z'])).toBe(
			'08 Aug, 2023'
		);
	});

	it('Should return correct date string whan weeksInInterval is 1 and dateList.lenth more then 1', () => {
		expect(
			createPeriodInText(1, [
				'2023-08-08T00:00:00.000Z',
				'2024-08-15T00:00:00.000Z',
			])
		).toBe('08 Aug, 2023 - 15 Aug, 2024 on Tuesdays');
	});
});

describe('Helper function getStartOfMonthForRequest', () => {
	it('Should return first day of month if there is a day in the middle of month', () => {
		const result = getStartOfMonthForRequest(dayjsDate) ?? '';

		expect(result.slice(0, result.length - 1)).toBe('2023-08-01T00:00:00.000');
	});

	it('Should return last day of month if there is a day in the middle of month', () => {
		const result = getEndOfMonthForRequest(dayjsDate) ?? '';

		expect(result.slice(0, result.length - 1)).toBe('2023-08-31T00:00:00.000');
	});

	it('Should return undefined if there is a null', () => {
		const result = getEndOfMonthForRequest(null);
		expect(result).toBe(undefined);
	});
});

describe('Helper function getEndOfMonthForRequest', () => {
	const mockDateChain = {
		clone: jest.fn().mockReturnThis(),
		endOf: jest.fn().mockReturnThis(),
		hour: jest.fn().mockReturnThis(),
		minute: jest.fn().mockReturnThis(),
		second: jest.fn().mockReturnThis(),
		millisecond: jest.fn().mockReturnThis(),
		format: jest.fn().mockReturnValue('2023-12-31T00:00:00.000Z'),
	};

	it('should return formatted end of month when date is provided', () => {
		const mockDate = {
			clone: jest.fn().mockReturnValue(mockDateChain),
		} as unknown as Dayjs;

		const result = getEndOfMonthForRequest(mockDate);

		expect(mockDate.clone).toHaveBeenCalled();
		expect(mockDateChain.endOf).toHaveBeenCalledWith('month');
		expect(mockDateChain.hour).toHaveBeenCalledWith(0);
		expect(mockDateChain.minute).toHaveBeenCalledWith(0);
		expect(mockDateChain.second).toHaveBeenCalledWith(0);
		expect(mockDateChain.millisecond).toHaveBeenCalledWith(0);
		expect(result).toBe('2023-12-31T00:00:00.000Z');
	});

	it('should return undefined when date is null', () => {
		const result = getEndOfMonthForRequest(null);
		expect(result).toBeUndefined();
	});
});

describe('Helper function getDateAsResponseType', () => {
	it('Should return iso date string in format YYYY-MM-DDT00:00:00.000Z', () => {
		expect(getDateAsResponseType(dayjsDate)).toBe('2023-08-08T00:00:00.000Z');
	});
});

describe('Helper function getDeleteDialogHeader', () => {
	it('Should return UNBLOCK_DIALOG_HEADER if value is BookEntity type and isBlock is true', () => {
		expect(
			getDeleteDialogHeader(BookingEntityBlocked.isBlock, mockBookingT)
		).toBe('Desk unblocking');
	});

	it('Should return CANCEL_DIALOG_HEADER if value is BookEntity type and isBlock is false', () => {
		expect(
			getDeleteDialogHeader(BookingEntityUnblocked.isBlock, mockBookingT)
		).toBe('Booking cancelation');
	});
});

describe('Helper function getDeleteDialogText', () => {
	it('Should return SURE_TO_UNBLOCK if BookingEntity is blocked and userId and currentUserId are same', () => {
		expect(getDeleteDialogText(BookingEntityBlocked, 1, mockBookingT)).toBe(
			'Are you sure you want to unblock the desk?'
		);
	});

	it("Should return SURE_TO_UNBLOCK if BookingEntity is blocked and userId and currentUserId aren\\'t same", () => {
		expect(getDeleteDialogText(BookingEntityBlocked, 0, mockBookingT)).toBe(
			'Are you sure you want to unblock the desk?'
		);
	});

	it("Should return SURE_TO_CANCEL_BOOKING if BookingEntity isn\\'t blocked and userId and currentUserId are same", () => {
		expect(getDeleteDialogText(BookingEntityUnblocked, 1, mockBookingT)).toBe(
			'Are you sure you want to cancel your booking?'
		);
	});

	it("Should return null if BookingEntity isn\\'t blocked and userId and currentUserId aren\\'t same", () => {
		expect(getDeleteDialogText(BookingEntityUnblocked, 0, mockBookingT)).toBe(
			null
		);
	});
});

describe('Helper function getDeskBlockDialogSubtext', () => {
	beforeEach(() => {
		i18next.changeLanguage('en');
	});

	it('Should return correct text', () => {
		expect(
			getDeskBlockDialogSubtext(
				dayjs('2023-08-08T00:00:00.000Z'),
				mockPlaceholdersT
			)
		).toBe(`From 08 Aug, 2023`);
	});
});

describe('Helper function getTooltipTitle', () => {
	it('Should return null if there are date and number more then 0 on input', () => {
		expect(
			getTooltipTitle(
				dayjs('2023-08-08T00:00:00.000Z'),
				{
					displayNumber: 1,
					abbreviation: 'W',
					id: 1,
					spaceId: 1,
					type: SelectedBookingItemType.Desk,
				} as SelectedBookingItem,
				false,
				true,
				mockBookingT
			)
		).toBe(null);
	});

	it('Should return SELECT_DESK if there are date and 0 on input', () => {
		expect(
			getTooltipTitle(
				dayjs('2023-08-08T00:00:00.000Z'),
				null,
				false,
				true,
				mockBookingT
			)
		).toBe(SELECT_DESK);
	});

	it('Should return SELECT_DATE if lastDay is undefined and selected is 0', () => {
		expect(getTooltipTitle(undefined, null, false, true, mockBookingT)).toBe(
			SELECT_DATE
		);
	});

	it("Should return SELECT_DATE if lastDay is undefined and selected isn\\'t 0", () => {
		expect(
			getTooltipTitle(
				undefined,
				{
					displayNumber: 1,
					abbreviation: 'W',
					id: 1,
					spaceId: 1,
					type: SelectedBookingItemType.Desk,
				} as SelectedBookingItem,
				false,
				true,
				mockBookingT
			)
		).toBe(SELECT_DATE);
	});

	it('Should return null when meeting room is opened', () => {
		expect(
			getTooltipTitle(
				dayjs('2023-08-08T00:00:00.000Z'),
				{
					id: 1,
					type: SelectedBookingItemType.Room,
					name: 'Room 1',
				},
				true,
				true,
				mockBookingT
			)
		).toBe(null);
	});

	it('Should return SELECT_ROOM', () => {
		expect(
			getTooltipTitle(
				dayjs('2023-08-08T00:00:00.000Z'),
				null,
				true,
				true,
				mockBookingT
			)
		).toBe(SELECT_ROOM);
	});

	it('Should return DESK_BOOKING_IS_NOT_ALLOWED', () => {
		expect(
			getTooltipTitle(
				dayjs('2024-11-20T00:00:00.000Z'),
				null,
				true,
				false,
				mockBookingT
			)
		).toBe(DESK_BOOKING_IS_NOT_ALLOWED);
	});
});

describe('Helper function getInitialFromTime', () => {
	it('Should return start of working day', () => {
		const hours = 1;
		const minutes = 0;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs(officeTime);
		const areHoursWorking = true;

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(getDayStartHour(areHoursWorking), 0)
		);
	});

	it('Should return nearest time for working day', () => {
		const hours = 9;
		const minutes = 32;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs.tz(officeTime);
		const areHoursWorking = true;

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(hours, 45)
		);
	});

	it('Should return nearest time for the start of working day', () => {
		const hours = 8;
		const minutes = 0;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs.tz(officeTime);
		const areHoursWorking = true;

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(hours, TIME_SELECTOR_STEP_IN_MINUTES)
		);
	});

	it('Should return nearest time for non working hours', () => {
		const hours = 1;
		const minutes = 0;
		const areHoursWorking = false;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs.tz(officeTime);

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(
				hours,
				(minutes + TIME_SELECTOR_STEP_IN_MINUTES) % MINUTES_IN_HOUR
			)
		);
	});

	it('Should return correct time for next day', () => {
		const hours = 23;
		const minutes = 59;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs.tz(officeTime);
		const areHoursWorking = false;

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(0, 0)
		);
	});

	it('Should return correct time for next day on the end of working day', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES;
		const areHoursWorking = true;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs(officeTime);

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, 0)
		);
	});

	it('Should return correct time for the end of the end of working day', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES * 2;
		const officeTime = createISOTime(hours, minutes);
		const officeDayjsTime = dayjs.tz(officeTime);
		const areHoursWorking = true;

		expect(getInitialFromTime(areHoursWorking, officeDayjsTime)).toBe(
			getFormattedTime(
				WORKING_DAY_END_HOUR,
				MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES
			)
		);
	});
});

describe('getInitialTime helper', () => {
	beforeEach(() => {
		jest.useFakeTimers();
	});

	afterEach(() => {
		jest.useRealTimers();
	});

	it('Should return correct initial time if current office time is 19:30', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES * 2;
		const areHoursWorking = true;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const { from, to } = getInitialTime(areHoursWorking, timezone);

		expect(getFormattedTime(from.hour(), from.minute())).toBe(
			getFormattedTime(
				WORKING_DAY_END_HOUR,
				MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES
			)
		);
		expect(getFormattedTime(to.hour(), to.minute())).toBe(
			getFormattedTime(WORKING_DAY_END_HOUR + 1, 0)
		);
	});

	it('Should return next working day if multiple', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES;
		const areHoursWorking = true;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);
		const { from, to } = getInitialTime(areHoursWorking, timezone);

		expect(getFormattedTime(from.hour(), from.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, 0)
		);
		expect(getFormattedTime(to.hour(), to.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, TIME_SELECTOR_STEP_IN_MINUTES)
		);
	});

	it('Should return next working day if not multiple', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES + 1;
		const areHoursWorking = true;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const { from, to } = getInitialTime(areHoursWorking, timezone);

		expect(getFormattedTime(from.hour(), from.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, 0)
		);
		expect(getFormattedTime(to.hour(), to.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, TIME_SELECTOR_STEP_IN_MINUTES)
		);
	});

	it('Should return start of working day', () => {
		const hours = WORKING_DAY_START_HOUR - 1;
		const minutes = MINUTES_IN_HOUR - TIME_SELECTOR_STEP_IN_MINUTES + 1;
		const areHoursWorking = true;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const { from, to } = getInitialTime(areHoursWorking, timezone);

		expect(getFormattedTime(from.hour(), from.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, 0)
		);
		expect(getFormattedTime(to.hour(), to.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, TIME_SELECTOR_STEP_IN_MINUTES)
		);
	});

	it('Should return start of working day', () => {
		const hours = WORKING_DAY_END_HOUR;
		const minutes = 59;
		const areHoursWorking = true;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const { from, to } = getInitialTime(areHoursWorking, timezone);

		expect(getFormattedTime(from.hour(), from.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, 0)
		);
		expect(getFormattedTime(to.hour(), to.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, TIME_SELECTOR_STEP_IN_MINUTES)
		);
	});

	it('Should return next working day if current time is the end of day', () => {
		const hours = NOT_WORKING_DAY_END_HOUR;
		const minutes = 59;
		const areHoursWorking = true;
		const timezone = Timezone.MINSK;
		const officeTime = createISOTime(hours, minutes);
		const officeTimezoneOffset = minskTimezoneOffset;
		jest.setSystemTime(
			new Date(getOfficeTimeTimestampInUTC(officeTime, officeTimezoneOffset))
		);

		const { from, to } = getInitialTime(areHoursWorking, timezone);

		expect(getFormattedTime(from.hour(), from.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, 0)
		);
		expect(getFormattedTime(to.hour(), to.minute())).toBe(
			getFormattedTime(WORKING_DAY_START_HOUR, TIME_SELECTOR_STEP_IN_MINUTES)
		);
	});
});

describe('Helper function isAllowBook', () => {
	const mockOffice: OfficeType = {
		location: LocationTypeId.Minsk,
		timeZone: Timezone.MINSK,
		timeZoneDisplayName: '',
		workspaces: [],
		meetingRooms: [],
		id: 1,
		name: 'Test office',
	};

	beforeEach(() => {
		jest.clearAllMocks();
		mockIsChiefRole.mockReturnValue(false);
		bookingMock.spacesRestrictions = {};
	});

	it('should return true for SpaceE for Qa department', () => {
		const mockSpace = {
			id: 1,
			internalCode: MinskSpacesInternalCodes.SpaceE,
		};
		mockFindById.mockReturnValue(mockSpace);

		const result = isAllowToBook(Roles.SE, 'QA', mockOffice, 1, '');

		expect(result).toBe(true);
	});

	it('should return true for chief role regardless of restrictions', () => {
		const mockSpace = {
			id: 1,
			internalCode: MinskSpacesInternalCodes.SpaceA,
		};
		mockFindById.mockReturnValue(mockSpace);
		mockIsChiefRole.mockReturnValue(true);

		const result = isAllowToBook(Roles.CEO, 'dept1', mockOffice, 1, '');

		expect(mockIsChiefRole).toHaveBeenCalledWith(Roles.CEO);
		expect(result).toBe(true);
	});

	it('should return true for unrestricted user email', () => {
		const mockSpace = {
			id: 1,
			internalCode: MinskSpacesInternalCodes.SpaceA,
		};
		mockFindById.mockReturnValue(mockSpace);
		mockIsChiefRole.mockReturnValue(false);

		const result = isAllowToBook(
			Roles.SE,
			'dept1',
			mockOffice,
			1,
			UN_RESTRICTED_USER_EMAIL
		);

		expect(result).toBe(true);
	});

	it('should return true when user department is in restrictions', () => {
		const mockSpace = {
			id: 1,
			internalCode: MinskSpacesInternalCodes.SpaceA,
		};
		mockFindById.mockReturnValue(mockSpace);
		mockIsChiefRole.mockReturnValue(false);

		const result = isAllowToBook(
			Roles.SE,
			'dept1',
			mockOffice,
			1,
			'marina.krivel@modsen-software.com'
		);

		expect(result).toBe(true);
	});

	it('should return false when user department is not in restrictions', () => {
		const mockSpace = {
			id: 1,
			internalCode: MinskSpacesInternalCodes.SpaceA,
		};
		mockFindById.mockReturnValue(mockSpace);
		mockIsChiefRole.mockReturnValue(false);

		spacesRestrictions[MinskSpacesInternalCodes.SpaceA] = ['dept2', 'dept3'];

		const result = isAllowToBook(Roles.SE, 'dept1', mockOffice, 1, '');

		expect(result).toBe(false);
	});

	it('should return true when no restrictions exist for space', () => {
		const mockSpace = {
			id: 1,
			internalCode: MinskSpacesInternalCodes.SpaceB,
		};
		mockFindById.mockReturnValue(mockSpace);
		mockIsChiefRole.mockReturnValue(false);

		spacesRestrictions[MinskSpacesInternalCodes.SpaceA] = ['dept1'];

		const result = isAllowToBook(Roles.SE, 'dept1', mockOffice, 1, '');

		expect(result).toBe(true);
	});

	it('should return true when no space is found', () => {
		mockFindById.mockReturnValue(undefined);
		mockIsChiefRole.mockReturnValue(false);

		const result = isAllowToBook(Roles.SE, 'dept1', mockOffice, null, '');

		expect(result).toBe(true);
	});

	it('should call findById with correct parameters', () => {
		const spaceId = 123;
		mockFindById.mockReturnValue(undefined);

		isAllowToBook(Roles.SE, 'dept1', mockOffice, spaceId, '');

		expect(mockFindById).toHaveBeenCalledWith(mockOffice.workspaces, spaceId);
	});
});
