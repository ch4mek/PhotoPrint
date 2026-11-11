import { ByPageResponse } from '@/queries/types';

export enum AttendenceDirection {
	Exit = 'Exit',
	Enter = 'Enter',
}

interface AttendanceDetailedData {
	id: number;
	userFullName: string;
	userFullNameRu: string;
	userDepartmentId: string | null;
	userResourceGroupId: number | null;
	userResourceGroupName: string | null;
	isUserActive: boolean;
	direction: AttendenceDirection;
	door: string;
	location: string;
	checkedAtUtc: string;
}

interface AttendanceTotalData {
	userId: string;
	userFirstName: string;
	userLastName: string;
	userFirstNameRu: string | null;
	userLastNameRu: string | null;
	userFullName: string;
	userDepartmentId: string | null;
	userResourceGroupId: number | null;
	userResourceGroupName: string | null;
	date: string;
	totalTimeInMinutes: number;
	isUserActive: boolean;
}

export type GetAttendancesDetailedResponse =
	ByPageResponse<AttendanceDetailedData>;
export type GetAttendancesTotalResponse = ByPageResponse<AttendanceTotalData>;
