import { Theme } from '@emotion/react';
import { Dispatch, JSX, ReactNode, SetStateAction } from 'react';
import { ToastOptions } from 'react-toastify';

import { LogoutReasons } from '@/constants/authConstants';
import { BookingActions } from '@/constants/bookingContext';
import { Roles } from '@/constants/roles';
import { BookingsState } from '@/context/Booking/types';
import { MainEquipmentFormValues } from '@/forms/EquipmentForm/MainForm/types';
import { Dayjs } from '@/lib/dayjs';
import { MeetingRoomsCollision, WorkplaceInfo } from '@/queries/booking/types';
import { ThemeMode } from '@/theme';
import { BookingModes, DeskBooking, MeetingRoomBooking } from '@/types/booking';
import type { Configuration, OfficeType } from '@/types/configuration';
import type { LookupUserResponse, UserInfo } from '@/types/userInfo';

export type ConfigurationState = Omit<
	ConfigurationContextValues,
	'setConfigurationContextValue'
>;

export interface ConfigurationContextValues extends Configuration {
	isLoading: boolean;
	setConfigurationContextValue: Dispatch<SetStateAction<ConfigurationState>>;
}

export interface ColorModeContextValues {
	mode: ThemeMode;
	toggleThemeMode: () => void;
	theme: Theme;
}

export interface ToastContextValues {
	open: (params: {
		message: string | JSX.Element | null;
		settings: ToastOptions;
	}) => void;
	subscribe: () => void;
}

export interface CalendarHighlightedDays {
	bookedList: number[];
	yourList: number[];
	blockedList: number[];
}

export interface HighlightedDays {
	bookedList: Dayjs[];
	yourList: Dayjs[];
	blockStartDate: Dayjs | null;
}

interface Workplace {
	bookingId: number;
	department: string;
	firstName: string;
	isBlocked: boolean;
	lastName: string;
	userId: number;
	workplaceId: number;
	workplaceNumber: number;
	isDisabled: boolean;
	isForGuest: boolean;
	guestName: string | null;
}

export type Employee = {
	id: number;
	name: string;
	positionDescription: string;
};

export enum SelectedBookingItemType {
	Desk,
	Room,
}

export interface SelectedDesk {
	id: number;
	displayNumber: number;
	spaceId: number | null;
	abbreviation: string;
	type: SelectedBookingItemType.Desk;
	isCanCanceled: boolean;
}

export interface SelectedRoom {
	id: number;
	name: string;
	type: SelectedBookingItemType.Room;
}

export type SelectedBookingItem = SelectedDesk | SelectedRoom;

export interface SelectedTime {
	from: Dayjs;
	to: Dayjs;
}

export interface BookingContextInitialValues {
	timelineDate: Dayjs;
	isAllDayChecked: boolean;
	selectedSpaceId: number | null;
	isWorkingHours: boolean;
	selectedItem: SelectedBookingItem | null;
	selectedTime: SelectedTime | null;
	firstDay: Dayjs | null;
	lastDay: Dayjs | null;
	employee: Employee | null;
	office: OfficeType;
	initialOffice: OfficeType | null;
	initialEmployee: Employee;
	highlightedDays: HighlightedDays;
	workplaces: Workplace[];
	error: null | string;
	selectedWorkplace?: Workplace;
	weeksCount: number;
	hasWorkplaceCollision: boolean | undefined;
	meetingRoomCollisions: Array<MeetingRoomsCollision>;
	officeWorkplaces: Array<WorkplaceInfo>;
	isTimelineDateSynchronized: boolean;
	isGuestMode: boolean;
	guestName: string | null;
	guestNameError: string | null;
	bookingUsersData: LookupUserResponse[] | undefined;
	isFetchingBookingUsers: boolean;
	activeTabIndex: number;
	itemToDelete: DeskBooking | MeetingRoomBooking | null;
}

