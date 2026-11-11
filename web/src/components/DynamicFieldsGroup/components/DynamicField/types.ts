export interface DynamicFieldProps {
	label: string;
	value: string;
	name: string;
	onChange: (field: string, newValue: string) => void;
	isError: boolean;
	isRequired?: boolean;
}
