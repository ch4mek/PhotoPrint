import { Dispatch, SetStateAction } from 'react';

export interface YearFilterControlsProps {
	year: string;
	onYearChange: Dispatch<SetStateAction<string>>;
	onReset: VoidFunction;
	isResetDisabled: boolean;
}
