import { create } from 'zustand';

import { defaultMiddleware } from '../../defaultMiddleware';

import { DEFAULT_ATTENDANCE_DETAILED_FILTERS } from '@/pages/Attendance/components/DetailedTab/constants';

import { AttendanceDetailedFilterStore } from './types';

const ATTENDANCE_DETAILED_STORE_TITLE = 'attendanceDetailedStore';

export const useAttendanceDetailedStore =
	create<AttendanceDetailedFilterStore>()(
		defaultMiddleware((set, get) => ({
			...DEFAULT_ATTENDANCE_DETAILED_FILTERS,
			initialFilters: DEFAULT_ATTENDANCE_DETAILED_FILTERS,

			setId: (id) =>
				set({ id }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setId`),
			setUser: (user) =>
				set({ user }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setUser`),
			setDepartments: (departments) =>
				set(
					{ departments },
					false,
					`${ATTENDANCE_DETAILED_STORE_TITLE}/setDepartments`
				),
			setGroup: (group) =>
				set({ group }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setGroup`),
			setDirection: (direction) =>
				set(
					{ direction },
					false,
					`${ATTENDANCE_DETAILED_STORE_TITLE}/setDirection`
				),
			setDoor: (door) =>
				set({ door }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setDoor`),
			setLocation: (location) =>
				set(
					{ location },
					false,
					`${ATTENDANCE_DETAILED_STORE_TITLE}/setLocation`
				),
			setDate: (date) =>
				set({ date }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setDate`),
			setTime: (time) =>
				set({ time }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setTime`),

			setAll: (filters) =>
				set({ ...filters }, false, `${ATTENDANCE_DETAILED_STORE_TITLE}/setAll`),

			reset: () => {
				const { initialFilters } = get();
				set(
					{ ...initialFilters },
					false,
					`${ATTENDANCE_DETAILED_STORE_TITLE}/reset`
				);
			},

			initialize: (initialFilters) =>
				set(
					{
						...initialFilters,
						initialFilters,
					},
					false,
					`${ATTENDANCE_DETAILED_STORE_TITLE}/initialize`
				),
		}))
	);

useAttendanceDetailedStore.devtools?.cleanup();
