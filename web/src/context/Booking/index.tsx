import {
	createContext,
	useReducer,
	useMemo,
	useCallback,
	useEffect,
	useContext,
	ComponentType,
	useState,
} from 'react';
import { useTranslation } from 'react-i18next';

import { useAuth } from '../AuthContext';
import { useConfiguration } from '../Configuration';
import { useToast } from '../Toast';
import {
	BookingProviderProps,
	type BookingContextInitialValues,
	type BookingContextValues,
	Employee,
	SelectedBookingItemType,
	BookingAction,
	ActionMap,
} from '../types';

import { ALL_DAY_TIME } from '@/constants/booking';
import {
	BookingActions,
	initialBookingState,
} from '@/constants/bookingContext';
import { NotificationType } from '@/constants/notifications';
import { Roles } from '@/constants/roles';
import { useBookingList } from '@/context/Booking/hooks/useBookingList';
import {
	areMeetingRoomsOpen,
	getRequestBookingDateTime,
	getRequestBookingDate,
} from '@/helpers/bookingTime';
import { createToast } from '@/helpers/createToast';
import { getFullName } from '@/helpers/formatName';
import { getInitialBookingOfficeByUserLocation } from '@/helpers/getInitialBookingOfficeByUserLocation';
import { getMeetingRoomBookedForPeriod } from '@/helpers/getToastMessage';
import { isMeetingRoomTimeInPast } from '@/helpers/isMeetingRoomTimeInPast';
import dayjs, { isDayjs } from '@/lib/dayjs';
import { useCheckMeetingRoomsCollisions } from '@/queries/booking/useCheckMeetingRoomsCollisions';
import { useUsersLookup } from '@/queries/useUsersLookup';
import { BookingModes } from '@/types/booking';
import { OfficeType } from '@/types/configuration';

const BookingContext = createContext({} as BookingContextValues);

const defaultTimeInPast = false;

const {
	SET_SELECTED_ITEM,
	SET_FIRST_DAY,
	SET_LAST_DAY,
	SET_EMPLOYEE,
	SET_OFFICE,
	SET_INITIAL_OFFICE,
	SET_HIGHLIGHTED_DAYS,
	SET_WORKPLACES,
	SET_ERROR,
	SET_GUEST_NAME,
	SET_GUEST_NAME_ERROR,
	RESET_ON_OFFICE_CHANGE,
	INIT,
	SET_WEEKS_COUNT,
	SET_SELECTED_SPACE_ID,
	SET_SELECTED_TIME,
	SET_IS_WORKING_HOURS,
	SET_ALL_DAY_CHECKED,
	SET_TIMELINE_DATE,
	RESET_ON_MEETING_ROOMS_TAB_CHANGE,
	SET_MEETING_ROOM_COLLISIONS,
	SET_HAS_WORKPLACE_COLLISION,
	SET_INITIAL_TIMELINE_DATE,
	SET_OFFICE_WORKPLACES,
	SET_ACTIVE_TAB_INDEX,
	SET_ITEM_TO_DELETE,
} = BookingActions;

