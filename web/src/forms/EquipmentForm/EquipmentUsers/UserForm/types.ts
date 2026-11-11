import { EquipmentHistoryData } from '../History/types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { Dayjs } from '@/lib/dayjs';
import { Approver } from '@/types/equipment';

export interface UserEquipmentFormProps {
	formik: FormikWithBackupRenderProps<InitUserFilterState>;
	isBrokenStatus: boolean;
	isEquipmentAssigned: boolean;
	isEquipmentReturned: boolean;
	equipmentHistoryProps: EquipmentHistoryData;
}

interface AssignedUser {
	id: number;
	fullName: string;
	departmentId: string | null;
	resourceGroupId: number | null;
}

export interface InitUserFilterState {
	id: number;
	approver: null | Approver;
	selectedUser: null | AssignedUser;
	issueDate: Dayjs | null;
	returnDate?: Dayjs | null;
}
