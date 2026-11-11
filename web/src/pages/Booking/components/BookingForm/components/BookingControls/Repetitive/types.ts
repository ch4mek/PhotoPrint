import { ChangeEvent } from 'react';

export interface RepetitiveInnerProps {
	isChecked: boolean;
	isControlDisabled: boolean;
	weeksCount: number;
	handleWeeksChange: (value: number) => void;
	handleChange: (event: ChangeEvent<HTMLInputElement>) => void;
}