const reducer =
	(initState: BookingContextInitialValues) =>
	(
		state: BookingContextInitialValues,
		{ type, payload }: BookingAction
	): BookingContextInitialValues => {
		switch (type) {
			case SET_GUEST_NAME_ERROR:
				return { ...state, guestNameError: payload };
			case SET_IS_WORKING_HOURS:
				return { ...state, isWorkingHours: payload };
			case SET_ALL_DAY_CHECKED:
				return { ...state, isAllDayChecked: payload };
			case SET_SELECTED_ITEM:
				return {
					...state,
					selectedItem: payload,
				};
			case SET_SELECTED_SPACE_ID:
				return { ...state, selectedSpaceId: payload };
			case SET_FIRST_DAY:
				return { ...state, firstDay: payload };
			case SET_TIMELINE_DATE:
				return { ...state, timelineDate: payload };
			case SET_LAST_DAY:
				return { ...state, lastDay: payload };
			case SET_SELECTED_TIME: {
				return {
					...state,
					selectedTime: payload,
				};
			}
			case SET_EMPLOYEE:
				return { ...state, employee: payload };
			case SET_OFFICE:
				return {
					...state,
					office: payload,
				};
			case SET_INITIAL_OFFICE:
				return { ...state, initialOffice: payload };
			case SET_MEETING_ROOM_COLLISIONS:
				return {
					...state,
					meetingRoomCollisions: payload,
				};
			case RESET_ON_OFFICE_CHANGE: {
				if (isDayjs(payload)) {
					return {
						...initState,
						office: state.office,
						initialOffice: state.initialOffice,
						initialEmployee: state.initialEmployee,
						officeWorkplaces: state.officeWorkplaces,
						timelineDate: payload.clone(),
						firstDay: payload.clone(),
						lastDay: payload.clone(),
					};
				}

				return {
					...initState,
					office: state.office,
					initialOffice: state.initialOffice,
					initialEmployee: state.initialEmployee,
					officeWorkplaces: state.officeWorkplaces,
				};
			}
			case RESET_ON_MEETING_ROOMS_TAB_CHANGE: {
				if (isDayjs(payload)) {
					return {
						...state,
						isWorkingHours: true,
						isAllDayChecked: false,
						weeksCount: 0,
						selectedItem: null,
						selectedTime: null,
						isTimelineDateSynchronized: false,
						timelineDate: payload.clone(),
						firstDay: payload.clone(),
						lastDay: payload.clone(),
					};
				}

				return {
					...state,
					firstDay: initState.firstDay,
					lastDay: initState.lastDay,
					timelineDate: dayjs().startOf('day'),
					isWorkingHours: true,
					isAllDayChecked: false,
					weeksCount: 0,
					selectedItem: null,
					selectedTime: null,
					isTimelineDateSynchronized: false,
					meetingRoomCollisions: [],
				};
			}
			case SET_INITIAL_TIMELINE_DATE: {
				if (isDayjs(payload)) {
					return {
						...state,
						timelineDate: payload.clone(),
						firstDay: payload.clone(),
						lastDay: payload.clone(),
						isTimelineDateSynchronized: true,
					};
				}

				return state;
			}
			case SET_HIGHLIGHTED_DAYS:
				return {
					...state,
					highlightedDays: payload,
				};
			case SET_WORKPLACES:
				return {
					...state,
					workplaces: payload,
				};
			case SET_ERROR:
				return {
					...state,
					error: payload,
				};
			case SET_GUEST_NAME:
				return {
					...state,
					guestName: payload,
				};
			case SET_OFFICE_WORKPLACES:
				return {
					...state,
					officeWorkplaces: payload,
				};

			case INIT:
				return {
					...state,
					selectedItem: null,
					employee: state.initialEmployee,
				};
			case SET_WEEKS_COUNT:
				return { ...state, weeksCount: payload };
			case SET_ACTIVE_TAB_INDEX:
				return { ...state, activeTabIndex: payload };
			case SET_HAS_WORKPLACE_COLLISION:
				return {
					...state,
					hasWorkplaceCollision: payload,
				};
			case SET_ITEM_TO_DELETE:
				return { ...state, itemToDelete: payload };
			default:
				return state;
		}
	};

const BOOKING_USERS_QUERY_PARAMS = {
	search: '',
	isActive: true,
};

