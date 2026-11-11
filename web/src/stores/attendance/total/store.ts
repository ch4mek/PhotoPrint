import { create } from 'zustand';

import { defaultMiddleware } from '../../defaultMiddleware';

import { DEFAULT_ATTENDANCE_TOTAL_FILTERS } from '@/pages/Attendance/components/TotalTab/constants';

import { AttendanceTotalFilterStore } from './types';

const ATTENDANCE_TOTAL_STORE_TITLE = 'attendanceTotalStore';

export const useAttendanceTotalStore = create<AttendanceTotalFilterStore>()(
	defaultMiddleware((set, get) => ({
		...DEFAULT_ATTENDANCE_TOTAL_FILTERS,
		initialFilters: DEFAULT_ATTENDANCE_TOTAL_FILTERS,

		setUserId: (userId) =>
			set({ userId }, false, `${ATTENDANCE_TOTAL_STORE_TITLE}/setId`),
		setUser: (user) =>
			set({ user }, false, `${ATTENDANCE_TOTAL_STORE_TITLE}/setUser`),
		setDepartments: (departments) =>
			set(
				{ departments },
				false,
				`${ATTENDANCE_TOTAL_STORE_TITLE}/setDepartments`
			),
		setGroup: (group) =>
			set({ group }, false, `${ATTENDANCE_TOTAL_STORE_TITLE}/setGroup`),
		setDate: (date) =>
			set({ date }, false, `${ATTENDANCE_TOTAL_STORE_TITLE}/setDate`),
		setTime: (time) =>
			set({ time }, false, `${ATTENDANCE_TOTAL_STORE_TITLE}/setTime`),

		setAll: (filters) =>
			set({ ...filters }, false, `${ATTENDANCE_TOTAL_STORE_TITLE}/setAll`),

		reset: () => {
			const { initialFilters } = get();
			set(
				{ ...initialFilters },
				false,
				`${ATTENDANCE_TOTAL_STORE_TITLE}/reset`
			);
		},

		initialize: (initialFilters) =>
			set(
				{
					...initialFilters,
					initialFilters,
				},
				false,
				`${ATTENDANCE_TOTAL_STORE_TITLE}/initialize`
			),
	}))
);

useAttendanceTotalStore.devtools?.cleanup();
