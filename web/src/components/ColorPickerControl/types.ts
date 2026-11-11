import { ButtonHTMLAttributes } from 'react';

export type ColorPickerWrapperProps = {
	disabled: boolean;
};

export type ColorPickerButtonProps = ColorPickerWrapperProps &
	ButtonHTMLAttributes<HTMLButtonElement> & {
		isActive: boolean;
	};

export type RandomColorsWrapperProps = {
	flexible?: boolean;
	fullWidth?: boolean;
	spaceBetween?: boolean;
};

export interface ColorPickerProps {
	value: string;
	onChange: (value: string, newValue: string) => void;
	fieldName: string;
	disabled?: boolean;
	flexible?: boolean;
	numberOfColors?: number;
	fullWidth?: boolean;
	spaceBetween?: boolean;
	randomColorsAmount?: number;
}
