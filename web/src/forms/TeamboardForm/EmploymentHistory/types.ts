import { EmploymentHistoryResponse } from '@/types/employmentHistory';

export interface EmploymentHistoryFormProps {
	handleClose: () => void;
	onSubmit: (values: UpdateEmploymentHistory) => Promise<object | undefined>;
	id: number;
	initialValues: EmploymentHistoryFormValues;
	shouldDisplayNda?: boolean;
}

export type EmploymentHistoryFormValues = Pick<
	EmploymentHistoryResponse,
	| 'professionalActivityStartDate'
	| 'hireDate'
	| 'fireDate'
	| 'ndaStartDate'
	| 'ndaEndDate'
>;

export interface UpdateEmploymentHistory extends EmploymentHistoryFormValues {
	userId: number;
}
