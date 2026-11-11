export interface AllDaySwitchProps {
	checked: boolean;
	isDisabled: boolean;
	handleChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
}