function BookingProvider({
	children,
	initialBookingContextState,
}: BookingProviderProps) {
	const [
		{
			meetingRoomCollisions,
			timelineDate,
			isAllDayChecked,
			isWorkingHours,
			selectedTime,
			selectedItem,
			selectedSpaceId,
			firstDay,
			lastDay,
			employee,
			office,
			initialOffice,
			highlightedDays,
			workplaces,
			initialEmployee,
			error,
			hasWorkplaceCollision,
			weeksCount,
			officeWorkplaces,
			isTimelineDateSynchronized,
			guestName,
			guestNameError,
			activeTabIndex,
			itemToDelete,
		},
		dispatch,
	] = useReducer(
		reducer(initialBookingContextState),
		initialBookingContextState
	);

	const toast = useToast();

	const callback: any = useCallback(
		<T extends keyof ActionMap>(type: T) =>
			(payload: ActionMap[T]) => {
				dispatch({ type, payload } as BookingAction);
			},
		[]
	);

	const selectedWorkplace = useMemo(
		() => workplaces.find((place) => place.workplaceId === selectedItem?.id),
		[workplaces, selectedItem]
	);

	const { id: userId, location, role } = useAuth();
	const { t } = useTranslation('booking');
	const {
		configurableValues: { offices },
	} = useConfiguration();

	const [selectMode, setSelectMode] = useState(BookingModes.Employees);
	const isGuestMode = useMemo(
		() => selectMode === BookingModes.Guest,
		[selectMode]
	);

	useEffect(() => {
		callback(SET_OFFICE)(
			getInitialBookingOfficeByUserLocation(offices, location)
		);
	}, [location]);

	useEffect(() => {
		const officeDate = dayjs().tz(office.timeZone);
		callback(RESET_ON_OFFICE_CHANGE)(officeDate);
		const hasSpaces = office.workspaces.length > 0;
		callback(SET_SELECTED_SPACE_ID)(hasSpaces ? office.workspaces[0].id : null);
	}, [office]);

	const isTimelineInRange = useMemo(
		() => timelineDate?.isBetween(firstDay, lastDay, 'day', '[]'),
		[timelineDate, firstDay, lastDay]
	);

	const areMeetingRoomsSelected = useMemo(
		() => areMeetingRoomsOpen(selectedSpaceId),
		[selectedSpaceId]
	);

	const areMeetingRoomDisabled = useMemo(() => {
		if (areMeetingRoomsSelected) {
			return (
				!selectedTime || meetingRoomCollisions.length > 0 || !isTimelineInRange
			);
		}

		return false;
	}, [
		areMeetingRoomsSelected,
		selectedTime,
		meetingRoomCollisions,
		isTimelineInRange,
	]);

	const isBlockedDesk = useMemo(() => {
		if (!selectedItem) {
			return false;
		}

		return workplaces.some(
			(workplace) =>
				workplace.workplaceId === selectedItem.id && workplace.isBlocked
		);
	}, [selectedItem]);

	const userHasRoomBookingsForTimelinePeriod = useMemo(() => {
		const currentTimelineCollisions = meetingRoomCollisions.some(
			(collision) => employee?.id === collision.userId
		);

		return currentTimelineCollisions;
	}, [meetingRoomCollisions, employee, timelineDate]);

	const isSelectedMeetingRoomTimeInPast = useMemo(() => {
		if (selectedTime) {
			const isInPast = isMeetingRoomTimeInPast(
				timelineDate,
				selectedTime,
				isAllDayChecked,
				office.timeZone
			);

			return isInPast;
		}

		return defaultTimeInPast;
	}, [timelineDate, selectedTime, isAllDayChecked, office.timeZone]);

	const timezone = office.timeZone;

	const { isFetching } = useCheckMeetingRoomsCollisions(
		{
			ReservationFromUtc:
				firstDay && selectedTime && timezone
					? getRequestBookingDateTime(
							firstDay,
							selectedTime.from,
							isAllDayChecked,
							ALL_DAY_TIME.from,
							timezone
					  )
					: undefined,
			ReservationToUtc:
				firstDay && selectedTime && timezone
					? getRequestBookingDateTime(
							firstDay,
							selectedTime.to,
							isAllDayChecked,
							ALL_DAY_TIME.to,
							timezone
					  )
					: undefined,
			ReservationPeriodFromOfficeDate: firstDay
				? getRequestBookingDate(firstDay)
				: undefined,
			ReservationPeriodToOfficeDate: lastDay
				? getRequestBookingDate(lastDay)
				: undefined,
			meetingRoomId: selectedItem?.id,
			RepeatReservationWeeks: weeksCount || null,
			enabled:
				isTimelineInRange &&
				areMeetingRoomsSelected &&
				!isSelectedMeetingRoomTimeInPast,
			UserId: employee?.id ?? userId,
		},
		{
			onSuccess: (collisions) => {
				callback(SET_MEETING_ROOM_COLLISIONS)(collisions);

				const selectedRoomCollision = collisions.find(
					(collision) => collision.meetingRoomId === selectedItem?.id
				);

				if (
					weeksCount > 0 &&
					collisions.length > 0 &&
					selectedItem?.type === SelectedBookingItemType.Room &&
					selectedRoomCollision
				) {
					toast.open(
						createToast(
							NotificationType.INFO,
							getMeetingRoomBookedForPeriod(selectedRoomCollision, t)
						)
					);
				}
			},
			onError: () => {
				callback(SET_MEETING_ROOM_COLLISIONS)([]);
			},
			initialData: [],
		}
	);

	const { bookingsState, isBookingsDataLoading } = useBookingList({ office });

	const isSelectedRoomAvailable = useMemo(() => {
		if (!areMeetingRoomsSelected) {
			return false;
		}

		if (isFetching) {
			return false;
		}

		const isRoomAvailable = !meetingRoomCollisions.some(
			(collision) => collision.meetingRoomId === selectedItem?.id
		);

		return isRoomAvailable;
	}, [
		meetingRoomCollisions,
		selectedItem,
		areMeetingRoomsSelected,
		isFetching,
	]);

	useEffect(() => {
		const firstCollision = meetingRoomCollisions[0];

		if (
			firstCollision &&
			firstCollision.userId === employee?.id &&
			firstCollision.meetingRoomId !== selectedItem?.id
		) {
			toast.open(
				createToast(NotificationType.INFO, t('toasts.alreadyHaveBooking'))
			);
		}
	}, [meetingRoomCollisions]);

	const { data: bookingUsersData, isFetching: isFetchingBookingUsers } =
		useUsersLookup({
			...BOOKING_USERS_QUERY_PARAMS,
			queryOptions: { enabled: role !== Roles.SE },
		});

	const value: BookingContextValues = useMemo(
		() => ({
			meetingRoomCollisions: isSelectedMeetingRoomTimeInPast
				? []
				: meetingRoomCollisions,
			isAllDayChecked,
			isWorkingHours,
			selectedTime,
			selectedSpaceId,
			selectedItem,
			firstDay,
			lastDay,
			employee,
			office,
			initialOffice,
			initialEmployee,
			highlightedDays,
			workplaces,
			error,
			weeksCount,
			selectedWorkplace,
			timelineDate,
			hasWorkplaceCollision,
			setGuestNameError: callback(SET_GUEST_NAME_ERROR),
			setIsAllDayChecked: callback(SET_ALL_DAY_CHECKED),
			setIsWorkingHours: callback(SET_IS_WORKING_HOURS),
			setSelectedItem: callback(SET_SELECTED_ITEM),
			setTimelineDate: callback(SET_TIMELINE_DATE),
			setSelectedSpaceId: callback(SET_SELECTED_SPACE_ID),
			setFirstDay: callback(SET_FIRST_DAY),
			setLastDay: callback(SET_LAST_DAY),
			setSelectedTime: callback(SET_SELECTED_TIME),
			setEmployee: callback(SET_EMPLOYEE),
			setOffice: callback(SET_OFFICE),
			setHighlightedDays: callback(SET_HIGHLIGHTED_DAYS),
			setWorkplaces: callback(SET_WORKPLACES),
			setWeeksCount: callback(SET_WEEKS_COUNT),
			setHasWorkplaceCollision: callback(SET_HAS_WORKPLACE_COLLISION),
			setError: callback(SET_ERROR),
			setGuestName: callback(SET_GUEST_NAME),
			init: callback(INIT),
			resetOnMeetingTabChange: callback(RESET_ON_MEETING_ROOMS_TAB_CHANGE),
			setInitialTimelineDate: callback(SET_INITIAL_TIMELINE_DATE),
			officeWorkplaces,
			setOfficeWorkplaces: callback(SET_OFFICE_WORKPLACES),
			setActiveTabIndex: callback(SET_ACTIVE_TAB_INDEX),
			areMeetingRoomDisabled,
			areMeetingRoomsSelected,
			isTimelineInRange,
			isBlockedDesk,
			userHasRoomBookingsForTimelinePeriod,
			isSelectedRoomAvailable,
			isSelectedMeetingRoomTimeInPast,
			isTimelineDateSynchronized,
			areMeetingRoomsCollisionsLoading: isFetching,
			setSelectMode,
			isGuestMode,
			guestName,
			guestNameError,
			bookingUsersData,
			isFetchingBookingUsers,
			activeTabIndex,
			bookingsState,
			isBookingsDataLoading,
			setItemToDelete: callback(SET_ITEM_TO_DELETE),
			itemToDelete,
		}),
		[
			meetingRoomCollisions,
			isAllDayChecked,
			isWorkingHours,
			selectedTime,
			selectedSpaceId,
			selectedItem,
			firstDay,
			lastDay,
			employee,
			office,
			initialOffice,
			initialEmployee,
			highlightedDays,
			workplaces,
			error,
			weeksCount,
			selectedWorkplace,
			timelineDate,
			areMeetingRoomDisabled,
			areMeetingRoomsSelected,
			isTimelineInRange,
			hasWorkplaceCollision,
			isBlockedDesk,
			userHasRoomBookingsForTimelinePeriod,
			isSelectedRoomAvailable,
			isSelectedMeetingRoomTimeInPast,
			officeWorkplaces,
			isTimelineDateSynchronized,
			isFetching,
			selectMode,
			isGuestMode,
			guestName,
			guestNameError,
			bookingUsersData,
			isFetchingBookingUsers,
			activeTabIndex,
			bookingsState,
			isBookingsDataLoading,
			itemToDelete,
		]
	);

	return (
		<BookingContext.Provider value={value}>{children}</BookingContext.Provider>
	);
}