export interface BookingContextValues extends BookingContextInitialValues {
	setGuestNameError: Dispatch<
		SetStateAction<SetGuestNameErrorAction['payload']>
	>;
	setGuestName: Dispatch<SetStateAction<SetGuestNameAction['payload']>>;
	setActiveTabIndex: Dispatch<
		SetStateAction<SetActiveTabIndexAction['payload']>
	>;
	setSelectMode: Dispatch<SetStateAction<BookingModes>>;
	setTimelineDate: Dispatch<SetStateAction<SetTimelineDateAction['payload']>>;
	setIsAllDayChecked: Dispatch<
		SetStateAction<SetAllDayCheckedAction['payload']>
	>;
	setIsWorkingHours: Dispatch<
		SetStateAction<SetIsWorkingHoursAction['payload']>
	>;
	setSelectedSpaceId: Dispatch<
		SetStateAction<SetSelectedSpaceIdAction['payload']>
	>;
	setSelectedItem: Dispatch<SetStateAction<SetSelectedItemAction['payload']>>;
	setItemToDelete: Dispatch<SetStateAction<SetItemToDeleteAction['payload']>>;
	setSelectedTime: Dispatch<SetStateAction<SetSelectedTimeAction['payload']>>;
	setFirstDay: Dispatch<SetStateAction<SetFirstDayAction['payload']>>;
	setLastDay: Dispatch<SetStateAction<SetLastDayAction['payload']>>;
	setEmployee: Dispatch<SetStateAction<SetEmployeeAction['payload']>>;
	setOffice: Dispatch<SetStateAction<SetOfficeAction['payload']>>;
	setHighlightedDays: Dispatch<
		SetStateAction<SetHighlightedDaysAction['payload']>
	>;
	setWorkplaces: Dispatch<SetStateAction<SetWorkplacesAction['payload']>>;
	setWeeksCount: Dispatch<SetStateAction<SetWeeksCountAction['payload']>>;
	setHasWorkplaceCollision: Dispatch<
		SetStateAction<SetHasWorkplaceCollisionAction['payload']>
	>;
	setError: Dispatch<SetStateAction<SetErrorAction['payload']>>;
	init: () => void;
	resetOnMeetingTabChange: Dispatch<
		SetStateAction<ResetOnMeetingRoomsTabChangeAction['payload']>
	>;
	setInitialTimelineDate: Dispatch<
		SetStateAction<SetInitialTimelineDateAction['payload']>
	>;
	setOfficeWorkplaces: Dispatch<
		SetStateAction<SetOfficeWorkplacesAction['payload']>
	>;

	areMeetingRoomDisabled: boolean;
	areMeetingRoomsSelected: boolean;
	isTimelineInRange: boolean;
	isBlockedDesk: boolean;
	isSelectedRoomAvailable: boolean;
	userHasRoomBookingsForTimelinePeriod: boolean;
	isSelectedMeetingRoomTimeInPast: boolean;
	areMeetingRoomsCollisionsLoading: boolean;
	bookingsState: BookingsState;
	isBookingsDataLoading: boolean;
}

export interface BookingProviderProps {
	children: ReactNode;
	initialBookingContextState: BookingContextInitialValues;
}

interface Action<T = undefined> {
	type: BookingActions;
	payload: T;
}
export interface ActionMap {
	[BookingActions.INIT]: undefined;
	[BookingActions.RESET_ON_MEETING_ROOMS_TAB_CHANGE]: Dayjs;
	[BookingActions.RESET_ON_OFFICE_CHANGE]: Dayjs;
	[BookingActions.SET_ALL_DAY_CHECKED]: boolean;
	[BookingActions.SET_EMPLOYEE]: Employee;
	[BookingActions.SET_ERROR]: string | null;
	[BookingActions.SET_GUEST_NAME]: string | null;
	[BookingActions.SET_GUEST_NAME_ERROR]: string | null;
	[BookingActions.SET_FIRST_DAY]: Dayjs;
	[BookingActions.SET_HAS_WORKPLACE_COLLISION]: boolean | undefined;
	[BookingActions.SET_HIGHLIGHTED_DAYS]: HighlightedDays;
	[BookingActions.SET_INITIAL_OFFICE]: OfficeType;
	[BookingActions.SET_INITIAL_TIMELINE_DATE]: Dayjs;
	[BookingActions.SET_IS_WORKING_HOURS]: boolean;
	[BookingActions.SET_LAST_DAY]: Dayjs;
	[BookingActions.SET_MEETING_ROOM_COLLISIONS]: Array<MeetingRoomsCollision>;
	[BookingActions.SET_OFFICE]: OfficeType;
	[BookingActions.SET_OFFICE_WORKPLACES]: Array<WorkplaceInfo>;
	[BookingActions.SET_SELECTED_ITEM]: SelectedBookingItem | null;
	[BookingActions.SET_SELECTED_SPACE_ID]: number | null;
	[BookingActions.SET_SELECTED_TIME]: SelectedTime | null;
	[BookingActions.SET_WEEKS_COUNT]: number;
	[BookingActions.SET_WORKPLACES]: Workplace[];
	[BookingActions.SET_TIMELINE_DATE]: Dayjs;
	[BookingActions.SET_ACTIVE_TAB_INDEX]: number;
}

export interface EquipmentContextValues {
	equipmentModel: MainEquipmentFormValues;
	setEquipmentModel: Dispatch<SetStateAction<MainEquipmentFormValues>>;
}

type AuthState = {
	token: string | null;
	role: Roles;
	isLoading?: boolean;
	handleReceivedToken?: HandleReceivedToken;
	onLogout?: (reason?: LogoutReasons) => void;
	setUserInfo?: Dispatch<SetStateAction<Partial<UserInfo>>>;
} & Partial<UserInfo>;

export type InitialAuthState = Omit<
	AuthState,
	'handleReceivedToken' | 'onLogout'
>;

export type AuthContextType = Omit<AuthState, 'loading'> & {
	setLogoutCallback?: (func: VoidFunction | null) => void;
	isUserInfoLoaded?: boolean;
};

export type HandleReceivedToken = (token: string) => void;

export enum SessionBroadcastEvents {
	requestSession = 'requestSession',
	sendSession = 'sendSession',
}

export type SessionBroadcastMessage = {
	type: string;
	data?: string;
};

