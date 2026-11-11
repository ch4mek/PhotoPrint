import type { Dayjs } from '@/lib/dayjs';
import { FormAutoselectOption, FormUserAutoselectOption } from '@/types/forms';
import { ProjectLookupResponse } from '@/types/project';

export interface ProjectAssignFormValues {
	user: FormUserAutoselectOption | null;
	project: ProjectLookupResponse | null;
	startDate: Dayjs | null;
	endDate: Dayjs | null;
	projectSeniority: FormAutoselectOption<string> | null;
	fte: string;
	billability: boolean;
}

export type ProjectAssignFormInitialValues = Partial<{
	[K in keyof ProjectAssignFormValues]: {
		value: ProjectAssignFormValues[K];
		disabled?: boolean;
	};
}>;

export interface ProjectAssignFormOptions {
	users: Array<FormUserAutoselectOption>;
	projects: Array<ProjectLookupResponse>;
}

export interface ProjectAssignFormProps {
	options: ProjectAssignFormOptions;
	initialValues?: ProjectAssignFormInitialValues;
	handleClose: () => void;
	onSubmit: (values: ProjectAssignFormValues) => void;
	isLoading?: boolean;
	label: string;
	validationSchema: any;
	projectSubValueField?: string;
	isAssignEditable: boolean;
}

export type ProjectAssignFormDisableFlags = Record<
	keyof ProjectAssignFormValues,
	boolean
>;

export enum ProjectAccessFormFields {
	PROJECT = 'project',
	USER = 'user',
	START_DATE = 'startDate',
	END_DATE = 'endDate',
	PROJECT_SENIORITY = 'projectSeniority',
	BILLABILITY = 'billability',
	FTE = 'fte',
}
