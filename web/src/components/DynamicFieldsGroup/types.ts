export interface DynamicFieldsGroupProps {
	name: string;
	label: string;
	mainLabel?: string;
	additionalLabel?: string;
	gridArea?: string;
	maxNumberOfFields?: number;

	onChange?: (fieldName: string, fieldValue: string) => void;
}

export interface GetFieldLabelParams {
	index: number;
	label: string;
	mainLabel?: string;
	additionalLabel?: string;
}