export type BookingMapImagesLoadStatus = Record<string, boolean>;

export interface BookingMapContextValues {
	isMapOpen: boolean;
	setIsMapOpen: Dispatch<SetStateAction<boolean>>;
	isLoading: boolean;
	setIsLoading: Dispatch<SetStateAction<boolean>>;
	imagesLoadStatus: BookingMapImagesLoadStatus;
	resetImagesLoadStatus: VoidFunction;
	setImageLoaded: (key: string) => void;
	isImageLoaded: (key: string) => boolean;
}

interface SetGuestNameAction extends Action<string | null> {
	type: BookingActions.SET_GUEST_NAME;
}

interface SetActiveTabIndexAction extends Action<number> {
	type: BookingActions.SET_ACTIVE_TAB_INDEX;
}

interface SetGuestNameErrorAction extends Action<string | null> {
	type: BookingActions.SET_GUEST_NAME_ERROR;
}

interface SetTimelineDateAction extends Action<Dayjs> {
	type: BookingActions.SET_TIMELINE_DATE;
}

interface SetAllDayCheckedAction extends Action<boolean> {
	type: BookingActions.SET_ALL_DAY_CHECKED;
}

interface SetSelectedItemAction extends Action<SelectedBookingItem | null> {
	type: BookingActions.SET_SELECTED_ITEM;
}

interface SetItemToDeleteAction
	extends Action<DeskBooking | MeetingRoomBooking | null> {
	type: BookingActions.SET_ITEM_TO_DELETE;
}

interface SetIsWorkingHoursAction extends Action<boolean> {
	type: BookingActions.SET_IS_WORKING_HOURS;
}

interface SetSelectedSpaceIdAction extends Action<number | null> {
	type: BookingActions.SET_SELECTED_SPACE_ID;
}

interface SetFirstDayAction extends Action<Dayjs | null> {
	type: BookingActions.SET_FIRST_DAY;
}

interface SetSelectedTimeAction extends Action<SelectedTime | null> {
	type: BookingActions.SET_SELECTED_TIME;
}

interface SetLastDayAction extends Action<Dayjs | null> {
	type: BookingActions.SET_LAST_DAY;
}

interface SetEmployeeAction extends Action<Employee | null> {
	type: BookingActions.SET_EMPLOYEE;
}

interface SetOfficeAction extends Action<OfficeType> {
	type: BookingActions.SET_OFFICE;
}

interface SetInitialOfficeAction extends Action<OfficeType> {
	type: BookingActions.SET_INITIAL_OFFICE;
}

interface SetHighlightedDaysAction extends Action<HighlightedDays> {
	type: BookingActions.SET_HIGHLIGHTED_DAYS;
}

interface SetWorkplacesAction extends Action<Workplace[]> {
	type: BookingActions.SET_WORKPLACES;
}

interface SetErrorAction extends Action<string | null> {
	type: BookingActions.SET_ERROR;
}

interface ResetOnOfficeChangeAction extends Action<Dayjs> {
	type: BookingActions.RESET_ON_OFFICE_CHANGE;
}

interface ResetOnMeetingRoomsTabChangeAction extends Action<Dayjs> {
	type: BookingActions.RESET_ON_MEETING_ROOMS_TAB_CHANGE;
}

interface InitAction extends Action {
	type: BookingActions.INIT;
}

interface SetWeeksCountAction extends Action<number> {
	type: BookingActions.SET_WEEKS_COUNT;
}

interface SetMeetingRoomCollisionsAction
	extends Action<Array<MeetingRoomsCollision>> {
	type: BookingActions.SET_MEETING_ROOM_COLLISIONS;
}

interface SetHasWorkplaceCollisionAction extends Action<boolean | undefined> {
	type: BookingActions.SET_HAS_WORKPLACE_COLLISION;
}

interface SetInitialTimelineDateAction extends Action<Dayjs> {
	type: BookingActions.SET_INITIAL_TIMELINE_DATE;
}

interface SetOfficeWorkplacesAction extends Action<Array<WorkplaceInfo>> {
	type: BookingActions.SET_OFFICE_WORKPLACES;
}
export type BookingAction =
	| SetGuestNameErrorAction
	| SetGuestNameAction
	| SetTimelineDateAction
	| SetAllDayCheckedAction
	| SetSelectedItemAction
	| SetItemToDeleteAction
	| SetIsWorkingHoursAction
	| SetSelectedSpaceIdAction
	| SetFirstDayAction
	| SetSelectedTimeAction
	| SetLastDayAction
	| SetEmployeeAction
	| SetOfficeAction
	| SetInitialOfficeAction
	| SetHighlightedDaysAction
	| SetWorkplacesAction
	| SetErrorAction
	| ResetOnOfficeChangeAction
	| ResetOnMeetingRoomsTabChangeAction
	| InitAction
	| SetWeeksCountAction
	| SetMeetingRoomCollisionsAction
	| SetHasWorkplaceCollisionAction
	| SetInitialTimelineDateAction
	| SetOfficeWorkplacesAction
	| SetActiveTabIndexAction;
