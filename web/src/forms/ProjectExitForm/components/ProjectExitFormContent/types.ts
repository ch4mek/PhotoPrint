import { ProjectExitFormValues } from '../../types';

export interface ProjectExitFormContentProps {
	title: string;
	isLoading: boolean;
	onClose: VoidFunction;
	handleOutsideClose: VoidFunction;
}

export type ProjectExitFormContentFields = {
	[Field in keyof ProjectExitFormValues]: {
		name: Field;
		label: string;
		valueField?: string;
	};
};
