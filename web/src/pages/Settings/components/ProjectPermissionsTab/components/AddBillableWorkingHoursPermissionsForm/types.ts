import { Dispatch, SetStateAction } from 'react';

import { ProjectType } from '@/types/project';

export interface ProjectOption {
	id: number;
	client?: string;
	name: string;
	type?: ProjectType;
}

export interface AddBillableWorkingHoursPermissionsFormProps {
	setIsOpenForm: Dispatch<SetStateAction<boolean>>;
}

interface UserOption {
	id: number;
	name: string;
	departmentId?: number;
}

export interface BillableWorkingHoursPermissionsFormValues {
	user: UserOption | null;
	projects: ProjectOption[];
}
