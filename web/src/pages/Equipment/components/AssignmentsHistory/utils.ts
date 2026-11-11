import { FromToProps } from '@/components/Grid';
import { Departments } from '@/constants/departments';
import { isHeadOrGroupManager, Roles } from '@/constants/roles';

import {
	AssignmentHistoryFilters,
	AssignmentsHistoryReducerType,
	SetAssignmentsHistoryFiltersType,
	AssignmentsHistoryActionType,
} from './types';

export const createSetAssignmentsHistoryFilters = (
	dispatch: React.Dispatch<AssignmentsHistoryReducerType>
): SetAssignmentsHistoryFiltersType => ({
	reset: (payload: AssignmentHistoryFilters) =>
		dispatch({ type: AssignmentsHistoryActionType.RESET, payload }),
	userId: (payload: string) =>
		dispatch({ type: AssignmentsHistoryActionType.USER_ID, payload }),
	equipmentId: (payload: string) =>
		dispatch({ type: AssignmentsHistoryActionType.EQUIPMENT_ID, payload }),
	availability: (payload?: string[]) =>
		dispatch({ type: AssignmentsHistoryActionType.IS_ASSIGNED, payload }),
	serialNumber: (payload: string) =>
		dispatch({ type: AssignmentsHistoryActionType.SERIAL_NUMBER, payload }),
	inventoryNumber: (payload: string) =>
		dispatch({ type: AssignmentsHistoryActionType.INVENTORY_NUMBER, payload }),
	name: (payload: string) =>
		dispatch({ type: AssignmentsHistoryActionType.EQUIPMENT_NAME, payload }),
	typeId: (payload?: string[]) =>
		dispatch({ type: AssignmentsHistoryActionType.EQUIPMENT_TYPE, payload }),
	userFullName: (payload?: string) =>
		dispatch({ type: AssignmentsHistoryActionType.USER_FULL_NAME, payload }),
	departments: (payload?: string[]) =>
		dispatch({ type: AssignmentsHistoryActionType.DEPARTMENTS, payload }),
	issueDate: (payload: FromToProps<Date>) =>
		dispatch({ type: AssignmentsHistoryActionType.ISSUE_DATE, payload }),
	returnDate: (payload: FromToProps<Date>) =>
		dispatch({ type: AssignmentsHistoryActionType.RETURN_DATE, payload }),
});

export const assignmentsHistoryReducer = (
	state: AssignmentHistoryFilters,
	{ type, payload }: AssignmentsHistoryReducerType
): AssignmentHistoryFilters => {
	switch (type) {
		case AssignmentsHistoryActionType.RESET: {
			return { ...(payload as AssignmentHistoryFilters) };
		}
		case AssignmentsHistoryActionType.USER_ID: {
			return { ...state, userId: payload as string };
		}
		case AssignmentsHistoryActionType.EQUIPMENT_ID: {
			return { ...state, equipmentId: payload as string };
		}
		case AssignmentsHistoryActionType.USER_FULL_NAME: {
			return { ...state, userFullName: payload as string };
		}
		case AssignmentsHistoryActionType.IS_ASSIGNED: {
			return { ...state, isAssigned: payload as string[] };
		}
		case AssignmentsHistoryActionType.DEPARTMENTS: {
			return { ...state, departments: payload as string[] };
		}
		case AssignmentsHistoryActionType.EQUIPMENT_NAME: {
			return { ...state, equipmentName: payload as string };
		}
		case AssignmentsHistoryActionType.EQUIPMENT_TYPE: {
			return { ...state, type: payload as string[] };
		}

		case AssignmentsHistoryActionType.SERIAL_NUMBER: {
			return { ...state, serialNumber: payload as string };
		}
		case AssignmentsHistoryActionType.INVENTORY_NUMBER: {
			return { ...state, inventoryNumber: payload as string };
		}

		case AssignmentsHistoryActionType.ISSUE_DATE: {
			return {
				...state,
				issueDate: {
					...state.issueDate,
					...(payload as FromToProps<Date>),
				},
			};
		}
		case AssignmentsHistoryActionType.RETURN_DATE: {
			return {
				...state,
				returnDate: {
					...state.returnDate,
					...(payload as FromToProps<Date>),
				},
			};
		}

		default:
			return state;
	}
};

export const getDepartmentsDisabledOptionProperty = (
	role: Roles,
	roleDepartmentId: string,
	departmentOptionId: string,
	hasManageAllEquipmentClaim: boolean = false
) => {
	const isUserHeadOrGM = isHeadOrGroupManager(role);

	if (
		(isUserHeadOrGM && roleDepartmentId === Departments.SysAdmin) ||
		roleDepartmentId === Departments.Administration
	) {
		return false;
	}

	return (
		isUserHeadOrGM &&
		!hasManageAllEquipmentClaim &&
		roleDepartmentId !== departmentOptionId
	);
};
