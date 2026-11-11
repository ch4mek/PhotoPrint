export interface ColorPickerControlProps {
	randomColors: string[];
	flexible: boolean;
	spaceBetween: boolean;
	handleColorChange: (newValue: string) => void;
	disabled: boolean;
}
