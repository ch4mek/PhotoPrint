export interface ColorPickerFieldProps {
	name: string;
	value: string;
	disabled?: boolean;
	flexible?: boolean;
	numberOfColors?: number;
	fullWidth?: boolean;
	spaceBetween?: boolean;
	onChange?: (value: string, newValue: string) => void;
	randomColorsAmount?: number;
}
