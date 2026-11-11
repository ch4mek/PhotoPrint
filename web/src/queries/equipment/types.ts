import { SortDirection } from '@/constants/sort';
import { ToastContextValues } from '@/context/types';
import { Dayjs } from '@/lib/dayjs';
import type { EquipmentItemRequest } from '@/types/equipment';

export interface UpdateMutationVariables
	extends Omit<EquipmentItemRequest, 'id' | 'status'> {
	brokenStatus: string;
	equipmentId: string;
}

export interface UpdateActiveHolderVariables {
	equipmentId: string;
	approverId: number;
	userId: number;
	issueDate: Dayjs;
}

export interface DeleteActiveHolderVariables {
	equipmentId: string;
	returnDate: Dayjs;
}

export interface EquipmentUsersLookupResponse {
	id: number;
	firstName: string;
	lastName: string;
	positionDescription: string;
	departmentId: string | null;
	resourceGroupId: number | null;
}

export interface UseGetEquipmentHistoryParams {
	toast: ToastContextValues;
	id: string;
	page: number;
	rowsPerPage: number;
	isDeletedFilter?: boolean;
	orderBy?: SortDirection;
	enabled?: boolean;
}
