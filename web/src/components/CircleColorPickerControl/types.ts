export interface CircleColorPickerControlProps {
	value: string;
	handleApplyColor: (color: string) => void;
	disabled: boolean;
}

export interface UseCircleColorPickerControlProps {
	handleApplyColor: (color: string) => void;
}
