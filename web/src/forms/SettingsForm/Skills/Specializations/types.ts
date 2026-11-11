import { DepartmentType } from '@/types/configuration';

export interface SpecializationFormProps {
	handleClose: () => void;
	onSubmit: (values: SpecializationFormValues) => Promise<unknown>;
	id?: number | string;
}

export type SpecializationDepartmentOption = Omit<
	DepartmentType,
	'shortName'
> & { disabled: boolean; disabledMessage?: string };

export interface SpecializationFormValues {
	name: string;
	hardSkills?: Array<{ id: number; name: string }> | undefined;
	departments: Array<SpecializationDepartmentOption> | undefined;
}