export const useBookingContext = () => useContext(BookingContext);

export const withBookingProvider = (Component: ComponentType<any>) =>
	function WithBookingProvider() {
		const { id, firstName, lastName, location, rightPosition } = useAuth();

		const {
			configurableValues: { offices },
		} = useConfiguration();

		const initialOfficeValue: OfficeType | null = useMemo(
			() => getInitialBookingOfficeByUserLocation(offices, location),
			[offices, location]
		);

		const initialSpaceId: number | null = useMemo(() => {
			const hasSpaces = initialOfficeValue
				? initialOfficeValue.workspaces.length > 0
				: false;

			return hasSpaces && initialOfficeValue
				? initialOfficeValue.workspaces[0].id
				: null;
		}, [initialOfficeValue]);

		const initialEmployeeValue: Employee | null = useMemo(() => {
			if (!id || !firstName || !lastName) {
				return null;
			}

			return {
				id,
				name: getFullName(firstName, lastName),
				positionDescription: rightPosition,
			};
		}, [id, firstName, lastName, rightPosition]);

		if (!initialOfficeValue || !initialEmployeeValue) {
			return null;
		}

		const initialBookingContextState: BookingContextInitialValues = {
			...initialBookingState,
			office: initialOfficeValue,
			selectedSpaceId: initialSpaceId,
			initialOffice: initialOfficeValue,
			initialEmployee: initialEmployeeValue,
			employee: initialEmployeeValue,
		};

		return (
			<BookingProvider initialBookingContextState={initialBookingContextState}>
				<Component />
			</BookingProvider>
		);
	};
