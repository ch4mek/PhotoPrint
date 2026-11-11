import { Timezone } from '@/components/Timeline/types';
import { Employee } from '@/context/types';
import { MinskSpacesInternalCodes } from '@/types/booking';
import { LocationTypeId, OfficeType } from '@/types/configuration';

import { Departments } from './departments';

export const BookPlaceType = {
	Booked: 'booked',
	Selected: 'selected',
	Blocked: 'blocked',
	YourBlocked: 'yourBlocked',
	Guest: 'guest',
};

export const BookType = {
	Single: 'single',
	Repetitive: 'repetitive',
	Multiple: 'multiple',
};

export const bookingOfficeNames: Record<
	string,
	'offices.warsaw' | 'offices.minsk'
> = {
	[LocationTypeId.Warsaw]: 'offices.warsaw',
	[LocationTypeId.Minsk]: 'offices.minsk',
};

export const REPEAT_WEEKS_MIN_VALUE = 1;
export const REPEAT_WEEKS_MAX_VALUE = 4;

export enum RequestTypes {
	CreateSelf = 'CreateSelf',
	CreateBlock = 'CreateBlock',
	CreateForUser = 'CreateForUser',
	CreateForGuest = 'CreateForGuest',
}

export const COLORS_ENUM = {
	Available: 'available',
	Blocked: 'blocked',
	Booked: 'booked',
	Selected: 'selected',
	Guest: 'guest',
};
export const COLORS = Object.values(COLORS_ENUM);

export const DESK_STATUS_LABEL_KEYS = {
	[COLORS_ENUM.Available]: 'deskStatus.available',
	[COLORS_ENUM.Blocked]: 'deskStatus.blocked',
	[COLORS_ENUM.Booked]: 'deskStatus.booked',
	[COLORS_ENUM.Selected]: 'deskStatus.selected',
	[COLORS_ENUM.Guest]: 'deskStatus.guest',
} as const;

export const MEETING_ROOMS_TAB_ID = 42;

export const MINSK_SPACE_ABBREVIATIONS: Record<number, string> = {
	1: '403',
	2: '404',
	3: '405',
	4: '406',
	5: '411',
	9: '207',
};

export const DEFAULT_MINSK_SPACE_ABBREVIATION = MINSK_SPACE_ABBREVIATIONS[1];

export const WARSAW_SPACE_ABBREVIATION = 'W';

export const ALL_DAY_TIME = {
	from: '00:00:00',
	to: '23:59:59',
};

const HH_MM_LENGTH_WITH_SEPARATOR = 5;

export const ALL_DAY_TIME_HH_MM = {
	from: ALL_DAY_TIME.from.slice(0, HH_MM_LENGTH_WITH_SEPARATOR),
	to: ALL_DAY_TIME.to.slice(0, HH_MM_LENGTH_WITH_SEPARATOR),
};

export const TIME_SELECTOR_STEP_IN_MINUTES = 15;
export const WORKING_DAY_START_HOUR = 8;
export const WORKING_DAY_END_HOUR = 19;
export const NOT_WORKING_DAY_START_HOUR = 0;
export const NOT_WORKING_DAY_END_HOUR = 23;

export const VIEW_TIME_FORMAT = 'HH:mm';
export const RESPONSE_TIME_FORMAT = 'HH:mm:ss';
export const UTC_RESPONSE_TIME_FORMAT = 'HH:mm:ss';
export const RESPONSE_ALL_DAY_START_TIME_FORMAT = '00:00:00';
export const RESPONSE_ALL_DAY_END_TIME_FORMAT = '23:59:59';

export const BOOKING_ITEM_TIME_FORMAT = 'DD MMM YYYY';
export const BOOKING_ITEM_TIME_FORMAT_WITH_COMMA = 'DD MMM, YYYY';

export const spacesRestrictions: { [key: string]: Array<string> } = {
	[MinskSpacesInternalCodes.SpaceB]: [Departments.Js],
	[MinskSpacesInternalCodes.SpaceC]: [Departments.Js],
	[MinskSpacesInternalCodes.SpaceD]: [Departments.Java, Departments.LD],
	[MinskSpacesInternalCodes.SpaceE]: [Departments.QA],
};

export const COLOR_MAP = {
	available: 'background-base-3',
	guest: 'graphics-yellow',
	blocked: 'feedback-info',
	yourBlocked: 'graphics-accent',
	booked: 'feedback-success',
	selected: 'graphics-accent',
	meetingRoom: 'graphics-primary-2',
};

export const DEFAULT_BOOKING_OFFICE: OfficeType = {
	id: 1,
	name: 'Minsk',
	location: LocationTypeId.Minsk,
	timeZone: Timezone.MINSK,
	timeZoneDisplayName: LocationTypeId.Minsk,
	meetingRooms: [
		{
			id: 1,
			name: 'Meeting Room 1',
		},
	],
	workspaces: [
		{
			id: 1,
			name: 'Space A',
			internalCode: MinskSpacesInternalCodes.SpaceA,
		},
		{
			id: 2,
			name: 'Space B',
			internalCode: MinskSpacesInternalCodes.SpaceB,
		},
		{
			id: 3,
			name: 'Space C',
			internalCode: MinskSpacesInternalCodes.SpaceC,
		},
		{
			id: 4,
			name: 'Space D',
			internalCode: MinskSpacesInternalCodes.SpaceD,
		},
	],
};

export const DEFAULT_EMPLOYEE: Employee = {
	id: 0,
	name: 'Modsen Company',
	positionDescription: 'Employee',
};

// TODO: remove within MMS2-6978
export const HIDDEN_MEETING_ROOMS = ['203'];
export const HIDDEN_WORKSPACES = ['Space 207'];
