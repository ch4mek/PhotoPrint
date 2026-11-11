import { FromToProps } from '@/components/Grid';
import { AttendanceDetailedFilters } from '@/pages/Attendance/components/DetailedTab/types';
import { AttendenceDirection } from '@/types/attendances';

export interface AttendanceDetailedFilterStore
	extends AttendanceDetailedFilters {
	initialFilters: AttendanceDetailedFilters;

	setId: (id: string) => void;
	setUser: (user: string) => void;
	setDepartments: (departments: string[]) => void;
	setGroup: (group: string) => void;
	setDirection: (direction: AttendenceDirection[]) => void;
	setDoor: (door: string) => void;
	setLocation: (location: string) => void;
	setDate: (date: FromToProps<Date>) => void;
	setTime: (time: FromToProps<string>) => void;
	setAll: (filters: AttendanceDetailedFilters) => void;
	reset: () => void;

	initialize: (initialFilters: AttendanceDetailedFilters) => void;
}
