import { JiraSettingsFormValues } from '../../types';

export interface JiraSettingsFormContentProps {
	isLoading: boolean;
	onClose: VoidFunction;
	handleOutsideClose: VoidFunction;
	popUpTitle: string;
	isEditing: boolean;
}

export type JiraSettingsFormFields = {
	[Field in keyof JiraSettingsFormValues]: {
		name: Field;
		label: string;
		valueField?: string;
		withTrimOnBlur?: boolean;
	};
};
