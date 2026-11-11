export interface UseColorPickerControlProps {
	onChange: (value: string, newValue: string) => void;
	fieldName: string;
	disabled?: boolean;
	randomColorsAmount?: number;
	numberOfColors?: number;
}
